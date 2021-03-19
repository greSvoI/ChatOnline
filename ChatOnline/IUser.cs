using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatOnline
{
    interface IUser
    {
        string ID { get; set; }
        string Name {get; set;}
        string NamePrivate { get; set; }
        bool ConnectPrivate { get; set; }
        bool DisconnectPrivate { get; set; }
        bool ConnectClient { get; set; }
        bool DisconnectClient { get; set; }
        bool SendFile { get; set; }
        bool SendMessage { get; set; }
        string Message { get; set; }
        byte[] Serialize();
        void Desserialize(byte[] data);
    }
}
