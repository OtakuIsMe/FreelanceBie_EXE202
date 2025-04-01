using System.Text;
using BE.src.api.domains.DTOs.Post;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Queue = BE.src.api.shared.Constant.EventBus;

namespace BE.src.api.domains.eventbus.Consumers
{
    // public class PostNotificationConsumer : BackgroundService
    // {
    //     private readonly IRabbitMQConnection _connection;
    //     private readonly IServiceScopeFactory _serviceScopeFactory;
    //     private IModel _channel;

    //     public PostNotificationConsumer(IRabbitMQConnection connection, IServiceScopeFactory serviceScopeFactory)
    //     {
    //         _connection = connection;
    //         _serviceScopeFactory = serviceScopeFactory;
    //         _channel = _connection.CreateModel();
    //     }

    //     protected override Task ExecuteAsync(CancellationToken stoppingToken)
    //     {
    //         _channel.QueueDeclare(queue: Queue.PostNotificationQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            
    //         var consumer = new EventingBasicConsumer(_channel);
    //         consumer.Received += async (model, ea) =>
    //         {
    //             var body = ea.Body.ToArray();
    //             var message = Encoding.UTF8.GetString(body);
    //             var postEvent = JsonConvert.DeserializeObject<PostCreatedEvent>(message);

    //             Console.WriteLine($"[Consumer] Received event: {message}");

    //             using (var scope = _serviceScopeFactory.CreateScope())
    //             {
    //                 var notificationRepo = scope.ServiceProvider.GetRequiredService<INotificationRepo>();

    //                 var existingNotification = await notificationRepo.CountExistingNotification(postEvent.postId, postEvent.userId);
    //                 if(existingNotification) {
    //                     Console.WriteLine("[Consumer] Notification already exists, skipping...");
    //                     return;
    //                 }

    //                 var newNotification = new Notification
    //                 {
    //                     UserId = postEvent.userId,
    //                     PostId = postEvent.postId,
    //                     Message = $"A new post titled '{postEvent.Title}' has been published. <a href='http://localhost:5173/post/{postEvent.postId}'>Check it out!</a>",
    //                     CreateAt = postEvent.CreatedAt,
    //                     UpdateAt = DateTime.Now
    //                 };

    //                 await notificationRepo.AddNotification(newNotification);
    //             }

    //             _channel.BasicAck(ea.DeliveryTag, false);
    //         };

    //         _channel.BasicConsume(queue: Queue.PostNotificationQueue, autoAck: false, consumer: consumer);

    //         return Task.CompletedTask;
    //     }

    //     public override void Dispose()
    //     {
    //         _channel?.Dispose();
    //         base.Dispose();
    //     }
    // }
}