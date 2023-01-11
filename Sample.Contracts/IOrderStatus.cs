namespace Sample.Contracts
{
    public interface IOrderStatus
    {
        public Guid OrderId { get; }
        public string State { get; }
    }
}
