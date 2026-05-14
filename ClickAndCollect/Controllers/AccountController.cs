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

        public IActionResult Register()
        {
            return View();
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
            if (user is Cashier cashier)
            {
                HttpContext.Session.SetInt32("StoreId", cashier.Store.StoreId);
            }
            else if (user is OrderMaker maker)
            {
                HttpContext.Session.SetInt32("StoreId", maker.Store.StoreId);
            }

            return role switch
            {
                "Client" => RedirectToAction("GetAllProducts", "Product"),
                "Cashier" => RedirectToAction("Index", "Cashier"),
                "OrderMaker" => RedirectToAction("Index", "OrderMaker"),
                _ => RedirectToAction("Login")
            };
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string firstname, string lastname, string phonenumber, int postalcode, string cityname, string streetname, int housenumber)
        {
            int? userId = await ClickAndCollect.Models.User.GetByUsername(username, userDAL);

            if (userId != null)
            {
                ViewBag.Error = "Username already taken";
                return View();
            }

            List<string> errors = new List<string>();

            if (password.Length < 8 || password.Length > 32)
                errors.Add("Password must be between 8 and 32 characters.");
            if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
                errors.Add("Password must contain at least one letter and one number.");
            if (firstname.Length < 3 || firstname.Length > 255 || firstname.Any(char.IsDigit))
                errors.Add("First name must be 3-255 characters with no digits.");
            if (lastname.Length < 3 || lastname.Length > 255 || lastname.Any(char.IsDigit))
                errors.Add("Last name must be 3-255 characters with no digits.");

            if (errors.Count > 0)
            {
                ViewBag.Errors = errors;
                return View();
            }

            bool success = await ClickAndCollect.Models.User.CreateAccount(
                username, password, firstname, lastname, phonenumber,
                postalcode, cityname, streetname, housenumber, userDAL);

            if (success)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Error = "An error occurred while creating the account.";
            return View();
        }
    }
}
