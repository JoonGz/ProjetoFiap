using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoTurmasFiap.Entities
{
    public class AlunoTurmaViewModel
    {
        public int Aluno_id { get; set; }
        public int Turma_id { get; set; }
        public bool Ativo { get; set; }
        public string? NomeTurma { get; set; }
        public int AnoTurma { get; set; }
        public int QtdAlunos { get; set; }
        public string? NomeAluno { get; set; }
        public int Aluno_idOld { get; set; }
        public int Turma_idOld { get; set; }

    }
}
