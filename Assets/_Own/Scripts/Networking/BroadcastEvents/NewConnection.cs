
public class NewConnection : BroadcastEvent<NewConnection>
{
    public readonly Connection connection;

    public NewConnection(Connection connection)
    {
        this.connection = connection;
    }
}