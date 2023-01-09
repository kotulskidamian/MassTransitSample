using MassTransit;
using Microsoft.Extensions.Logging;
using Sample.Contracts;

namespace Sample.Components.Consuments
{
    public class SubmitOrderConsumer : IConsumer<ISubmitOrder>
    {
        private readonly ILogger<SubmitOrderConsumer> _logger;

        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ISubmitOrder> context)
        {
            _logger.Log(LogLevel.Debug, $"SubmitOrderCunsumer: {context.Message.CustomerNumber}");

            if (context.Message.CustomerNumber.Contains("TEST"))
            {
                await context.RespondAsync<IOrderSubmissionRejected>(new
                {
                    InVar.Timestamp,
                    context.Message.OrderId,
                    context.Message.CustomerNumber,
                    Reason = $"Test customer can't submit orders: {context.Message.CustomerNumber}"
                });

                return;
            }

            await context.RespondAsync<IOrderSubmissionAccepted>(new
            {
                InVar.Timestamp,
                context.Message.OrderId,
                context.Message.CustomerNumber
            });
        }
    }
}