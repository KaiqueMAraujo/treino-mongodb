using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treino.Models
{
    public class Perfil
    {
        public double Peso { get; set; }

        public double Altura { get; set; }

        public string Objetivo { get; set; }
            = string.Empty;

        public int TotalTreinos { get; set; }
    }

    public class Usuario
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Nome { get; set; }
            = string.Empty;

        public string Email { get; set; }
            = string.Empty;

        public int Idade { get; set; }

        public Perfil Perfil { get; set; }
            = new Perfil();
    }

}
