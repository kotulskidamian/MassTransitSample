using Automatonymous;
using MassTransit;
using MassTransit.RedisIntegration;
using MassTransit.Saga;
using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Components.StateMachines
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderStatusRequested, x =>
            {
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {
                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync<IOrderNotFound>(new { context.Message.OrderId });
                    }
                }));
            });

            InstanceState(x => x.CurrentState);

            Initially(
                When(OrderSubmitted)
                .Then(context =>
                {
                    context.Instance.SubmitDate = context.Data.Timestamp;
                    context.Instance.CustomerNumber = context.Data.CustomerNumber;
                    context.Instance.Updated = DateTime.UtcNow;
                })
                .TransitionTo(Submitted)
                );

            During(Submitted,
                Ignore(OrderSubmitted));

            DuringAny(
                When(OrderStatusRequested)
                .RespondAsync(x => x.Init<IOrderStatus>(new
                {
                    OrderId = x.Instance.CorrelationId,
                    State = x.Instance.CurrentState
                }))
            );

            DuringAny(
                When(OrderSubmitted)
                .Then(context =>
                {
                    context.Instance.SubmitDate ??= context.Data.Timestamp;
                    context.Instance.CustomerNumber ??= context.Data.CustomerNumber;
                })
            );
        }

        public State Submitted { get; private set; }

        public Event<IOrderSubmitted> OrderSubmitted { get; private set; }
        public Event<ICheckOrder> OrderStatusRequested { get; private set; }
    }

    public class OrderState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }

        public string CurrentState { get; set; }

        public string CustomerNumber { get; set; }

        public DateTime? Updated { get; set; }
        public DateTime? SubmitDate { get; set; }
    }
}
