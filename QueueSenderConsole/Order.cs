using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QueueSenderConsole
{
    [DataContract]
    public class Order
    {
        [DataMember]
        public int OrderId { get; set; }
        [DataMember]
        public DateTime OrderTime { get; set; }
        [DataMember]
        public string CustomerName { get; set; }
        [DataMember]
        public string Product { get; set; }


    }
}
