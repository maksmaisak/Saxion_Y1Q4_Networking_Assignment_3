using System.Collections.Generic;

public class ChatboxState
{
    private readonly List<string> lines = new List<string>();
    private readonly HashSet<string> nicknames = new HashSet<string>();

    public IReadOnlyCollection<string> GetLines() => lines.AsReadOnly();
    public void AddLine(string line) => lines.Add(line);
    
    public void AddNickname   (string nickname) => nicknames.Add     (nickname);
    public void RemoveNickname(string nickname) => nicknames.Remove  (nickname);
    public bool HasNickname   (string nickname) => nicknames.Contains(nickname);
}