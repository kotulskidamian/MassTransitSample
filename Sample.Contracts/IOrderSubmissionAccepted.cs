namespace Sample.Contracts
{
    public interface IOrderSubmissionAccepted
    {
        public Guid OrderId { get; }
        public DateTime Timestamp { get; }
        public string CustomerNumber { get; }
    }
}
