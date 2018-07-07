using System;

/// An object that could be serialized/deserialized using a unified serialization function 
public interface IUnifiedSerializable
{
	void Serialize(IUnifiedSerializer s);
}
