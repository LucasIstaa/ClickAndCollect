using ClickAndCollect.Models.Classes;
using ClickAndCollect.Models.DALInterfaces;
using ClickAndCollect.Models.ViewModels;
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
            return View("Login", new LoginViewModel());
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
                    _ => View(new LoginViewModel())
                };
            }
            return View(new LoginViewModel());
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
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int? userId = await Models.Classes.User.GetByUsername(model.Username, userDAL);

            if (userId == null)
            {
                ViewBag.Error = "Invalid username or password";
                return View(model);
            }

            bool validPassword = await Models.Classes.User.VerifyPassword(model.Password, userId.Value, userDAL);

            if (!validPassword)
            {
                ViewBag.Error = "Invalid username or password";
                return View(model);
            }

            User? user = await Models.Classes.User.GetUser(userId.Value, userDAL);

            if (user == null)
            {
                ViewBag.Error = "Invalid username or password";
                return View(model);
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
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int? userId = await ClickAndCollect.Models.Classes.User.GetByUsername(model.Username, userDAL);

            if (userId != null)
            {
                ViewBag.Error = "Username already taken";
                return View(model);
            }

            List<string> errors = ClickAndCollect.Models.Classes.User.ValidateRegistrationData(model.Password, model.Firstname, model.Lastname);

            if (errors.Count > 0)
            {
                ViewBag.Errors = errors;
                return View(model);
            }

            bool success = await ClickAndCollect.Models.Classes.User.CreateAccount(
                model.Username, model.Password, model.Firstname, model.Lastname, model.Phonenumber,
                model.Postalcode, model.Cityname, model.Streetname, model.Housenumber, userDAL);

            if (success)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Error = "An error occurred while creating the account.";
            return View(model);
        }
    }
}