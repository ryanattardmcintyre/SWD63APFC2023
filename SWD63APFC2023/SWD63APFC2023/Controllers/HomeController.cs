using Google.Cloud.Diagnostics.AspNetCore3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SWD63APFC2023.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SWD63APFC2023.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IExceptionLogger _exceptionLogger;
        public HomeController(ILogger<HomeController> logger, IExceptionLogger exceptionLogger)
        {
            _exceptionLogger = exceptionLogger;

            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy( )
        {
            try
            {
                throw new Exception("This is a testing exception");
            }
            catch (Exception ex)
            {
                _exceptionLogger.Log(ex);
            }


            return View();
        }


        [Authorize]
        public IActionResult MembersHome()
        {
            return View();
        }

        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
