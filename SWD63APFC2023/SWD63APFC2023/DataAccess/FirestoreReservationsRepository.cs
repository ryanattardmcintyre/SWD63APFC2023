using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using SWD63APFC2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWD63APFC2023.DataAccess
{
    public class FirestoreReservationsRepository
    {
        FirestoreDb db;
        public FirestoreReservationsRepository(string project)
        {
            db = FirestoreDb.Create(project);
        }

        public async Task AddReservation(Reservation r, [FromServices]FirestoreBooksRepository booksRepo)
        {
            var docIdOfTheBookBeingReserved = booksRepo.GetBookDocumentId(r.Isbn);

            await db.Collection($"books/{docIdOfTheBookBeingReserved}/reservations").Document().SetAsync(r);
        }
    }
}
