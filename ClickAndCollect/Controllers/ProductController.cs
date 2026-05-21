
﻿using ClickAndCollect.Models.Classes;
using ClickAndCollect.Filters;
using ClickAndCollect.Models.DALInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickAndCollect.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductDAL productDAL;
        private readonly ICategoryDAL categoryDAL;

        public ProductController(IProductDAL prodal, ICategoryDAL catdal) 
        {
            this.productDAL = prodal;
            this.categoryDAL = catdal;
        }

        [RoleFilter("Client")]
        public async Task<IActionResult> Browse(int? categoryid) 
        {
            ViewBag.SelectedCategory = categoryid;

            string? role = HttpContext.Session.GetString("Role");
            if (role == "Cashier")
            {
                return RedirectToAction("ConsultTodayClientList", "Cashier");
            }
            if (role == "OrderMaker")
            {
                return RedirectToAction("CheckTomorrowOrders", "OrderMaker");
            }

            List<Product> products = new List<Product>();

            if (categoryid.HasValue)
            {
                products = await Product.GetProductsByCategory(productDAL, categoryid);
            }
            else 
            {
                products = await Product.GetAllProducts(productDAL);
            }

            ViewBag.Categories = await Category.GetAllCategories(categoryDAL);

            return View("DisplayProducts",products);
        }


        public IActionResult Index()
        {
            return View();
        }

        [RoleFilter("Client")]
        public async Task<IActionResult> AddProductToCart(int id, int? categoryid)
        {
            Product p = await Product.GetProductAsync(productDAL, id);

            Cart c = HttpContext.Session.GetObject<Cart>("cart");

            if (c == null)
            {
                c = new Cart();
                CartLine cl = new CartLine(1, p);
                c.AddCartline(cl);
            }
            else
            {
                bool ok = false;

                foreach (CartLine cl in c.Lines)
                {
                    if (cl.Product.Equals(p))
                    {
                        cl.Quantity = cl.Quantity + 1;
                        ok = true;
                        break;
                    }
                }

                if (ok == false)
                {
                    CartLine cl = new CartLine(1, p);
                    c.AddCartline(cl);
                }
            }

            HttpContext.Session.SetObject("cart", c);



            if (categoryid.HasValue) 
            {
                return RedirectToAction("Browse", new { categoryid });
            }
                
            return RedirectToAction("Browse");
        }
    }
}
