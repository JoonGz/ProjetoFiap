using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProjetoTurmasFiap.Entities;
using System.Linq;

namespace CrudProjetoTurmaFiapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlunosTurmaController : ControllerBase
    {
        #region Propriedades
        private readonly string _connectionString;
        #endregion

        #region Construtor
        public AlunosTurmaController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ProjFiapCs");
        }
        #endregion

        #region Eventos CRUD Aluno Turmas
        [HttpGet]
        public async Task<IActionResult> GetAllTurmasGeradas()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql =
                    "SELECT A.turma_id, B.Nome NomeTurma, B.ano AnoTurma, consulta.QtdAlunos " +
                    "FROM Aluno_Turma A " +
                    "INNER JOIN Turma B ON A.turma_id = B.id " +
                    "OUTER APPLY (SELECT COUNT(aso.aluno_id) as 'QtdAlunos' FROM Aluno_Turma aso WHERE aso.turma_id = B.id and aso.ativo = 1) consulta " +
                    "WHERE consulta.QtdAlunos <> 0 " +
                    "GROUP BY A.turma_id, B.Nome, B.ano, consulta.QtdAlunos";

                var TurmasAso = await sqlConnection.QueryAsync<Aluno_Turma>(sql);

                return Ok(TurmasAso);
            }           
        }       

        [HttpGet("{idTurma}")]
        public async Task<IActionResult> GetAlunosByTurmaId(int idTurma)
        {
            var parameters = new
            {
                idTurma
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql =
                    "SELECT A.aluno_id, A.turma_id, A.ativo, B.Nome NomeTurma, B.ano AnoTurma, C.nome NomeAluno " +
                    "FROM Aluno_Turma A " +
                    "INNER JOIN Turma B ON A.turma_id = B.id " +
                    "INNER JOIN Aluno C ON A.aluno_id = C.id " +
                    "WHERE turma_id = @idTurma and A.ativo = 1";

                var alunoTurma = await sqlConnection.QueryAsync<Aluno_Turma>(sql, parameters);

                if (alunoTurma is null)
                {
                    return NotFound();
                }

                return Ok(alunoTurma);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CriarAlunoTurma(Aluno_Turma alunoTurma)
        {
            alunoTurma.Ativo = true;

            var parameters = new
            {
                alunoTurma.Aluno_id,
                alunoTurma.Turma_id,
                alunoTurma.Ativo
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "INSERT INTO Aluno_turma VALUES (@Aluno_id, @Turma_id, @Ativo)";

                try
                {
                    await sqlConnection.ExecuteAsync(sql, parameters);
                }
                catch (Exception ex)
                {
                    if (ex.HResult == -2146232060)
                    {
                        return BadRequest("A relação entre turma e aluno já existe.");
                    }
                }              

                return Ok(new { alunoTurma.Aluno_id, alunoTurma.Turma_id });
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditarAlunoTurma(Aluno_Turma alunoTurma)
        {
            if (alunoTurma.Aluno_idOld == 0 || alunoTurma.Turma_idOld == 0)
            {
                return NotFound();
            }

            var parameters = new
            {
                alunoTurma.Aluno_id,
                alunoTurma.Turma_id,
                alunoTurma.Aluno_idOld,
                alunoTurma.Turma_idOld
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                const string sql = "UPDATE Aluno_Turma SET aluno_id = @aluno_id, turma_id = @turma_id WHERE aluno_id = @Aluno_idOld and turma_id = @Turma_idOld";

                try
                {
                    await sqlConnection.ExecuteAsync(sql, parameters);
                }
                catch(Exception ex)
                {
                    if (ex.HResult == -2146232060)
                    {
                        return BadRequest("A relação entre turma e aluno já existe.");
                    }
                }

                return Ok("Atualizado com sucesso.");
            }
        }

        [HttpDelete("{idAluno}/{idTurma}")]
        public async Task<IActionResult> InativarAlunoTurma(int idAluno, int idTurma)
        {
            var parameters = new
            {
                idAluno,
                idTurma
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {                
                const string sql = "UPDATE Aluno_turma SET ativo = 0 WHERE aluno_id = @idAluno and turma_id = @idTurma";

                await sqlConnection.ExecuteAsync(sql, parameters);

                return Ok("Inativado com sucesso.");
            }
        }
        #endregion
    }
}
