using ClickAndCollect.Models;
using ClickAndCollect.Models.DALInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickAndCollect.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductDAL productDAL;

        public ProductController(IProductDAL dal) 
        {
            this.productDAL = dal;
        }

        public async Task<IActionResult> GetAllProducts() 
        {
            List<Product> products = await Product.GetAllProducts(productDAL);
            return View("DisplayProducts",products);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
