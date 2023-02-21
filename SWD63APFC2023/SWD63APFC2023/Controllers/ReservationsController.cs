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
        public ReservationsController(FirestoreReservationsRepository _frr, FirestoreBooksRepository _fbr)
        {
            frr = _frr;
            fbr = _fbr;
        }

        public IActionResult Create(string isbn)
        {
            ViewBag.Isbn = isbn;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Reservation r)
        {
            await frr.AddReservation(r, fbr);
            return View();
        }
    }
}
