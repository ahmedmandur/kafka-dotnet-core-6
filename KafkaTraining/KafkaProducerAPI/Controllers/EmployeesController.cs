using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace KafkaProducerAPI.Controllers;

public class EmployeesController : Controller
{
    private readonly string bootstrapServers = "localhost:9092";
    private readonly string topic = "training-kafka";

    [HttpPost("api/employees/post")]
    public async Task<IActionResult> Post([FromBody] EmployeeRequest request)
    {
        var message = JsonSerializer.Serialize(request);
        return Ok(await SendOrderRequest(topic, message));
    }

    private async Task<bool> SendOrderRequest(string topic, string message)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            ClientId = Dns.GetHostName()
        };

        try
        {
            using var producer = new ProducerBuilder<Null, string>(config).Build();

            var result = await producer.ProduceAsync(topic, new Message<Null, string>
            {
                Value = message
            });

            Debug.WriteLine($"Delivery Timestamp:{result.Timestamp.UtcDateTime}");
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occured: {ex.Message}");
        }

        return await Task.FromResult(false);
    }
}