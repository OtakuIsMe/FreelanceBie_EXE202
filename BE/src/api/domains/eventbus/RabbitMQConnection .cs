using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace BE.src.api.domains.eventbus
{
    public interface IRabbitMQConnection
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();
    }
	public class RabbitMQConnection : IRabbitMQConnection
	{
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private bool _disposed;
        public RabbitMQConnection (IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ApplicationException(nameof(connectionFactory));
            if(!IsConnected)
            {
                TryConnect();
            }
        }   
		public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }

        public bool TryConnect()
        {
            try
            {
                _connection = _connectionFactory.CreateConnection();
            }
            catch (BrokerUnreachableException)
            {
                Thread.Sleep(2000);
                _connection = _connectionFactory.CreateConnection();
            }

            if (IsConnected)
            {
                Console.WriteLine($"RabbitMQ persistent connection acquired a connection {_connection.Endpoint.HostName} and is subscribed to failure events");
                return true;
            }
            else
            {
                Console.WriteLine("FATAL ERROR: RabbitMQ connections could not be created and opened");
                return false;
            }
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new ApplicationException("No RabbitMQ connections are available to perform this action");
            }
            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
	}
}