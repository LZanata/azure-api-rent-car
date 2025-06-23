# azure-api-aluguel-carros

## Descrição

Este projeto é uma API Node.js para simular o aluguel de carros, integrando serviços do Azure como Key Vault e Service Bus. O objetivo é demonstrar boas práticas de segurança, mensageria e organização de código em aplicações modernas na nuvem.

---

## Como rodar o projeto

1. **Clone o repositório**
   ```sh
   git clone https://github.com/LZanata/azure-api-rent-car.git
   cd seu-repositorio/app-locadora
   ```

2. **Configure as variáveis de ambiente**
   - Crie um arquivo `.env` na pasta `app-locadora`:
     ```
     KEY_VAULT_URL=https://<nome-do-seu-keyvault>.vault.azure.net/
     ```
   - O segredo `ServiceBusConnectionString` deve estar criado no seu Azure Key Vault.

3. **Instale as dependências**
   ```sh
   npm install
   ```

4. **Execute a aplicação**
   ```sh
   npm start
   ```
   A API estará disponível em `http://localhost:3001`.

5. **Exemplo de requisição**
   ```sh
   curl -X POST http://localhost:3001/api/locacao \
     -H "Content-Type: application/json" \
     -d '{"nome": "João", "email": "joao@email.com"}'
   ```

---

## Estrutura do Projeto

```
app-locadora/
│
├── index.js          # Código principal da API
├── package.json      # Dependências e scripts
├── dockerfile        # Dockerização da aplicação
├── .env              # Variáveis de ambiente (não versionar)
└── README.md         # Este arquivo
```

---

## Principais decisões e aprendizados

- **Segurança:** Utilização do Azure Key Vault para armazenar secrets, evitando exposição de credenciais no código.
- **Mensageria:** Integração com Azure Service Bus para envio de mensagens de locação, simulando um fluxo assíncrono.
- **Boas práticas:** Uso de variáveis de ambiente, tratamento de erros e validação de dados de entrada.
- **Docker:** Projeto pronto para ser containerizado, facilitando o deploy em qualquer ambiente.

---

## Possibilidades de evolução

- Implementar autenticação JWT para proteger a API.
- Adicionar testes automatizados.
- Criar endpoints para consultar status das locações.
- Integrar com um banco de dados para persistência.

---

## Insights

Durante o desenvolvimento, aprendi a importância de separar as responsabilidades do código, proteger informações sensíveis e como a mensageria pode tornar sistemas mais escaláveis e desacoplados. O uso do Azure Key Vault e Service Bus mostrou como é possível integrar facilmente recursos de nuvem em aplicações Node.js.

---

## Como entregar

1. Crie um repositório no GitHub e suba o código.
2. Complete este README com prints e, se desejar, mais detalhes do seu aprendizado.
3. Compartilhe o link do repositório conforme instruções do curso.