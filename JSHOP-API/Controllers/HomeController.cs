using Microsoft.AspNetCore.Mvc;

namespace JSHOP_API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "API JANIER";
            return Content("API - STOK JANIER SHOP");
        }
    }
}
