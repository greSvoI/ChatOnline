using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerChat
{
    public class User : IUser
    {
		public string ID { get; set; }
		public string Name { get; set; }
		public string NamePrivate { get; set; }
		public bool ConnectPrivate { get; set; }
		public bool DisconnectPrivate { get; set; }
		public bool SendFile { get; set; }
		public bool ConnectClient { get; set; }
		public bool DisconnectClient { get; set; }
		public bool SendMessage { get; set; }
		public string Message { get; set; } = "";
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
