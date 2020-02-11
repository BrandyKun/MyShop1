using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class HomeController : Controller
    {
        
        
            IRepository<Product> context;
            IRepository<ProductCategory> productCategories;

            public HomeController(IRepository<Product> productContext, IRepository<ProductCategory> productCategoryContex)
            {
                context = productContext;
                productCategories = productCategoryContex;
            }
            public ActionResult Index()
            {
            List<Product> products = context.Collection().ToList();
                return View(products);
            }
            public ActionResult Details(string ID)
        {
            Product product = context.Find(ID);
            if (product==null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(product);
            }
        }
            public ActionResult About()
            {
                ViewBag.Message = "Your application description page.";

                return View();
            }

            public ActionResult Contact()
            {
                ViewBag.Message = "Your contact page.";

                return View();
            }
    }
}