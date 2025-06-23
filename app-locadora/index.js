const express = require('express');
const cors = require('cors');
const { DefaultAzureCredential } = require("@azure/identity");
const { SecretClient } = require("@azure/keyvault-secrets");
const { ServiceBusClient } = require("@azure/service-bus");
require('dotenv').config();

const app = express();
app.use(cors());
app.use(express.json());

let sbClient;
let sender;

// Função para inicializar/reutilizar Service Bus Client e Sender
async function initServiceBus() {
  if (!sbClient || !sender) {
    const credential = new DefaultAzureCredential();
    const keyVaultUrl = process.env.KEY_VAULT_URL;
    const secretClient = new SecretClient(keyVaultUrl, credential);

    // Busca a connection string do Key Vault
    const serviceBusSecret = await secretClient.getSecret("ServiceBusConnectionString");
    sbClient = new ServiceBusClient(serviceBusSecret.value);
    sender = sbClient.createSender("fila-locacao-auto");
  }
}

app.post('/api/locacao', async (req, res) => {
  const { nome, email } = req.body;

  // Validação simples dos campos obrigatórios
  if (!nome || !email) {
    return res.status(400).send("Nome e email são obrigatórios.");
  }

  const veiculo = {
    modelo: "Gol",
    ano: 2022,
    tempoAluguel: "1 semana",
  };

  const mensagem = {
    nome,
    email,
    ...veiculo,
    data: new Date().toISOString(),
  };

  try {
    await initServiceBus();

    const message = {
      body: mensagem,
      contentType: "application/json",
    };

    await sender.sendMessages(message);

    res.status(200).send("Locação enviada para a fila com sucesso");
  } catch (err) {
    console.error("Erro ao enviar mensagem para a fila:", err.message);
    res.status(500).send("Erro interno ao processar a locação.");
  }
});

// Fechar conexões ao encerrar o app
process.on('SIGINT', async () => {
  if (sender) await sender.close();
  if (sbClient) await sbClient.close();
  process.exit();
});

app.listen(3001, () => console.log("BFF rodando na porta 3001"));