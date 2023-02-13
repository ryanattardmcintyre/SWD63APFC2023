using Google.Cloud.Firestore;
using SWD63APFC2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWD63APFC2023.DataAccess
{
    public class FirestoreBooksRepository
    {
        FirestoreDb db;

        public FirestoreBooksRepository(string project)
        {
             db = FirestoreDb.Create(project);
        }

        public async void AddBook(Book b)
        {
                await db.Collection("books").Document().SetAsync(b);
        }

        public async Task<List<Book>> GetBooks()
        {
            List<Book> books = new List<Book>();

            Query allBooksQuery = db.Collection("books");
            QuerySnapshot allBooksQuerySnapshot = await allBooksQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in allBooksQuerySnapshot.Documents)
            {
                Book book = documentSnapshot.ConvertTo<Book>();
                books.Add(book);
            }
            return books;
        }
    }
}
