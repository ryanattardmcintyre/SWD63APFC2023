using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using System.Threading;
using Common.Models;
using Newtonsoft.Json;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using RestSharp;
using RestSharp.Authenticators;

namespace SubscriberApp.Controllers
{
    public class SubscriberController : Controller
    {
        public async Task<int> Index()
        {
            string projectId = "swd63a2023-377009";
            string subscriptionId = "messages-sub";
            bool acknowledge = true;

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
                messages.Add($"{text}");

                Interlocked.Increment(ref messageCount);
                //if(acknowledge == true) return 1 else return 0;

                return Task.FromResult(acknowledge ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack);
            });
            // Run for 5 seconds.
            await Task.Delay(5000);
            await subscriber.StopAsync(CancellationToken.None);
            // Lets make sure that the start task finished successfully after the call to stop.
            await startTask;

            //sending an email
            foreach (var msg in messages)
            {
                Reservation r = JsonConvert.DeserializeObject<Reservation>(msg);

                //code to send email using MailGun/sendgrid/....
               
                MimeMessage mail = new MimeMessage();
                mail.From.Add(new MailboxAddress("Excited Admin", ""));
                mail.To.Add(new MailboxAddress("Excited User", r.Email));
                mail.Subject = "Reservation confirmed";
                mail.Body = new TextPart("plain")
                {
                    Text = $"Reservation confirmed for book {r.Isbn} from {r.FromDotNet.ToLongDateString()} till {r.ToDotNet.ToLongDateString()}",
                };

                // Send it!
                using (var client = new SmtpClient())
                {
                    // XXX - Should this be a little different?
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect("smtp.mailgun.org", 587, false);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate("", "");

                    client.Send(mail);
                    client.Disconnect(true);
                }

            }


            return messageCount;
        }
    }
}
