using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using OrderSystem;
using System;
using static OrderSystem.Program;

namespace OrderSystem.Services
{
    public class QueueService
    {
        private readonly string _connectionString;

        // Initializes the QueueService with configuration and sets up the connection string
        public QueueService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureQueueStorage");
        }

        // Sends a message to the specified Azure queue
        public async Task SendMessageAsync(string queueName, string message)
        {
            var queueClient = new QueueClient(_connectionString, queueName, new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            }); // automatically encodes messages in Base64

            await queueClient.CreateIfNotExistsAsync();

            if (await queueClient.ExistsAsync())
            {
                await queueClient.SendMessageAsync(message);
            }
        }
    }
}
