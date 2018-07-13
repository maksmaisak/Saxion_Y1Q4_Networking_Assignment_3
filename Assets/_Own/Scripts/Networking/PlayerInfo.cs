
public struct PlayerInfo : IUnifiedSerializable
{
    public uint id;
    public string nickname;
    
    public void Serialize(IUnifiedSerializer s)
    {
        s.Serialize(ref id);

        bool isNicknameNull = nickname == null;
        s.Serialize(ref isNicknameNull);
        if (!isNicknameNull)
        {
            s.Serialize(ref nickname);
        }
    }
}
