using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treino.Models
{
    public class Treino
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Nome { get; set; }
            = string.Empty;

        // Documento aninhado
        public List<Exercicio> Exercicios
        { get; set; }
            = new List<Exercicio>();
    }

    public class Exercicio
    {
        public string Nome { get; set; }
            = string.Empty;

        public int Series { get; set; }

        public int Repeticoes { get; set; }
    }
}
