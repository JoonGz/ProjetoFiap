using Dapper;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProjetoTurmasFiap.Entities;

namespace CrudProjetoTurmaFiapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurmasController : ControllerBase
    {
        #region Propriedades
        private readonly string _connectionString;
        #endregion

        #region Construtor
        public TurmasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ProjFiapCs");
        }
        #endregion

        #region Eventos CRUD Turmas
        [HttpGet]
        public async Task<IActionResult> GetAllTurmas()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "SELECT * FROM Turma WHERE ativo = 1";

                var turmas = await sqlConnection.QueryAsync<Turma>(sql);

                return Ok(turmas);
            }           
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTurmaById(int id)
        {
            var parameters = new
            {
                id
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "SELECT * FROM Turma WHERE Id = @id";

                var Turma = await sqlConnection.QuerySingleOrDefaultAsync<Turma>(sql, parameters);

                if (Turma is null)
                {
                    return NotFound();
                }

                return Ok(Turma);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CriarTurma(Turma turma)
        {
            var turmaExiste = BuscaTurmaByName(turma.Nome);
            if (turmaExiste.Result != null)
            {
                return BadRequest("O nome de turma informado já existe.");
            }
            if (turma.Ano < DateTime.Now.Year)
            {
                return BadRequest("Não é permitido cadastrar turmas com datas anteriores da atual.");
            }

            turma.Ativo = true;

            var parameters = new
            {
                turma.Nome,
                turma.Ano,
                turma.Ativo
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "INSERT INTO Turma OUTPUT INSERTED.Id VALUES (@Nome, @Ano, @ativo)";

                var id = await sqlConnection.ExecuteScalarAsync<int>(sql, parameters);

                return Ok(id);
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditarTurma(Turma turma)
        {
            if (turma.Ano < DateTime.Now.Year)
            {
                return BadRequest("Não é permitido cadastrar turmas com datas anteriores da atual.");
            }

            var parameters = new
            {
                turma.Id,
                turma.Nome,
                turma.Ano
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sqlValidaAtt = "SELECT nome FROM Turma WHERE id = @id";

                var nomeBase = await sqlConnection.QuerySingleOrDefaultAsync<String>(sqlValidaAtt, parameters);

                if (nomeBase != turma.Nome && BuscaTurmaByName(turma.Nome).Result != null)
                {
                    return BadRequest("O nome de turma informado já existe.");
                }

                const string sql = "UPDATE Turma SET nome = @nome, ano = @ano WHERE id = @id";

                await sqlConnection.ExecuteAsync(sql, parameters);

                return Ok("Atualizado com sucesso.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> InativarTurma(int id)
        {
            var parameters = new
            {
                id
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "UPDATE Turma SET ativo = 0 WHERE id = @id";

                await sqlConnection.ExecuteAsync(sql, parameters);

                return Ok("Inativado com sucesso.");
            }
        }
        #endregion

        #region Metodos Auxiliares
        private async Task<Turma> BuscaTurmaByName(string nome)
        {
            var parameters = new
            {
                nome
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "SELECT * FROM Turma WHERE nome = @nome";

                var Turma = await sqlConnection.QuerySingleOrDefaultAsync<Turma>(sql, parameters);

                if (Turma is null)
                {
                    return null;
                }

                return Turma;
            }
        }
        #endregion
    }
}
