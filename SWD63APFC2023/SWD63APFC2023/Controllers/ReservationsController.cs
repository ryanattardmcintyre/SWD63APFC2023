using Microsoft.AspNetCore.Mvc;
using SWD63APFC2023.DataAccess;
using SWD63APFC2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWD63APFC2023.Controllers
{
    public class ReservationsController : Controller
    {

        FirestoreReservationsRepository frr;
        FirestoreBooksRepository fbr;
        PubsubEmailsRepository pser;
        public ReservationsController(FirestoreReservationsRepository _frr, 
            FirestoreBooksRepository _fbr, PubsubEmailsRepository _pser)
        {
            frr = _frr;
            fbr = _fbr;
            pser = _pser;
        }

        [HttpGet]
        public IActionResult Create(string isbn)
        {
            ViewBag.Isbn = isbn;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Reservation r, DateTime from, DateTime to)
        {
            try
            {
                r.From = Google.Cloud.Firestore.Timestamp.FromDateTime(from.ToUniversalTime());
                r.To = Google.Cloud.Firestore.Timestamp.FromDateTime(to.ToUniversalTime());

                await frr.AddReservation(r, fbr);
                string msgId= await pser.PushMessage(r);

                TempData["success"] = "Reservation added";
            }catch (Exception ex
            )
            {
                TempData["error"] = "Reservation adding failed";
            }
            return View();
        }


        public async Task<IActionResult> Index()
        {
            var list = await frr.GetReservations(fbr);
            return View(list);
        }
    }
}
