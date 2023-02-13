using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWD63APFC2023.Models
{
    [FirestoreData]
    public class Book
    {
        [FirestoreProperty]
        public string Isbn { get; set; }
        
        [FirestoreProperty]
        public string Name { get; set; }
        
        [FirestoreProperty]
        public string Author { get; set; }
        
        [FirestoreProperty]
        public int Year { get; set; }

    }
}
