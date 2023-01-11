namespace Sample.Contracts
{
    public interface IOrderSubmitted
    {
        public Guid OrderId { get; }
        public DateTime Timestamp { get; }
        public string CustomerNumber { get; }
    }
}
