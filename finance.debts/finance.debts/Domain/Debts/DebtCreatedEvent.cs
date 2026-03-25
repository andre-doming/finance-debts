namespace finance.debts.Domain;

public class DebtCreatedEvent
{
    public int DebtId { get; set; }
    public int ClientId { get; set; }
    public decimal AmountDue { get; set; }
    public int StatusId { get; set; }
    public DateTime CreatedAt { get; set; }
}