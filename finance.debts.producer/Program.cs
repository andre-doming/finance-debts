using MassTransit;
using finance.debts.producer;
using finance.debts.domain.Entities;
using finance.debts.producer.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var rabbitConfig = builder.Configuration.GetSection("RabbitMQ");
var rabbitHost = rabbitConfig["Host"] ?? throw new Exception("Rabbit host não configurado");
var rabbitVhost = rabbitConfig["VirtualHost"] ?? throw new Exception("Rabbit vhost não configurado");
var rabbitUser = rabbitConfig["Username"] ?? throw new Exception("Rabbit username não configurado"); ;
var rabbitPass = rabbitConfig["Password"] ?? throw new Exception("Rabbit password não configurado"); ;
var rabbitQueue = rabbitConfig["Queue"] ?? throw new Exception("Rabbit queue não configurado"); ;

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHost, rabbitVhost, h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });

        cfg.Message<DebtCreatedEvent>(e =>
        {
            e.SetEntityName(rabbitQueue);
        });
    });
});

var host = builder.Build();
host.Run();
