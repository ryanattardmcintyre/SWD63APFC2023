﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task< IActionResult> Create(Book b)
        {
            try
            {
                await fbr.AddBook(b);
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

        [HttpGet] //will be called when the user first clicks on the Edit link
        public async Task<IActionResult> GetBook(string isbn)
        {
            var b= await fbr.GetBook(isbn);
            return View("Update",b);
        }
        [HttpPost] //will be called when the user types in the data and submits the form
        public async Task<IActionResult> Update(Book b)
        {
            try
            {
                await fbr.Update(b);
                TempData["success"] = "updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = "error occurred. check your inputs, try again later";
            }

            return View(b);

        }
    }
}
