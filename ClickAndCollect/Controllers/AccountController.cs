using ClickAndCollect.Models;
using ClickAndCollect.Models.DALInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickAndCollect.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserDAL userDAL;

        public AccountController(IUserDAL dal)
        {
            this.userDAL = dal;
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            int? userId = await ClickAndCollect.Models.User.GetByUsername(username, userDAL);

            if (userId == null)
            {
                ViewBag.Error = "Username doesn't exist";
                return View();
            }

            bool validPassword = await ClickAndCollect.Models.User.VerifyPassword(password, userId.Value, userDAL);

            if (!validPassword)
            {
                ViewBag.Error = "Incorrect password";
                return View();
            }

            User? user = await ClickAndCollect.Models.User.GetUser(userId.Value, userDAL);

            string role = user.GetRole();

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", role);
            HttpContext.Session.SetString("Username", user.Username);

            return role switch
            {
                "Client" => RedirectToAction("GetAllProducts", "Product"),
                "Cashier" => RedirectToAction("Index", "Cashier"),
                "OrderMaker" => RedirectToAction("Index", "OrderMaker"),
                _ => RedirectToAction("Login")
            };
        }
    }
}
