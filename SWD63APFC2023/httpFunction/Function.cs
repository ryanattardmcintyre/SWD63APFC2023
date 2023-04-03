using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using System.Linq;

namespace httpFunction;

public class Function : IHttpFunction
{
    /// <summary>
    /// Logic for your function goes here.
    /// </summary>
    /// <param name="context">The HTTP context, containing the request and the response.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task HandleAsync(HttpContext context)
    {

   // System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
        //   @"swd63a2023-377009-5caf98b90066.json");

        HttpRequest request = context.Request;
        string message = request.Query["message"]; //assuming that in message you received your data

        Book b  = await GetBook(message);
        await context.Response.WriteAsync($"<div>Name {b.Name} <br/> Author {b.Author} <br/> <a href=\"{b.Link}\">Download</a></div>");
    }

        private async Task<Book> GetBook(string isbn)
        {
            var db = FirestoreDb.Create("swd63a2023-377009");
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

}
