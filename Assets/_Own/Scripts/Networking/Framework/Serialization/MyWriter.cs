using System;
using System.IO;
using System.Text;

// Writes given values to a Stream
public class MyWriter : IUnifiedSerializer, IDisposable 
{
	private readonly BinaryWriter writer;

	public bool isWriting => true;
	public bool isReading => false;

	public Stream BaseStream => writer.BaseStream;

	public MyWriter(Stream stream, Encoding encoding, bool leaveOpen = true) 
	{
		writer = new BinaryWriter(stream, encoding, leaveOpen);
	}
		
	public void Serialize(ref bool    value) {writer.Write(value);}
	public void Serialize(ref byte    value) {writer.Write(value);}

	public void Serialize(ref sbyte   value) {writer.Write(value);}
	public void Serialize(ref short   value) {writer.Write(value);}
	public void Serialize(ref int     value) {writer.Write(value);}
	public void Serialize(ref long    value) {writer.Write(value);}
	public void Serialize(ref ushort  value) {writer.Write(value);}
	public void Serialize(ref uint    value) {writer.Write(value);}
	public void Serialize(ref ulong   value) {writer.Write(value);}
	public void Serialize(ref float   value) {writer.Write(value);}
	public void Serialize(ref double  value) {writer.Write(value);}
	public void Serialize(ref decimal value) {writer.Write(value);}
	public void Serialize(ref char    value) {writer.Write(value);}
	public void Serialize(ref string  value) {writer.Write(value);}

	public void Serialize(ref byte[]  value, int count) {writer.Write(value);}
	public void Serialize(ref char[]  value, int count) {writer.Write(value);}

	public void Dispose() 
	{
		((IDisposable)writer).Dispose();
	}
}
