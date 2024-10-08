#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using DotLanches.Domain.Exceptions;

namespace DotLanches.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ClienteCpf { get; set; }
    public EStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public IEnumerable<Combo> Combos { get; set; }
    public int QueueKey { get; set; }
    public DateTime AddedToQueueAt { get; set; }
    public Pagamento Pagamento { get; set; }

    private Pedido() { }

    public Pedido(DateTime createdAt,
                  string? clienteCpf,
                  IEnumerable<Combo> combos)
    {
        Id = Guid.NewGuid();
        CreatedAt = createdAt;
        ClienteCpf = clienteCpf;
        Combos = combos;
        Status = EStatus.Confirmado;
        Pagamento = new Pagamento();

        ValidateEntity();
    }

    private void ValidateEntity()
    {
        if (Combos is null || !Combos.Any())
            throw new DomainValidationException(nameof(Combos));        
    }

    public void CalculateTotalPrice()
    {
        TotalPrice = Combos.Sum(c => c.CalculatePrice());

        if (TotalPrice <= 0)
            throw new DomainValidationException(nameof(TotalPrice));
    }

    public void ReceivePagamento(QueueKey queueKey)
    {
        this.Status = EStatus.Recebido;
        this.QueueKey = queueKey.Value;
        this.AddedToQueueAt = queueKey.CreationDate;
    }

    public void Cancel()
    {
        this.Status = EStatus.Cancelado;
    }

}