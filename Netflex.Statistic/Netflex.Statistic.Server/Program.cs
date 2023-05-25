using Kafka.Consumer.Configuration;
using Netflex.Statistic.Server.Kafka;
using Netflex.Statistic.Server.RabitMQProducer;
using Kafka.Consumer.Consumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterMongoDB(builder.Configuration);
builder.Services.AddSingleton<IRabitMQProducer, RabitMQProducer>();
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddSingleton<IHostedService, KafkaConsumer>();
builder.Services.AddCors();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
