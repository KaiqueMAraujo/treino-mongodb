using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using treino.Data;
using treino.Models;
using treino.Services;

namespace treino
{
    public class Program
    {
        static void Main(string[] args)
        {
            var conexao = new MongoConnection();
            var usuarioService = new UsuarioService(conexao);
            var treinoService = new TreinoService(conexao);
            var registroService = new RegistroTreinoService(conexao);

            Console.WriteLine("--- SISTEMA DE TREINO MONGODB ---");

            // MENU INICIAL
            Console.WriteLine("\nEscolha uma opção:");
            Console.WriteLine("1 - Cadastrar Novo Aluno (Teclado)");
            Console.WriteLine("2 - Executar Carga Automática e Relatórios");
            Console.Write("\nOpção: ");
            string opcao = Console.ReadLine();

            if (opcao == "1")
            {
                // INTERAÇÃO VIA TECLADO (Requisito de interface CLI)
                Console.WriteLine("\n--- NOVO CADASTRO DE ALUNO ---");

                Console.Write("Nome: ");
                string nome = Console.ReadLine();

                Console.Write("Email: ");
                string email = Console.ReadLine();

                Console.Write("Idade: ");
                int idade = int.Parse(Console.ReadLine() ?? "0");

                Console.Write("Peso (kg): ");
                double peso = double.Parse(Console.ReadLine() ?? "0");

                Console.Write("Altura (m): ");
                double altura = double.Parse(Console.ReadLine() ?? "0");

                Console.Write("Objetivo (ex: Hipertrofia, Emagrecimento): ");
                string obj = Console.ReadLine();

                var novoAluno = new Usuario
                {
                    Nome = nome,
                    Email = email,
                    Idade = idade,
                    Perfil = new Perfil
                    {
                        Peso = peso,
                        Altura = altura,
                        Objetivo = obj,
                        TotalTreinos = 0
                    }
                };

                // Insere usando a lógica de Update/Upsert que evita erro de ID imutável
                usuarioService.InserirUsuarios(new List<Usuario> { novoAluno });
                Console.WriteLine("\nAluno cadastrado com sucesso!");
            }

            // EXECUÇÃO DOS REQUISITOS TÉCNICOS (Para o seu vídeo)
            Console.WriteLine("\n--- PROCESSANDO DADOS DO PROJETO ---");

            // 1. Carga de Usuários Padrão (Garante dados para os relatórios analíticos)
            var listaPadrao = new List<Usuario>
            {
                new Usuario { Nome = "Daniel", Email = "daniel@gmail.com", Idade = 20,
                    Perfil = new Perfil { Peso = 72, Altura = 1.77, Objetivo = "Hipertrofia", TotalTreinos = 0 }},
                new Usuario { Nome = "Ana", Email = "ana@gmail.com", Idade = 19,
                    Perfil = new Perfil { Peso = 58, Altura = 1.65, Objetivo = "Condicionamento", TotalTreinos = 0 }}
            };
            usuarioService.InserirUsuarios(listaPadrao);

            // 2. Operações de Treino ($push para manipulação de listas)
            var treinos = new List<Treino> {
                new Treino { Nome = "Treino A", Exercicios = new List<Exercicio> {
                    new Exercicio { Nome = "Supino", Series = 4, Repeticoes = 12 }
                }}
            };
            treinoService.InserirTreinos(treinos);
            treinoService.AdicionarExercicio("Treino A", new Exercicio { Nome = "Flexão", Series = 3, Repeticoes = 15 });

            // 3. Registro e Atualização ($inc para contagem de treinos)
            var usuario = usuarioService.BuscarPorEmail("daniel@gmail.com");
            var treino = conexao.Treinos.Find(t => t.Nome == "Treino A").FirstOrDefault();

            if (usuario != null && treino != null)
            {
                registroService.RegistrarTreino(new RegistroTreino
                {
                    UsuarioId = usuario.Id,
                    TreinoId = treino.Id,
                    Duracao = 60
                });
                usuarioService.IncrementarTreinos(usuario);
            }

            // 4. Exibição dos Relatórios Obrigatórios (Aggregation Framework)
            usuarioService.ListarUsuarios();
            treinoService.ListarTreinos();

            Console.WriteLine("\n--- RELATÓRIO 1 (JUNÇÃO $LOOKUP) ---");
            registroService.RelatorioUsuariosTreinos();

            Console.WriteLine("\n--- RELATÓRIO 2 (DASHBOARD $FACET) ---");
            registroService.Dashboard();

            Console.WriteLine("\nProcesso finalizado. Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
}
