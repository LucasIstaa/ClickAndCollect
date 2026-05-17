using ClickAndCollect.Models.Classes;
using ClickAndCollect.Models.DALInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickAndCollect.Controllers
{
    public class StoreController : Controller
    {
        private readonly IStoreDAL storeDAL;

        public StoreController(IStoreDAL storeDAL) 
        {
            this.storeDAL = storeDAL;
        }

        public async Task<IActionResult> AddStore(Store s)
        {
            bool success = await Store.AddStoreAsync(s, storeDAL);

            if (success)
                return RedirectToAction("Index");

            return BadRequest("Impossible d'ajouter le store");
        }


        public IActionResult Index()
        {
            return View("AddStore");
        }
    }
}
