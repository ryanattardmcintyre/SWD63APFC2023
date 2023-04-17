using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace pubsubFunction
{
    public class Function : ICloudEventFunction<MessagePublishedData>
    {
        private ILogger<Function> _logger;
        public Function(ILogger<Function> logger)
        =>
         _logger =logger;

        public Task HandleAsync(CloudEvent cloudEvent, MessagePublishedData data, CancellationToken cancellationToken)
        {
            var nameFromMessage = data.Message?.TextData;
            var name = string.IsNullOrEmpty(nameFromMessage) ? "world" : nameFromMessage;
            Console.WriteLine($"Hello {name}");
            _logger.LogInformation($"Hello {name}");

            //assuming that from the pubsub queue I get the document id
            FirestoreDb db = FirestoreDb.Create("swd63a2023-377009");
            DocumentReference docRef = db.Collection("books").Document(nameFromMessage);
            Dictionary<string, object> update = new Dictionary<string, object>
            {
                { "status", true }
            };
            var t = docRef.SetAsync(update, SetOptions.MergeAll);
            t.Wait();
            return Task.CompletedTask;
        }
    }
}