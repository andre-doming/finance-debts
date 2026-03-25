using finance.debts.Domain;
using finance.Infrastructure;
using MassTransit;

namespace finance.debts
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
                var debt = new Domain.Debt
                {
                    ClientId = random.Next(1, 1000),
                    AmountDue = Math.Round((decimal)(random.NextDouble() * 2500), 2),
                    StatusId = 1,
                    CreatedAt = DateTime.UtcNow
                };

                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                context.Debts.Add(debt);
                await context.SaveChangesAsync(stoppingToken);

                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>(); //descomentar

                //var sendEndpointProvider = scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>(); //remover 

                //var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:finance.debts.queue")); //remover 


                await publishEndpoint.Publish(new DebtCreatedEvent //await endpoint.Send(new DebtCreatedEvent  //descomentar
                {
                    DebtId = debt.DebtId,
                    ClientId = debt.ClientId,
                    AmountDue = debt.AmountDue,
                    StatusId = debt.StatusId,
                    CreatedAt = debt.CreatedAt
                });

                _logger.LogInformation("Saved Debt to DB: ClientId={ClientId}, Amount={Amount}",
                    debt.ClientId, debt.AmountDue);

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
