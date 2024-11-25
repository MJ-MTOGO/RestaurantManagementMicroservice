using Microsoft.EntityFrameworkCore;
using RestaurantManagementService.Application.Ports;
using RestaurantManagementService.Application.Services;
using RestaurantManagementService.Infrastructure.Adapters.Repositories;
using RestaurantManagementService.Infrastructure.Publishers;
using RestaurantManagementService.Infrastructure;
using RestaurantManagementService.Infrastructure.WebSocketManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Database Context with connection string
builder.Services.AddDbContext<RestaurantManagementServiceDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

// Register IMessageBus with GooglePubSubMessageBus
builder.Services.AddSingleton<IMessageBus, GooglePubSubMessageBus>(sp =>
    new GooglePubSubMessageBus(builder.Configuration["GoogleCloud:ProjectId"]));

// Register IMessagePublisher with MessagePublisher
builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();

// Register WebSocketManager
builder.Services.AddSingleton<RestaurantWebSocketManager>();

// Register HttpClient for OrderListenerService
builder.Services.AddHttpClient();

// Register OrderListenerService with configuration values
builder.Services.AddSingleton<OrderListenerService>(sp =>
{
    var webSocketManager = sp.GetRequiredService<RestaurantWebSocketManager>();
    var httpClient = sp.GetRequiredService<HttpClient>();
    var projectId = builder.Configuration["GoogleCloud:ProjectId"];
    var subscriptionId = builder.Configuration["GoogleCloud:SubscriptionId"];
    return new OrderListenerService(webSocketManager, httpClient, projectId, subscriptionId);
});

// Register Repository
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IDatabaseRepository, RestaurantRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add CORS (optional)
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

app.UseAuthorization();

// Add WebSocket Middleware
app.UseWebSockets();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var webSocketManager = context.RequestServices.GetRequiredService<RestaurantWebSocketManager>();
            await webSocketManager.HandleWebSocketConnectionAsync(webSocket);
        }
        else
        {
            context.Response.StatusCode = 400; // Bad Request
        }
    }
    else
    {
        await next();
    }
});
app.MapControllers();

// Start Pub/Sub Listener Service
var orderListenerService = app.Services.GetRequiredService<OrderListenerService>();
_ = orderListenerService.StartListeningAsync(); // Fire-and-forget task

app.Run();
