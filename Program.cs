using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace TreinoMongoDB
{
    // --- MODELOS DE DADOS (CLASSES) ---
    public class Perfil {
        public double Peso { get; set; }
        public double Altura { get; set; }
        public string Objetivo { get; set; }
        public int TotalTreinos { get; set; }
    }

    public class Usuario {
        [BsonId] public ObjectId Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public int Idade { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class Exercicio {
        public string Nome { get; set; }
        public string Series { get; set; } // Ex: "4x12" ou "30 min"
    }

    public class FichaTreino {
        [BsonId] public ObjectId Id { get; set; }
        public string NomeTreino { get; set; }
        public List<Exercicio> Exercicios { get; set; }
    }

    public class Historico {
        [BsonId] public ObjectId Id { get; set; }
        public ObjectId UsuarioId { get; set; }
        public DateTime Data { get; set; }
        public string Modalidade { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // CONEXÃO
            var client = new MongoClient("mongodb://127.0.0.1:27017");
            var db = client.GetDatabase("TREINO_DB");

            var usuariosColl = db.GetCollection<Usuario>("usuarios");
            var fichasColl = db.GetCollection<FichaTreino>("fichas_treino");
            var historicoColl = db.GetCollection<Historico>("historico_atividades");

            // 1. POPULANDO USUÁRIOS (Diversidade de perfis)
            var listaUsuarios = new List<Usuario> {
                new Usuario { Nome = "Daniel de Freitas", Email = "daniel.cunha@gmail.com", Idade = 20, Perfil = new Perfil { Peso = 72, Altura = 1.77, Objetivo = "Ganho de Massa", TotalTreinos = 0 } },
                new Usuario { Nome = "Pam Adrielly", Email = "pam.adrielly@email.com", Idade = 25, Perfil = new Perfil { Peso = 60, Altura = 1.60, Objetivo = "Hipertrofia", TotalTreinos = 0 } },
                new Usuario { Nome = "Marcos Oliveira", Email = "marcos.oliva@gmail.com", Idade = 42, Perfil = new Perfil { Peso = 95, Altura = 1.85, Objetivo = "Emagrecimento", TotalTreinos = 0 } },
                new Usuario { Nome = "Ana Beatriz", Email = "ana.bea@outlook.com", Idade = 19, Perfil = new Perfil { Peso = 55, Altura = 1.68, Objetivo = "Condicionamento", TotalTreinos = 0 } },
                new Usuario { Nome = "Ricardo Santos", Email = "ricardo.s@gmail.com", Idade = 35, Perfil = new Perfil { Peso = 80, Altura = 1.75, Objetivo = "Definição", TotalTreinos = 0 } },
                new Usuario { Nome = "Carla Mendes", Email = "carla.m@uol.com.br", Idade = 28, Perfil = new Perfil { Peso = 68, Altura = 1.62, Objetivo = "Resistência", TotalTreinos = 0 } }
            };

            foreach (var u in listaUsuarios) {
                var filter = Builders<Usuario>.Filter.Eq(user => user.Email, u.Email);
                usuariosColl.ReplaceOne(filter, u, new ReplaceOptions { IsUpsert = true });
            }

            // 2. POPULANDO FICHAS DE TREINO (A, B, C, D)
            var listaFichas = new List<FichaTreino> {
                new FichaTreino { 
                    NomeTreino = "Treino A - Inferiores", 
                    Exercicios = new List<Exercicio> { 
                        new Exercicio { Nome = "Agachamento", Series = "4 séries de 10" },
                        new Exercicio { Nome = "Leg Press", Series = "3 séries de 12" },
                        new Exercicio {Nome = "Agachamento Sumo", Series = "3x 12"},
                        new Exercicio {Nome = "Bulgaro", Series = " 3 x 10"},
                        new Exercicio {Nome = "Esteira",Series = "10 Minutos"}
                    } 
                },
                new FichaTreino { 
                    NomeTreino = "Treino B - Peito e Tríceps", 
                    Exercicios = new List<Exercicio> { 
                        new Exercicio { Nome = "Supino Reto", Series = "4 séries de 8" },
                        new Exercicio { Nome = "Tríceps Pulley", Series = "3 séries de 15" },
                        new Exercicio {Nome = "Crusifixo Maquina", Series = "3 x 20"},
                        new Exercicio {Nome ="Triceps Banco", Series = "3 x 15"}
                    } 
                },
                new FichaTreino { 
                    NomeTreino = "Treino C - Costas e Bíceps", 
                    Exercicios = new List<Exercicio> { 
                        new Exercicio { Nome = "Puxada Alta", Series = "4 séries de 10" },
                        new Exercicio { Nome = "Rosca Direta", Series = "3 séries de 12" },
                        new Exercicio { Nome = "Levantamento Terra", Series = "3 X 8"},
                        new Exercicio { Nome = "Remada Curvada com barra",Series = "3x12"}
                    } 
                },
                new FichaTreino { 
                    NomeTreino = "Treino D - Cardio e Core", 
                    Exercicios = new List<Exercicio> { 
                        new Exercicio { Nome = "Corrida", Series = "20 minutos" },
                        new Exercicio { Nome = "Plancha", Series = "3 séries de 1 min" },
                        new Exercicio {Nome = "Abdominal Remador", Series = "3x30"},
                        new Exercicio {Nome = "Abdominal Infra",Series = "3X15"}
                    } 
                }
            };

            foreach (var f in listaFichas) {
                var filter = Builders<FichaTreino>.Filter.Eq(ficha => ficha.NomeTreino, f.NomeTreino);
                fichasColl.ReplaceOne(filter, f, new ReplaceOptions { IsUpsert = true });
            }

            // (Registrando treinos para o relatório)
            var daniel = usuariosColl.Find(u => u.Email == "daniel.cunha@gmail.com").FirstOrDefault();
            var ana = usuariosColl.Find(u => u.Email == "ana.bea@outlook.com").FirstOrDefault();

            if (daniel != null) {
                historicoColl.InsertOne(new Historico { UsuarioId = daniel.Id, Data = DateTime.Now, Modalidade = "Musculação Profissional" });
                usuariosColl.UpdateOne(u => u.Id == daniel.Id, Builders<Usuario>.Update.Inc(u => u.Perfil.TotalTreinos, 1));
            }

            if (ana != null) {
                historicoColl.InsertOne(new Historico { UsuarioId = ana.Id, Data = DateTime.Now, Modalidade = "HIIT Aeróbico" });
                usuariosColl.UpdateOne(u => u.Id == ana.Id, Builders<Usuario>.Update.Inc(u => u.Perfil.TotalTreinos, 1));
            }

            Console.WriteLine("Banco TREINO_DB! Usuários e Fichas integrados.");

            // RELATÓRIO DE MONITORAMENTO DOS ALUNOS
            var query = historicoColl.Aggregate()
                .Lookup("usuarios", "UsuarioId", "_id", "user")
                .Unwind("user")
                .Project(BsonDocument.Parse("{ Aluno: '$user.Nome', Atividade: '$Modalidade', Objetivo: '$user.Perfil.Objetivo' }"))
                .ToList();

            Console.WriteLine("\n RELATORIO DE ÚLTIMAS ATIVIDADES NO SISTEMA:");
            
            foreach (var item in query) {
                Console.WriteLine($"- {item["Aluno"]} ({item["Objetivo"]}) fez {item["Atividade"]}");
            }
        }
    }
}