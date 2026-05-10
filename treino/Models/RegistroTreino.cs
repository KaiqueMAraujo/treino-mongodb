using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treino.Models
{
    public class Historico
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public ObjectId UsuarioId { get; set; }

        public DateTime Data { get; set; }

        public string Modalidade { get; set; }
            = string.Empty;
    }

    public class RegistroTreino
    {
        [BsonId]
        public ObjectId Id { get; set; }

        // Referências com ObjectId
        public ObjectId UsuarioId { get; set; }

        public ObjectId TreinoId { get; set; }

        public int Duracao { get; set; }
    }
}
