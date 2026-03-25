using finance.debts;
using finance.debts.Domain;
using finance.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "finance", h =>
        {
            h.Username("svc-debts");
            h.Password("SvcDebts!7pQ3a");
        });

        // 👇 AQUI FUNCIONA EM TODAS VERSÕES
        cfg.Message<DebtCreatedEvent>(e =>
        {
            e.SetEntityName("finance.debts.created");
        });
    });
});

var host = builder.Build();
host.Run();
