using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using treino.Data;
using treino.Models;

namespace treino.Services
{
    public class TreinoService
    {
        private readonly MongoConnection _context;

        public TreinoService(
            MongoConnection context
        )
        {
            _context = context;
        }

       public void InserirTreinos(List<Treino> treinos)
        {
            foreach (var treino in treinos)
            {
                var filtro = Builders<Treino>.Filter.Eq(t => t.Nome, treino.Nome);

                var update = Builders<Treino>.Update
                .Set(t => t.Nome, treino.Nome)
                .Set(t => t.Exercicios, treino.Exercicios);

                 _context.Treinos.UpdateOne(
                filtro,
                update,
                new UpdateOptions { IsUpsert = true }
            );
        }

        Console.WriteLine("Processamento de treinos concluído!");
        }

        // $push
        public void AdicionarExercicio(
            string nomeTreino,
            Exercicio exercicio
        )
        {
            var update =
                Builders<Treino>.Update
                .Push(
                    t => t.Exercicios,
                    exercicio
                );

            _context.Treinos.UpdateOne(
                t => t.Nome == nomeTreino,
                update
            );
        }

        public void ListarTreinos()
        {
            var treinos =
                _context.Treinos
                .Find(_ => true)
                .ToList();

            Console.WriteLine(
                "\n=== TREINOS ==="
            );

            foreach (var treino in treinos)
            {
                Console.WriteLine(
                    $"\n{treino.Nome}"
                );

                foreach (
                    var ex in treino.Exercicios
                )
                {
                    Console.WriteLine(
                        $"- {ex.Nome} | {ex.Series}x{ex.Repeticoes}"
                    );
                }
            }
        }
    }
}
