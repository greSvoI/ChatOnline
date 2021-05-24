using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatOnline.Model
{
    public class InfoFile:IInfoFile
    {
        public string FileName { get; set; }
		public long FileLenght { get; set; } = 0;
		public string PrivateName { get; set; } = "";

        public byte[] Serialize()
		{
			using (MemoryStream m = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(m))
				{
					writer.Write(FileName);
					writer.Write(FileLenght);
					writer.Write(PrivateName);
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
					FileName = reader.ReadString();
					FileLenght = reader.ReadInt64();
					PrivateName = reader.ReadString();
				}
			}
		}
	}
}
