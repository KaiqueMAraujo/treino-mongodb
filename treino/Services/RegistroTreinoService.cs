using MongoDB.Bson;
using MongoDB.Bson.IO;
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
    public class RegistroTreinoService
    {
        private readonly MongoConnection _context;

        public RegistroTreinoService(
            MongoConnection context
        )
        {
            _context = context;
        }

        public void RegistrarTreino(
            RegistroTreino registro
        )
        {
            _context.Registros
                .InsertOne(registro);
        }

        // $lookup
        public void RelatorioUsuariosTreinos()
        {
            var resultado =
                _context.Registros
                .Aggregate()

                .Lookup(
                    "usuarios",
                    "UsuarioId",
                    "_id",
                    "usuario"
                )

                .Unwind("usuario")

                .Lookup(
                    "treinos",
                    "TreinoId",
                    "_id",
                    "treino"
                )

                .Unwind("treino")

                .ToList();

            Console.WriteLine(
                "\n=== RELATÓRIO ==="
            );

            foreach (var doc in resultado)
            {
                Console.WriteLine(
                    doc.ToJson(
                        new JsonWriterSettings
                        {
                            Indent = true
                        }
                    )
                );
            }
        }

        public void Dashboard()
        {
            var total =
                _context.Registros
                .CountDocuments(_ => true);

            Console.WriteLine(
                "\n=== DASHBOARD ==="
            );

            Console.WriteLine(
                $"Total de treinos registrados: {total}"
            );
        }
    }
}