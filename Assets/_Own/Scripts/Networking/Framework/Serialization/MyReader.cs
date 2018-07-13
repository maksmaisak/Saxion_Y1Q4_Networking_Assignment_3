using System;
using System.IO;
using System.Text;

public class MyReader : IUnifiedSerializer, IDisposable 
{
	private readonly BinaryReader reader;

	public bool isWriting => false;
	public bool isReading => true;

	public Stream BaseStream => reader.BaseStream;

	private uint maxNumBytesLeft;

	public MyReader(Stream stream, Encoding encoding, bool leaveOpen = true) 
	{
		reader = new BinaryReader(stream, encoding, leaveOpen);
	}

	#region IUnifiedSerializer Implementation

	public void Serialize(ref bool    value) {value = reader.ReadBoolean();}
	public void Serialize(ref byte    value) {value = reader.ReadByte();   }
	public void Serialize(ref sbyte   value) {value = reader.ReadSByte();  }
	public void Serialize(ref short   value) {value = reader.ReadInt16();  }
	public void Serialize(ref int     value) {value = reader.ReadInt32();  }
	public void Serialize(ref long    value) {value = reader.ReadInt64();  }
	public void Serialize(ref ushort  value) {value = reader.ReadUInt16(); }
	public void Serialize(ref uint    value) {value = reader.ReadUInt32(); }
	public void Serialize(ref ulong   value) {value = reader.ReadUInt64(); }
	public void Serialize(ref float   value) {value = reader.ReadSingle(); }
	public void Serialize(ref double  value) {value = reader.ReadDouble(); }
	public void Serialize(ref decimal value) {value = reader.ReadDecimal();}
	public void Serialize(ref char    value) {value = reader.ReadChar();   }
	public void Serialize(ref string  value) {value = reader.ReadString(); }

	public void Serialize(ref byte[]  value, int count) {value = reader.ReadBytes(count);}
	public void Serialize(ref char[]  value, int count) {value = reader.ReadChars(count);}

	#endregion

	public void Dispose() 
	{
		((IDisposable)reader).Dispose();
	}
}
