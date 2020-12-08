using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Stateless;

namespace FakeXiecheng.API.Models
{
    public class Order
    {
        private StateMachine<OrderStateEnum, OrderStateTriggerEnum> _machine;

        public Order()
        {
            StateMachineInit();
        }

        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public ICollection<LineItem> OrderItems { get; set; }

        public OrderStateEnum State { get; set; }

        public DateTime CreateDateUTC { get; set; }

        public string TransactionMetadata { get; set; }

        private void StateMachineInit()
        {
            _machine = new StateMachine<OrderStateEnum, OrderStateTriggerEnum>(OrderStateEnum.Pending);

            _machine.Configure(OrderStateEnum.Pending)
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Cancel, OrderStateEnum.Cancelled);

            _machine.Configure(OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Approve, OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Reject, OrderStateEnum.Declined);

            _machine.Configure(OrderStateEnum.Declined)
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing);

            _machine.Configure(OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Return, OrderStateEnum.Refund);
        }
    }

    public enum OrderStateEnum
    {
        Pending,
        Processing,
        Completed,
        Declined,
        Cancelled,
        Refund
    }

    public enum OrderStateTriggerEnum
    {
        PlaceOrder,
        Approve,
        Reject,
        Cancel,
        Return
    }
}