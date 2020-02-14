using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
    public class BasketService: IBasketService
    {
        IRepository<Product> productContex;
        IRepository<Basket> basketContex;

        public const string BasketSessionName = "eCommerceBasket";

        public BasketService(IRepository<Product> ProductContex, IRepository<Basket> BasketContex)
        {
            this.basketContex = BasketContex;
            this.productContex = ProductContex;
        }

        private Basket GetBasket(HttpContextBase httpContex, bool CreateIfNull)
        {
            HttpCookie cookie = httpContex.Request.Cookies.Get(BasketSessionName);

            Basket basket = new Basket();
            if(cookie != null)
            {
                string basketId = cookie.Value;
                if (!string.IsNullOrEmpty(basketId))
                {
                    basket = basketContex.Find(basketId);
                }
                else
                {
                    if (CreateIfNull)
                    {
                        basket = CreateNewBasket(httpContex);
                    }
                }
            }
            else { 
            if (CreateIfNull)
            {
                basket = CreateNewBasket(httpContex);
            }
            }
            return basket;
        }

        private Basket CreateNewBasket(HttpContextBase httpContext)
        {
            Basket basket = new Basket();
            basketContex.Insert(basket);
            basketContex.Commit();

            HttpCookie cookie = new HttpCookie(BasketSessionName);
            cookie.Value = basket.Id;
            cookie.Expires = DateTime.Now.AddDays(1);
            httpContext.Response.Cookies.Add(cookie);

            return basket;
        }

        public void AddToBasket(HttpContextBase httpContext, string productId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);

            if (item==null)
            {
                item = new BasketItem()
                {
                    BasketID = basket.Id,
                    ProductId = productId,
                    Quantity = 1
                };

                basket.BasketItems.Add(item);
            }
            else
            {
                item.Quantity += 1;
            }

            basketContex.Commit();
        }
        public void RemoveFromBasket(HttpContextBase httpContext, string itemId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

            if (item !=null)
            {
                basket.BasketItems.Remove(item);
                basketContex.Commit();
            }
        }

        public List<BasketItemViewModel> GetBasketItems (HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);

            if (basket!=null)
            {
                var results = (from b in basket.BasketItems
                              join p in productContex.Collection() on b.ProductId equals p.Id
                              select new BasketItemViewModel()
                              {
                                  Id = b.Id,
                                  Quantity = b.Quantity,
                                  ProductName = p.Name,
                                  Image = p.Image,
                                  Price = p.Price
                              }
                                ).ToList();
                return results;
            }
            else
            {
                return new List<BasketItemViewModel>();
            }
                
        }

        public BasketSummaryViewModel GetBasketSummary(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);

            BasketSummaryViewModel model = new BasketSummaryViewModel(0, 0);

            if (basket != null)
            {
                int? basketCount = (from item in basket.BasketItems
                                    select item.Quantity).Sum();
                decimal? basketTotal = (from item in basket.BasketItems
                                        join p in productContex.Collection() on item.ProductId equals p.Id
                                        select item.Quantity * p.Price).Sum();
                model.BasketCount = basketCount ?? 0;
                model.BasketTotal = basketTotal ?? decimal.Zero;

                return model;
            }
            else
            {
                return model;
            }
        }

        public void ClearBasket(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);
            basket.BasketItems.Clear();
            basketContex.Commit();
        }
    }
}
