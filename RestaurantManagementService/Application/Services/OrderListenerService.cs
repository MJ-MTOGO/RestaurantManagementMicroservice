using Google.Cloud.PubSub.V1;
using Newtonsoft.Json;
using RestaurantManagementService.Application.DTOs;
using RestaurantManagementService.Infrastructure.WebSocketManagement;

namespace RestaurantManagementService.Application.Services
{
    public class OrderListenerService
    {
        private readonly RestaurantWebSocketManager _webSocketManager;
        private readonly HttpClient _httpClient;
        private readonly string _subscriptionId;
        private readonly string _projectId;

        public OrderListenerService(
            RestaurantWebSocketManager webSocketManager,
            HttpClient httpClient,
            string projectId,
            string subscriptionId)
        {
            _webSocketManager = webSocketManager;
            _httpClient = httpClient;
            _projectId = projectId;
            _subscriptionId = subscriptionId;
        }

        public async Task StartListeningAsync()
        {
            var subscriber = await SubscriberClient.CreateAsync(
                SubscriptionName.FromProjectSubscription(_projectId, _subscriptionId));

            await subscriber.StartAsync(async (PubsubMessage message, CancellationToken cancellationToken) =>
            {
                try
                {
                    var pubSubData = JsonConvert.DeserializeObject<PubSubMessage>(message.Data.ToStringUtf8());
                    if (pubSubData?.OrderId == null)
                    {
                        Console.WriteLine("Invalid Pub/Sub message: OrderId is null.");
                        return SubscriberClient.Reply.Ack;
                    }
                    Console.WriteLine("step 1. Order ID: "+pubSubData.OrderId);
                    var orderEndpoint = $"http://localhost:5194/api/orders/{pubSubData.OrderId}";
                    var httpResponse = await _httpClient.GetAsync(orderEndpoint, cancellationToken);

                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to fetch order details for OrderId: {pubSubData.OrderId}");
                        return SubscriberClient.Reply.Nack;
                    }

                    var orderJson = await httpResponse.Content.ReadAsStringAsync();
                    await _webSocketManager.BroadcastMessageAsync(orderJson);
                    Console.WriteLine("step 2. sendte Data : " + orderJson);
                    return SubscriberClient.Reply.Ack;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    return SubscriberClient.Reply.Nack;
                }
            });
        }
    }
}
