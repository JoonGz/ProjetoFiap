Create table aluno
(
    id int primary key identity(1, 1),
    nome varchar(255) not null,
    usuario varchar(45) not null,
    senha char(64) not null,
	ativo bit default 1 not null
)

Create table turma
(
    id int primary key identity(1, 1),
    nome varchar(45) not null,
    ano int,
	ativo bit default 1 not null
)

Create table aluno_turma
(
    aluno_id int foreign key references aluno(id),
    turma_id int foreign key references turma(id),
	primary key (aluno_id, turma_id),
	ativo bit default 1 not null
)