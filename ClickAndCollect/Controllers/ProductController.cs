
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

            if (p == null) 
            {
                return NotFound();
            }

            Cart c = HttpContext.Session.GetObject<Cart>("cart") ?? new Cart();
            c.AddProduct(p);
            HttpContext.Session.SetObject("cart", c);

            return categoryid.HasValue ? RedirectToAction("Browse", new { categoryid }) : RedirectToAction("Browse");
        }
    }
}
