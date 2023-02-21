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

        public async Task AddBook(Book b)
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


        public async Task<Book> GetBook(string isbn)
        {
            Query allBooksQuery = db.Collection("books").WhereEqualTo("Isbn", isbn);
            QuerySnapshot allBooksQuerySnapshot = await allBooksQuery.GetSnapshotAsync();
          
            DocumentSnapshot documentSnapshot = allBooksQuerySnapshot.Documents.FirstOrDefault();
            if (documentSnapshot == null)
            {
                return null;
            }
            else
            {
                Book book = documentSnapshot.ConvertTo<Book>();
                return book;
            }
        }

        public async void Delete(string isbn)
        {
            Query allBooksQuery = db.Collection("books").WhereEqualTo("Isbn", isbn);
            QuerySnapshot allBooksQuerySnapshot = await allBooksQuery.GetSnapshotAsync();

            DocumentSnapshot documentSnapshot = allBooksQuerySnapshot.Documents.FirstOrDefault();
            if (documentSnapshot != null)
            {
                string id = documentSnapshot.Id;

                DocumentReference bookRef = db.Collection("books").Document(id);
                await bookRef.DeleteAsync();
            }
        }

        public async Task Update(Book b)
        {
            Query allBooksQuery = db.Collection("books").WhereEqualTo("Isbn", b.Isbn);
            QuerySnapshot allBooksQuerySnapshot = await allBooksQuery.GetSnapshotAsync();

            DocumentSnapshot documentSnapshot = allBooksQuerySnapshot.Documents.FirstOrDefault();
            if (documentSnapshot != null)
            {
                string id = documentSnapshot.Id;
                DocumentReference bookRef = db.Collection("books").Document(id);
                await bookRef.SetAsync(b);
            }
        }

        public async Task<string> GetBookDocumentId(string isbn)
        {
            Query allBooksQuery = db.Collection("books").WhereEqualTo("Isbn",isbn);
            QuerySnapshot allBooksQuerySnapshot = await allBooksQuery.GetSnapshotAsync();

            DocumentSnapshot documentSnapshot = allBooksQuerySnapshot.Documents.FirstOrDefault();
            return documentSnapshot.Id;
        }
    }
}
