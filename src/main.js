const { MongoClient, ObjectId } = require('mongodb');

// Configuração da conexão local
const url = 'mongodb://127.0.0.1:27017';
const client = new MongoClient(url);
const dbName = 'TREINO_DB'; // Nome do banco alterado conforme solicitado

async function main() {
    try {
        await client.connect();
        console.log(`Conectado ao banco de dados: ${dbName}`);
        const db = client.db(dbName);

        // Definição das coleções
        const usuarios = db.collection('usuarios');
        const fichas = db.collection('fichas_treino');
        const historico = db.collection('historico_atividades');

        // 1. Cadastro de Usuários
        const listaUsuarios = [
            { 
                email: "pam.adrielly@email.com", 
                nome: "Pam Adrielly", 
                idade: 25, 
                perfil: { peso: 60, altura: 1.60, objetivo: "Hipertrofia", total_treinos: 0 } 
            },
            { 
                email: "daniel.cunha@gmail.com", 
                nome: "Daniel de Freitas", 
                idade: 20, 
                perfil: { peso: 70, altura: 1.77, objetivo: "Ganho de Massa", total_treinos: 0 } 
            },
            { 
                email: "marcos.silva@email.com", 
                nome: "Marcos Silva", 
                idade: 30, 
                perfil: { peso: 85, altura: 1.80, objetivo: "Emagrecimento", total_treinos: 0 } 
            },
            { 
                email: "ana.souza@email.com", 
                nome: "Ana Souza", 
                idade: 22, 
                perfil: { peso: 55, altura: 1.65, objetivo: "Condicionamento", total_treinos: 0 } 
            },
            {
                email:"luiz.rocha@gmail.com",
                nome:"Luiz Henrique Rocha",
                idade: 55,
                perfil: {peso: 65, altura: 1.70, objetivo:"Condicionamento para Luta",total_treinos: 0}
            }
        ];

        // Inserção/Atualização de usuários
        for (const u of listaUsuarios) {
            await usuarios.updateOne(
                { email: u.email },
                { $set: u },
                { upsert: true }
            );
        }

        // Recupera o ID de um usuario para vincular os treinos a ele
        const usuarioAtivo = await usuarios.findOne({ email: "daniel.cunha@gmail.com" });
        const userId = usuarioAtivo._id;

        // 2. Cadastro de Fichas de Treino (A, B, C e D)
        const listaFichas = [
            { 
                nome_treino: "Treino A - Inferiores", 
                exercicios: [
                    { nome: "Agachamento Livre", series: 4, reps: 10 },
                    { nome: "Leg Press 45", series: 3, reps: 12 },
                    { nome: "Cadeira Extensora", series: 3 , reps: 12}
                ] 
            },
            { 
                nome_treino: "Treino B - Superiores", 
                exercicios: [
                    { nome: "Supino Reto", series: 4, reps: 8 },
                    { nome: "Remada Curvada", series: 3, reps: 10 },
                    { nome: "Crucifixo inverso", series: 3 , reps: 8}
                ] 
            },
            { 
                nome_treino: "Treino C - Core", 
                exercicios: [
                    { nome: "Prancha", series: 3, tempo: "1min" },
                    { nome: "Abdominal", series: 4, reps: 20 },
                    { nome: "Levantamento terra", series: 4, reps: 12 }
                ] 
            },
            { 
                nome_treino: "Treino D - Cardio", 
                exercicios: [
                    { nome: "Esteira", duracao: "30min" },
                    { nome: "Bicicleta", duracao: "15min" },
                    { nome: "Corda", duracao: "5min" }
                ] 
            }
        ];

        for (const f of listaFichas) {
            await fichas.updateOne(
                { nome_treino: f.nome_treino },
                { $set: f },
                { upsert: true }
            );
        }

        // 3. Atualização de indicadores ($inc) e Registro de Atividade
        // Incrementa o contador de treinos do usuário
        await usuarios.updateOne(
            { _id: userId },
            { $inc: { "perfil.total_treinos": 1 } }
        );

        // Adiciona um registro no histórico de atividades
        await historico.insertOne({
            usuario_id: userId,
            data: new Date(),
            modalidade: "Musculação",
            duracao_min: 60
        });

        console.log("Sincronização de dados finalizada com sucesso.");

        // RELATÓRIOS (AGGREGATION FRAMEWORK)

        // Relatório 1: Junção de coleções ($lookup + $unwind)
        console.log("Histórico de Atividades (Junção de Coleções)");
        const relatorio1 = await historico.aggregate([
            {
                $lookup: {
                    from: "usuarios",
                    localField: "usuario_id",
                    foreignField: "_id",
                    as: "usuario_info"
                }
            },
            { $unwind: "$usuario_info" },
            {
                $project: {
                    _id: 0,
                    Usuario: "$usuario_info.nome",
                    Atividade: "$modalidade",
                    Data: { $dateToString: { format: "%d/%m/%Y", date: "$data" } }
                }
            }
        ]).toArray();
        console.table(relatorio1);

        // Relatório 2: Análise Multifacetada ($facet)
        console.log("\nRELATÓRIO 2: Dashboard de Estatísticas");
        const relatorio2 = await usuarios.aggregate([
            {
                $facet: {
                    "Estatistica_Idade": [
                        { $group: { _id: null, media_idade: { $avg: "$idade" }, total_alunos: { $sum: 1 } } }
                    ],
                    "Distribuicao_Objetivos": [
                        { $group: { _id: "$perfil.objetivo", quantidade: { $sum: 1 } } }
                    ]
                }
            }
        ]).toArray();
        console.log(JSON.stringify(relatorio2, null, 2));

    } catch (err) {
        console.error("❌ Erro durante a execução:", err);
    } finally {
        await client.close();
    }
}

main();