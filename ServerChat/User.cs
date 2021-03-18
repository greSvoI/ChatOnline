using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerChat
{
    public class User : IUser
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool Chat { get; set; }
        public bool SendFile { get; set; }
        public bool ConnectClient { get; set; }
        public bool DisconnectClient { get; set; }
        public bool SendMessage { get; set; }
        public string Message { get; set; }
    }
}
