using MongoDB.Bson;
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
    public class UsuarioService
    {
        private readonly MongoConnection _context;

        public UsuarioService(MongoConnection context)
        {
            _context = context;
        }

        // CORREÇÃO DO ERRO: Trocamos ReplaceOne por UpdateOne com .Set()
        // Isso evita o erro de "immutable field _id" 
        public void InserirUsuarios(List<Usuario> usuarios)
        {
            foreach (var u in usuarios)
            {
                var filter = Builders<Usuario>.Filter.Eq(user => user.Email, u.Email);

                // Em vez de substituir o objeto todo, atualizamos campo por campo
                // Se o usuário não existir (Upsert), o MongoDB cria um novo com um ID válido
                var update = Builders<Usuario>.Update
                    .Set(user => user.Nome, u.Nome)
                    .Set(user => user.Email, u.Email)
                    .Set(user => user.Idade, u.Idade)
                    .Set(user => user.Perfil, u.Perfil);

                _context.Usuarios.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
            }
            Console.WriteLine("Processamento de usuários concluído!");
        }

        public Usuario BuscarPorEmail(string email)
        {
            return _context.Usuarios
                .Find(u => u.Email == email)
                .FirstOrDefault();
        }

        // Requisito 2.2: Uso de $inc para campos numéricos
        public void IncrementarTreinos(Usuario usuario)
        {
            if (usuario == null) return;

            var update = Builders<Usuario>.Update.Inc(u => u.Perfil.TotalTreinos, 1);
            _context.Usuarios.UpdateOne(u => u.Id == usuario.Id, update);
        }

        public void ListarUsuarios()
        {
            var usuarios = _context.Usuarios.Find(_ => true).ToList();

            Console.WriteLine("\n=== LISTA DE ALUNOS ATIVOS ===");
            foreach (var u in usuarios)
            {
                // Verificação de segurança para o Perfil não dar erro de nulo
                var objetivo = u.Perfil != null ? u.Perfil.Objetivo : "Não definido";
                var treinos = u.Perfil != null ? u.Perfil.TotalTreinos : 0;

                Console.WriteLine($"{u.Nome.PadRight(20)} | {u.Email.PadRight(25)} | {objetivo} | Treinos: {treinos}");
            }
        }
    }
}