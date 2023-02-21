﻿using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWD63APFC2023.Models
{
    [FirestoreData]
    public class Reservation
    {
        [FirestoreProperty]
        public string Email { get; set; }
        [FirestoreProperty]
        public string Isbn { get; set; }

        [FirestoreProperty]
        public Google.Cloud.Firestore.Timestamp From { get; set; }
        [FirestoreProperty]
        public Google.Cloud.Firestore.Timestamp To { get; set; }

    }
}
