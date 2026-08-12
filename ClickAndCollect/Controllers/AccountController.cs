using ClickAndCollect.Models.Classes;
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

        public IActionResult Index() 
        {
            return View("Login");
        }

        public IActionResult Login()
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != null)
            {
                return role switch
                {
                    "Cashier" => RedirectToAction("ConsultTodayClientList", "Cashier"),
                    "OrderMaker" => RedirectToAction("CheckTomorrowOrders", "OrderMaker"),
                    "Client" => RedirectToAction("Browse", "Product"),
                    _ => View()
                };
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            int? userId = await Models.Classes.User.GetByUsername(username, userDAL);

            if (userId == null)
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

            bool validPassword = await Models.Classes.User.VerifyPassword(password, userId.Value, userDAL);

            if (!validPassword)
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

            User? user = await Models.Classes.User.GetUser(userId.Value, userDAL);

            if (user == null)
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

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
                "Client" => RedirectToAction("Browse", "Product"),
                "Cashier" => RedirectToAction("ConsultTodayClientList", "Cashier"),
                "OrderMaker" => RedirectToAction("CheckTomorrowOrders", "OrderMaker"),
                _ => RedirectToAction("Login")
            };
        }


        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string firstname, string lastname, string phonenumber, int postalcode, string cityname, string streetname, int housenumber)
        {
            int? userId = await ClickAndCollect.Models.Classes.User.GetByUsername(username, userDAL);

            if (userId != null)
            {
                ViewBag.Error = "Username already taken";
                return View();
            }

            List<string> errors = ClickAndCollect.Models.Classes.User.ValidateRegistrationData(password, firstname, lastname);

            if (errors.Count > 0)
            {
                ViewBag.Errors = errors;
                return View();
            }

            bool success = await ClickAndCollect.Models.Classes.User.CreateAccount(
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