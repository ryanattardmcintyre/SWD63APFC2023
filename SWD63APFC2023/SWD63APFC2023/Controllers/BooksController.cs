using Microsoft.AspNetCore.Mvc;
using SWD63APFC2023.DataAccess;
using SWD63APFC2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWD63APFC2023.Controllers
{
    public class BooksController : Controller
    {
        FirestoreBooksRepository fbr;
        public BooksController(FirestoreBooksRepository _fbr)
        {
            fbr = _fbr;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(Book b)
        {
            try
            {
                fbr.AddBook(b);
                TempData["success"] = "Book added successfully";
            }
            catch (Exception ex)
            {
                //logging : i will do this during the Error Reporting week

                TempData["error"] = "error occurred. check your inputs, try again later";

            }
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var list =  await( fbr.GetBooks());
            return View(list);
        }

        //public IActionResult Update(string isbn)
        //{
        //    FirestoreBooksRepository fbr = new FirestoreBooksRepository();
        //    return View(); }
        //public IActionResult Update(Book b)
        //{
        //    FirestoreBooksRepository fbr = new FirestoreBooksRepository();
        //    return View();

        //}
    }
}
