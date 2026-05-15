using ClickAndCollect.Filters;
using ClickAndCollect.Models;
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

        public async Task<IActionResult> Browse(int? categoryid) 
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role == "Cashier")
            {
                return RedirectToAction("ConsultTodayClientList", "Cashier");
            }
            if (role == "OrderMaker")
            {
                return RedirectToAction("Index", "OrderMaker");
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
    }
}
