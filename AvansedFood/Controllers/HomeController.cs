using System.Diagnostics;
using AvansedFood.Models;
using Microsoft.AspNetCore.Mvc;

namespace AvansedFood.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

}
