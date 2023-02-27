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
            var docIdOfTheBookBeingReserved = await booksRepo.GetBookDocumentId(r.Isbn);

            await db.Collection($"books/{docIdOfTheBookBeingReserved}/reservations").Document().SetAsync(r);
        }

        public async Task<List<Reservation>> GetReservations([FromServices] FirestoreBooksRepository booksRepo)
        {
            List<Reservation> reservations = new List<Reservation>();

            var listOfBooks = await booksRepo.GetBooks();
            foreach(Book b in listOfBooks)
            {
                var bookId = await booksRepo.GetBookDocumentId(b.Isbn);
                Query allReservationsQuery = db.Collection($"books/{bookId}/reservations");
                QuerySnapshot allReservationsQuerySnapshot = await allReservationsQuery.GetSnapshotAsync();
                if (allReservationsQuerySnapshot.Documents.Count > 0)
                {
                    foreach (DocumentSnapshot documentSnapshot in allReservationsQuerySnapshot.Documents)
                    {
                        Reservation reservation = documentSnapshot.ConvertTo<Reservation>();
                        reservations.Add(reservation);
                    }
                }
            }
            return reservations; 
        }
    }
}
