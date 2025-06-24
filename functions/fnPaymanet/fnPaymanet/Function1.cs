using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using fnPaymanet.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace fnPaymanet;

public class Payment
{
    private readonly ILogger<Payment> _logger;
    private readonly IConfiguration _configuration;
    private readonly string[] StatusList = ["Aprovado", "Reprovado", "Em análise"];
    private readonly Random random = new Random();
    public Payment(ILogger<Payment> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task SentToNotification { get; private set; }

    [Function(nameof(Payment))]
    [CosmosDBOutput("%CosmosDb%", "%CosmosContainer%", Connection = "CosmosDBConnection", CreateIfNotExists = true)]
    public async Task<PaymentModel> Run(
        [ServiceBusTrigger("payment-queue", Connection = "ServiceBusConnection")]
Azure.Messaging.ServiceBus.ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        PaymentModel payment = null;

        try
        {
            payment = JsonSerializer.Deserialize<PaymentModel>(message.Body.ToString(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (payment == null)
            {
                await messageActions.DeadLetterMessageAsync(message,null, "The message coud not be desialized.");
            }
            int index = random.Next(StatusList.Length);
            string status = StatusList[index];
            payment.Status = status;

            if (status == "Aprovado")
            {
                payment.DataAprovacao = DateTime.Now;
                await SentToNotification{payment};
            }

            return payment;

        }
        catch (Exception)
        {
            await messageActions.DeadLetterMessageAsync(message, null, $"Erro:{ex.Message}");
        }

        finally
        {
            await messageActions.CompleteMessageAsync(message);
        }

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
    private async Task SentToNotificationQueue(PaymentModel payment)
    {
        var connectionString = _configuration["ServiceBusConnection"];
        var queueName = _configuration.GetSection("NotificationQueue").Value.ToString();
    
        var serviceBusClient = new ServiceBusClient(connectionString);
        var sender = serviceBusClient.CreateSender(queueName);
        var message = new ServiceBusMessage(JsonSerializer.Serialize(payment))
        {
            ContentType = "application/json",
            MessageId = "Payment Notification"
        };

        message.ApplicationProperties.Add("IdPayment", payment.id);
        message.ApplicationProperties.Add("type", "notification");
        message.ApplicationProperties.Add("message", "Pagamento Aprovado com Sucesso");

        try
        {
            await sender.SendMessageAsync(message);
            _logger.LogInformation("Payment notification sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send payment notification.");
            throw;
        }
        finally
        {
            await sender.DisposeAsync();
            await serviceBusClient.DisposeAsync();
        }

    }
}