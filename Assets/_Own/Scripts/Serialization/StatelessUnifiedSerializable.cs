using System;

/// A class to inherit from if your class needs toe be IUnifiedSerializable but has no state to serialize.
public abstract class StatelessUnifiedSerializable : IUnifiedSerializable 
{
	public void Serialize(IUnifiedSerializer s) {}
}

