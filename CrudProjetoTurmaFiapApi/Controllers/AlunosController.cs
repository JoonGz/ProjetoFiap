using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProjetoTurmasFiap.Entities;
using System;
using System.Security.Cryptography;
using System.Text;

namespace CrudProjetoTurmaFiapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlunosController : ControllerBase
    {
        #region Propriedades
        private readonly string _connectionString;
        private readonly RNGCryptoServiceProvider _rngCsp = new RNGCryptoServiceProvider();
        #endregion

        #region Construtor
        public AlunosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ProjFiapCs");
        }
        #endregion

        #region Eventos CRUD Alunos
        [HttpGet]
        public async Task<IActionResult> GetAllAlunos()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "SELECT * FROM Aluno";

                var Alunos = await sqlConnection.QueryAsync<Aluno>(sql);

                return Ok(Alunos);
            }           
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlunoById(int id)
        {
            var parameters = new
            {
                id
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "SELECT * FROM Aluno WHERE Id = @id";

                var Aluno = await sqlConnection.QuerySingleOrDefaultAsync<Aluno>(sql, parameters);

                if (Aluno is null)
                {
                    return NotFound();
                }

                return Ok(Aluno);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CriarAluno(Aluno aluno)
        {
            aluno.Ativo = true;

            var validacao = verificaForcaSenha(aluno.Senha);

            if (validacao == ForcaDaSenha.Inaceitavel || validacao == ForcaDaSenha.Fraca)
            {
                return NotFound("A senha informada está fraca.");
            }

            aluno.Senha = CriarHash(aluno.Senha);

            var parameters = new
            {
                aluno.Nome,
                aluno.Usuario,
                aluno.Senha,
                aluno.Ativo
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "INSERT INTO Aluno OUTPUT INSERTED.Id VALUES (@Nome, @Usuario, @Senha, @ativo)";

                var id = await sqlConnection.ExecuteScalarAsync<int>(sql, parameters);

                return Ok(id);
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditarAluno(Aluno aluno)
        {
            var parameters = new
            {
                aluno.Id,
                aluno.Nome,
                aluno.Usuario,
                aluno.Senha
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "UPDATE Aluno SET nome = @nome, usuario = @usuario, senha = @senha WHERE id = @id";

                await sqlConnection.ExecuteAsync(sql, parameters);

                return Ok("Atualizado com sucesso.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> InativarAluno(int id)
        {
            var parameters = new
            {
                id
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "UPDATE Aluno SET ativo = 0 WHERE id = @id";

                await sqlConnection.ExecuteAsync(sql, parameters);

                return Ok("Inativado com sucesso.");
            }
        }
        #endregion

        #region Métodos Auxiliares
        private string CriarHash(string texto)
        {
            var sha = SHA256.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(texto);
            byte[] hash = sha.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        private ForcaDaSenha verificaForcaSenha(string senha)
        {
            ChecaForcaSenha item = new ChecaForcaSenha();
            return item.GetForcaDaSenha(senha);
        }
        #endregion
    }
}
