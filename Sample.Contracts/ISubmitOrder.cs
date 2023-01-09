namespace Sample.Contracts
{
    public interface ISubmitOrder
    {
        public Guid OrderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string CustomerNumber { get; set; }
    }
}