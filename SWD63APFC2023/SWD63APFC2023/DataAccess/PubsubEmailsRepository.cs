using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Newtonsoft.Json;
using SWD63APFC2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
namespace SWD63APFC2023.DataAccess
{
    public class PubsubEmailsRepository
    {
        TopicName topicName;
        public PubsubEmailsRepository(string projectId)
        {
            try
            {   PublisherServiceApiClient publisher = PublisherServiceApiClient.Create();
                topicName = TopicName.FromProjectTopic(projectId, "messages"); //projects/swd63a2023-377009/topics/messages
                if (publisher.GetTopic(topicName) == null)
                {
                    Topic topic = null;
                    topic = publisher.CreateTopic(topicName);
                }
            }
            catch (Exception ex)
            {//log
                throw ex;
            }
        }

        public async Task<string> PushMessage(Reservation r)
        {
         
            PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

            var reservation = JsonConvert.SerializeObject(r);

            var pubsubMessage = new PubsubMessage
            {
                Data = ByteString.CopyFromUtf8(reservation),
                Attributes =
                {
                    { "priority", "low" }
                }
            };

            string message = await publisher.PublishAsync(pubsubMessage);
            return message;
        }
    }
}
