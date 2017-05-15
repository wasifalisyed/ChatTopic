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
           
           
                while (true)
                {
                  BrokeredMessage msg;
                    var input = "1";
                    var ord = new Order();
                    ord.OrderId = id;
                    ord.OrderTime = DateTime.Now;
                    ord.CustomerName = "wasif";
                    ord.Product = "ball";
                  
                    ;
                    Task t = new Task(() =>
                    {
                        if(objectType=="1")
                        {
                             msg = new BrokeredMessage(ord);
                             msg.MessageId = id.ToString();
                             msg.Properties.Add("type", "json");
                             qc.Send(msg);
                             Console.WriteLine(msg.MessageId);
                        }
                           
                        else
                        {
                            string json = Newtonsoft.Json.JsonConvert.SerializeObject(ord);
                            msg = new BrokeredMessage(json);
                            msg.MessageId = id.ToString();
                            msg.ContentType = "application/json";
                            msg.Label = ord.GetType().ToString();
                            qc.Send(msg);
                        }
                       
                      

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

            Console.WriteLine("waiting for messages");
            
            
                var qc = QueueClient.CreateFromConnectionString(connection, queue);
                
                qc.OnMessage(msg =>
                {
                    var obj = new Order();
                    if(!(msg.ContentType== "application/json"))
                    {
                        obj = msg.GetBody<Order>();
                        Console.WriteLine("Message Id:" + msg.MessageId + " CustomerName:" + obj.CustomerName + " Order Date: " + obj.OrderTime );
                    }
                    else
                    {
                        string ord = msg.GetBody<string>();
                        try
                        {
                           dynamic order = Newtonsoft.Json.JsonConvert.DeserializeObject(ord);
                            Console.WriteLine("Message Id:" + msg.MessageId + " CustomerName:" + order.CustomerName + " Order Date: " + order.OrderTime);
                        } catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        

                    }
                  
                   

                });
            
        }
    }
   
}
