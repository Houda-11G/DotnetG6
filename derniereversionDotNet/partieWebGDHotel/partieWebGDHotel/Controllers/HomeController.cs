using Microsoft.AspNetCore.Mvc;

namespace partieWebGDHotel.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); 
        }
    }
}
