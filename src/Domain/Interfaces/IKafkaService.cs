namespace Domain.Interfaces
{
    public interface IKafkaService
    {
        Task ProduceMessageAsync(string topic, string message);
    }
}
