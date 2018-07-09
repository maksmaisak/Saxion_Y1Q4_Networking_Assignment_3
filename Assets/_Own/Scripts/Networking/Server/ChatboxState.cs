using System.Collections.Generic;
using UnityEngine.Assertions;

public class ChatboxState
{
    private readonly List<string> lines = new List<string>();
    private readonly HashSet<string> nicknames = new HashSet<string>();

    public IReadOnlyCollection<string> GetLines() => lines.AsReadOnly();

    public void AddLine(string line)
    {
        lines.Add(line);
    }

    public bool AddNickname(string nickname)
    {        
        if (!nicknames.Add(nickname)) return false;
        
        Server.Instance.SendAllConnectedPlayers(new UserJoinedMessage(nickname));
        Server.Instance.SendAllConnectedPlayers(NewChatEntryMessage.MakeWithTimestamp($"{nickname} has joined the chat.", NewChatEntryMessage.Kind.ServerMessage));
        return true;
    }

    public bool RemoveNickname(string nickname)
    {
        if (!nicknames.Remove(nickname)) return false;
        
        Server.Instance.SendAllConnectedPlayers(new UserLeftMessage(nickname));
        Server.Instance.SendAllConnectedPlayers(NewChatEntryMessage.MakeWithTimestamp($"{nickname} has left the chat.", NewChatEntryMessage.Kind.ServerMessage));
        return true;
    }

    public bool HasNickname   (string nickname) => nicknames.Contains(nickname);
    public IEnumerable<string> GetNicknames() => nicknames;
}