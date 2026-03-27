using finance.debts.domain.Entities;
using finance.debts.producer.Infrastructure;
using finance.debts.Contracts;
using MassTransit;

namespace finance.debts.producer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var random = new Random();

            while (!stoppingToken.IsCancellationRequested)
            {
                var correlationId = Guid.NewGuid();

                var debt = new Debt(
                    debtId: 0,
                    clientId: random.Next(1, 1000),
                    amountDue: Math.Round((decimal)random.NextDouble() * 2500, 2),
                    correlationId: correlationId
                );

                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                context.Debts.Add(debt);
                await context.SaveChangesAsync(stoppingToken);

                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await publishEndpoint.Publish<IDebtCreated>(new
                {
                    DebtId = debt.DebtId,
                    Amount = debt.AmountDue,
                    Document = debt.ClientId.ToString(),
                    CreatedAt = DateTime.UtcNow
                },
                ctx =>
                {
                    ctx.CorrelationId = correlationId;
                });

                _logger.LogInformation("Evento publicado | DebtId={DebtId}", debt.DebtId);

                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}