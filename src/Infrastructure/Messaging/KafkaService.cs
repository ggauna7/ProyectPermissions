using Confluent.Kafka;
using Domain.Interfaces;

namespace Infrastructure.Messaging
{
    public class KafkaService : IKafkaService
    {
        private readonly IProducer<string, string> _producer;

        public KafkaService(IProducer<string, string> producer)
        {
            _producer = producer;
        }

        // Método para enviar un mensaje a un tópico de Kafka
        public async Task ProduceMessageAsync(string topic, string message)
        {
            try
            {
                // Creamos un mensaje de prueba
                var msg = new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = message
                };
                // Enviar el mensaje al tema de Kafka
                var result = await _producer.ProduceAsync(topic, msg);

            }
            catch (ProduceException<string, string> e)
            {
                Console.WriteLine($"Error al enviar el mensaje a Kafka: {e.Message}");
            }
        }
    }
}
