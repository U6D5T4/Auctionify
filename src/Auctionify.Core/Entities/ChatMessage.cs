using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctionify.Core.Entities
{
    internal class ChatMessage
    {
        public int Id { get; set; }
        public User Sender { get; set; }
        public User Reciever { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
