using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace QueueSenderConsole
{
   public class Program
    {
        static string connection = "Endpoint=sb://wasifbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=iIEQztl3IJfr8cfoBvS6CydqJnf8dAZ9hnVhBOOa/3I=";
        static string queue = "OrderQueue";
        static void Main(string[] args)
        {
          
            Console.WriteLine("press 1 for message sender and 2 for receiver");
           
            var input = Console.ReadLine();
            if(input.Equals("1"))
            {
                CreateQueue();
                MessageWriter();
            }
            else if(input.Equals("2"))
            {
                MessageReader();
                Console.ReadLine();
            }
        }
        static void MessageWriter()
        {
            var qc = QueueClient.CreateFromConnectionString(connection, queue);
            int id = 3;
            Console.WriteLine("Press 1 for object 2 for json");
            var objectType = Console.ReadLine();
            if (objectType == "1")
            {
                while (true)
                {
                    var input = "1";
                    var ord = new Order();
                    ord.OrderId = id;
                    ord.OrderTime = DateTime.Now;
                    ord.CustomerName = "wasif";
                    ord.Product = "ball";
                    BrokeredMessage msg = new BrokeredMessage(ord);
                    msg.MessageId = id.ToString();
                    msg.Properties.Add("env", "test");
                    Task t = new Task(() =>
                    {
                        qc.Send(msg);
                        Console.WriteLine(msg.MessageId);

                    });
                    t.Start();
                  var display=  t.ContinueWith((x) =>
                    {
                        Console.WriteLine("Press 1 to add more order or any key to exit");
                        input= Console.ReadLine();
                   
                    });
                    display.Wait();
                    if(input!="1")
                    {
                        qc.Close();
                        break;
                    }
                    id = id + 1;
                    
                }
            }
        }
        static void CreateQueue()
        {
            var ns = NamespaceManager.CreateFromConnectionString(connection);
           if(!ns.QueueExists(queue))
            {
                var qd = new QueueDescription(queue);
                qd.AutoDeleteOnIdle = new TimeSpan(1, 0, 0);
                ns.CreateQueue(qd);
            }
        }
        static void MessageReader()
        {
            Console.WriteLine("Press 1 for object and 2 for Json");
            var objType = Console.ReadLine();
            if(objType.Equals("1"))
            {
                var qc = QueueClient.CreateFromConnectionString(connection, queue);
                qc.OnMessage(msg =>
                {
                    var obj = msg.GetBody<Order>();
                    Console.WriteLine("Message Id:" + msg.MessageId + " CustomerName:" + obj.CustomerName + " Order Date: " + obj.OrderTime+" env value:"+msg.Properties["env"].ToString());

                });
            }
        }
    }
   
}
