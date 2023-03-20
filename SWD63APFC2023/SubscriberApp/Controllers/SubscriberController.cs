using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using System.Threading;
using SWD63APFC2023.Models;
using Newtonsoft.Json;

namespace SubscriberApp.Controllers
{
    public class SubscriberController : Controller
    {
        public async Task<int> Index()
        {
            string projectId = "swd63a2023-377009";
            string subscriptionId = "messages-sub";
            bool acknowledge = false;

            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);

            // SubscriberClient runs your message handle function on multiple
            // threads to maximize throughput.
            int messageCount = 0;

            List<string> messages = new List<string>();

            Task startTask = subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
            {
                string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());

                System.Diagnostics.Debug.WriteLine($"Message {message.MessageId}: {text}");
                Console.WriteLine($"Message {message.MessageId}: {text}");
                messages.Add($"{message.MessageId}:{text}");

                Interlocked.Increment(ref messageCount);
                //if(acknowledge == true) return 1 else return 0;

                return Task.FromResult(acknowledge ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack);
            });
            // Run for 5 seconds.
            await Task.Delay(5000);
            await subscriber.StopAsync(CancellationToken.None);
            // Lets make sure that the start task finished successfully after the call to stop.
            await startTask;

            //will send email here
            List<string> uniquemessages = messages.Distinct().ToList();

            //sending an email
            foreach (var msg in uniquemessages)
            {
                Reservation r = JsonConvert.DeserializeObject<Reservation>(msg.Split(new char[] { ':' })[1]);

                //code to send email using MailGun

            }


            return messageCount;
        }
    }
}
