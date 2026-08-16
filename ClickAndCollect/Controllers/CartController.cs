using ClickAndCollect.Filters;
using ClickAndCollect.Models.Classes;
using ClickAndCollect.Models.DALInterfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ClickAndCollect.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductDAL productDAL;

        public CartController(IProductDAL prodal)
        {
            this.productDAL = prodal;
        }

        public IActionResult Index()
        {
            return View();
        }

        [RoleFilter("Client")]
        public IActionResult Manage()
        {
            Cart c = HttpContext.Session.GetObject<Cart>("cart");
            List<CartLine> cl;

            if (c == null) 
            {
                c = new Cart();
                HttpContext.Session.SetObject("cart", c);
                return View("ManageCart",c);
            }

            return View("ManageCart",c);
        }

        [RoleFilter("Client")]
        public IActionResult RemoveProduct(int id)
        {
            Cart c = HttpContext.Session.GetObject<Cart>("cart");

            if (c == null) 
            {
                return RedirectToAction("Manage");
            }
                

            c.RemoveProduct(id);

            HttpContext.Session.SetObject("cart", c);

            return RedirectToAction("Manage");
        }

        [RoleFilter("Client")]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("cart");

            return RedirectToAction("Manage");
        }
    }
}
