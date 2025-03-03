using System.Text;
using BE.src.api.domains.eventbus;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace BE.src.api.domains.eventbus.Producers
{
    public interface IEventBusRabbitMQProducer
    {
        void Publish<T>(string queueName, T eventMessage);
    }
    public class EventBusRabbitMQProducer : IEventBusRabbitMQProducer
    {
        private readonly IRabbitMQConnection _connection;

        public EventBusRabbitMQProducer(IRabbitMQConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public void Publish<T>(string queueName, T eventMessage)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            
            var message = JsonConvert.SerializeObject(eventMessage);
            var body = Encoding.UTF8.GetBytes(message);

            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;

            channel.ConfirmSelect();
            channel.BasicPublish(exchange: "", routingKey: queueName, mandatory: true, basicProperties: properties, body: body);
            channel.WaitForConfirmsOrDie();

            channel.BasicAcks += (sender, eventArgs) =>
            {
                Console.WriteLine("Sent RabbitMQ");
            };
            channel.ConfirmSelect();
        }
    }
}