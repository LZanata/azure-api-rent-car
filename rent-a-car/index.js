const express = require('express');
const cors = require('cors');
const { DefaultAzureCredential } = require("@azure/identity");
const { ServiceBusClient } = require("@azure/service-bus");
require('dotenv').config();
const app = express();
app.use(cors());
app.use(express.json());

app.post('/api/locacao', async (req, res) => {
    const { nome, email, modelo, ano, tempoAluguel } = req.body;
    const connectionString = process.env.SERVICE_BUS_CONNECTION_STRING;

    const mensagem = {
        nome,
        email,
        modelo,
        ano,
        tempoAluguel,
        data: new Date().toISOString(),
    };

    try{
        const credential = new DefaultAzureCredential();
        const serviceBusConnection = connectionString;
        const queueName = "fila-locacao-auto";
        const sbClient = new ServiceBusClient(serviceBusConnection);
        const sender = sbClient.createSender(queueName);
        const message = {
            body: mensagem,
            contentType: "application/json",
            label: "locacao",
        };

        await sender.sendMessages(message);
        await sender.close();
        await sbClient.close();

        res.status(201).send("Locação de Veículo enviada para a fila com sucesso");

    }catch (error) {
        console.error("Erro ao enviar mensagem:", error);
        return res.status(500).send("Erro ao enviar mensagem para a fila.");
    }
})

app.listen(5001, () => {
    console.log("Servidor rodando na porta 5001");
});
