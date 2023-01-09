namespace Sample.Contracts
{
    public interface IOrderSubmissionRejected
    {
        public Guid OrderId { get; }
        public DateTime Timestamp { get; }
        public string CustomerNumber { get; }
        public string Reason { get; }
    }
}
