using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatOnline.Model
{
    interface IInfoFile
    {
        string FileName { get; set; }
        long FileLenght { get; set; }
        string PrivateName { get; set; }
        byte[] Serialize();
        void Desserialize(byte[] data);
    }

}
