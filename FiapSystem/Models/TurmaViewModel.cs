namespace ProjetoTurmasFiap.Entities
{
    public class TurmaViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Ano { get; set; }
        public bool Ativo { get; set; }

        //Apoio para parte de Associação de alunos com as turmas
        public int QtdAlunos { get; set; }
    }
}
