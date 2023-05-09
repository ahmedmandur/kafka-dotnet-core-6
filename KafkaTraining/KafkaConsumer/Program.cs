using System.Diagnostics;
using System.Text.Json;
using Confluent.Kafka;
using Shared;

const string topic = "training-kafka";
const string groupId = "test_group";
const string bootstrapServers = "localhost:9092";


var config = new ConsumerConfig
{
    BootstrapServers = bootstrapServers,
    GroupId = groupId,
    AutoOffsetReset = AutoOffsetReset.Earliest
};

try
{
    using var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build();
    consumerBuilder.Subscribe(topic);
    var cancelToken = new CancellationTokenSource();

    try
    {
        while (true)
        {
            var consumer = consumerBuilder.Consume
                (cancelToken.Token);
            var employee = JsonSerializer.Deserialize<EmployeeRequest>(consumer.Message.Value);
            Console.WriteLine($"Processing Employee Name: {employee.Name}");
        }
    }
    catch (OperationCanceledException)
    {
        consumerBuilder.Close();
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.WriteLine("Hello, World!");