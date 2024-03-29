﻿using Google.Cloud.Diagnostics.AspNetCore3;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        IExceptionLogger excpetionLogger;
        ILogger<BooksController> logger;
        public BooksController(FirestoreBooksRepository _fbr, IExceptionLogger _exceptionLogger, ILogger<BooksController> _logger)
        {
            excpetionLogger = _exceptionLogger;
            fbr = _fbr;
            logger = _logger;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost][ValidateAntiForgeryToken()]
        public async Task< IActionResult> Create(Book b, IFormFile file, [FromServices] IConfiguration config)
        {
            try
            {
                logger.LogInformation($"{User.Identity.Name} is creating a new book with isbn {b.Isbn}");

                 
                if (file != null)
                {
                    logger.LogInformation($"{User.Identity.Name} is uploading a file called {file.FileName}");
                    string objectName = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);
                    logger.LogInformation($"{file.FileName} has been renamed {objectName}");
                    // ------------------------ start: adding actual file to cloud storage ----------------------------
                    string bucket = config["bucket"];
                    // string projectId = config["project"];

                    var storage = StorageClient.Create();
                    logger.LogInformation($"{file.FileName} with size {file.Length} will be uploaded in the next line");
                    using var fileStream = file.OpenReadStream();
                    storage.UploadObject(bucket, objectName, null, fileStream);
                  
                    b.Link = $"https://storage.googleapis.com/{bucket}/{objectName}"; 
                    logger.LogInformation($"{file.FileName} with {objectName} has been uploaded in {b.Link}");
                }
                // ------------------------ end: adding actual file to cloud storage ----------------------------

                //adding rest of info in firestore
                await fbr.AddBook(b);
                logger.LogInformation($"{b.Isbn} with physical file {b.Link} has been saved in db");
                TempData["success"] = "Book added successfully";
            }
            catch (Exception ex)
            {
                //logging : i will do this during the Error Reporting week
                excpetionLogger.Log(ex);
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

        public  IActionResult Delete(string isbn)
        {
            fbr.Delete(isbn);
            return RedirectToAction("Index");
        }
    }
}
