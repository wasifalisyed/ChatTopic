using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus;

namespace ChatTopic
{
    class Program
    {
        static string connection = "Endpoint=sb://wasifbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=iIEQztl3IJfr8cfoBvS6CydqJnf8dAZ9hnVhBOOa/3I=";
        static string topic = "ChatTopic";
        static string name = "";
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your name");
            name=Console.ReadLine();
            CreateTopic();
            CreateNamespace();
            MessagePump();
            SendMessage();
        }

        private static void SendMessage()
        {
            Console.WriteLine("Type exit to close chat");
            var factory = MessagingFactory.CreateFromConnectionString(connection);
            var tc = factory.CreateTopicClient(topic);
           // var tc = TopicClient.CreateFromConnectionString(connection, topic);
            tc.Send(new BrokeredMessage("Hey") { Label = name });
            while(true)
            {
                var text = Console.ReadLine();
                if (text == "exit")
                    break;
                tc.Send(new BrokeredMessage(text) { Label = name });
            }
            factory.Close();
        }

        private static void MessagePump()
        {
            var factory = MessagingFactory.CreateFromConnectionString(connection);
            var sc = factory.CreateSubscriptionClient(topic,name);
           // var sc = SubscriptionClient.CreateFromConnectionString(connection, topic,"AllChat");
            sc.OnMessage(msg =>
           {
               if (msg.Label != name)
               {
                   Console.WriteLine(msg.Label + ">" + msg.GetBody<string>());
               }
               //else
               //{
               //    msg.Abandon();
                
               //}
           }
            );

        }

        private static void CreateTopic()
        {
            var manager = NamespaceManager.CreateFromConnectionString(connection);
            if (!manager.TopicExists(topic))
            {
                TopicDescription td = new TopicDescription(topic);
                td.MaxSizeInMegabytes = 5120;
                td.AutoDeleteOnIdle = TimeSpan.FromMinutes(5);
                td.DefaultMessageTimeToLive = new TimeSpan(0, 50, 0);
               // td.Path = topic;
                manager.CreateTopic(td);
        }
    }

        private static void CreateNamespace()
        {
            var manager = NamespaceManager.CreateFromConnectionString(connection);
            if(!manager.SubscriptionExists(topic,name))
            {
           
                manager.CreateSubscription(topic,name);
            }
        }
    }
}
