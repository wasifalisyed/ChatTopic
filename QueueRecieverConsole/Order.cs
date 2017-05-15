using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using System.Runtime.Serialization;

namespace QueueRecieverConsole
{
   [DataContract]
    public class Order
    {
        public int OrderId { get; set; }

        public DateTime OrderTime { get; set; }
        public string CustomerName{get;set;}

       public string Product { get; set; }


    }
}
