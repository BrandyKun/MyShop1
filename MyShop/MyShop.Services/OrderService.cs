using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services
{
    public class OrderService :IOrderService
    {
        IRepository<Order> orderContex;
        public OrderService(IRepository<Order> OrderContex)
        {
            this.orderContex = OrderContex;
        }

        void IOrderService.CreateOrder(Order baseOrder, List<BasketItemViewModel> basketItems)
        {
            foreach (var item in basketItems)
            {
                baseOrder.OrderItems.Add(new OrderItem()
                {
                    ProductId = item.Id,
                    Image=item.Image,
                    Price=item.Price,
                    ProductName=item.ProductName,
                    Quantity=item.Quantity
                });
                orderContex.Insert(baseOrder);
                orderContex.Commit();
            }
        }
    }
}
