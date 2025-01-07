using Microsoft.AspNetCore.Mvc;

namespace partieWebGDHotel.Controllers
{
    public class AcceuilController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
