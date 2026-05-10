using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using treino.Models;

namespace treino.Data
{
    public class MongoConnection
    {
        private readonly IMongoDatabase _database;

        public MongoConnection()
        {
            var client =
                new MongoClient(
                    "mongodb://127.0.0.1:27017"
                );

            _database =
                client.GetDatabase("TREINO_DB");
        }

        public IMongoCollection<Usuario>
            Usuarios =>

            _database.GetCollection<Usuario>(
                "usuarios"
            );

        public IMongoCollection<Treino>
            Treinos =>

            _database.GetCollection<Treino>(
                "treinos"
            );

        public IMongoCollection<RegistroTreino>
            Registros =>

            _database.GetCollection<RegistroTreino>(
                "registros"
            );

        public IMongoCollection<Historico>
            Historicos =>

            _database.GetCollection<Historico>(
                "historicos"
            );
    }
}