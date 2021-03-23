using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChatOnline
{
    public class User : IUser
    {
        public string ID { get ; set ; }
        public string Name { get ; set ; }
		public string NamePrivate { get; set; } = "";
		public bool ConnectPrivate { get; set; } = false;
		public bool DisconnectPrivate { get; set; } = false;
		public bool SendFile { get; set; } = false;
		public bool ConnectClient { get ; set ; } = false;
		public bool DisconnectClient { get ; set ; } = false;
		public bool SendMessage { get; set ; } = false;
		public string Message { get; set; } = "";
		private Brush brush;
		public Brush Brush { get => brush; set { brush = value; } }

       

        public byte[] Serialize()
		{
			using (MemoryStream m = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(m))
				{
					writer.Write(ID);
					writer.Write(Name);
					writer.Write(NamePrivate);
					writer.Write(ConnectPrivate);
					writer.Write(DisconnectPrivate);
					writer.Write(SendFile);
					writer.Write(ConnectClient);
					writer.Write(DisconnectClient);
					writer.Write(SendMessage);
					writer.Write(Message);					
				}
				return m.ToArray();
			}
		}
		public void Desserialize(byte[] data)
		{

			using (MemoryStream m = new MemoryStream(data))
			{
				using (BinaryReader reader = new BinaryReader(m))
				{
					ID = reader.ReadString();
					Name = reader.ReadString();
					NamePrivate = reader.ReadString();
					ConnectPrivate = reader.ReadBoolean();
					DisconnectPrivate = reader.ReadBoolean();
					SendFile = reader.ReadBoolean();
					ConnectClient = reader.ReadBoolean();
					DisconnectClient = reader.ReadBoolean();
					SendMessage = reader.ReadBoolean();
					Message = reader.ReadString();
				}
			}
		}
	}
}
