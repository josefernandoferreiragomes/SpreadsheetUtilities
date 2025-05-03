public class LoggingInvoker
{
    private readonly List<ILogCommand> _commands = new();

    public void AddCommand(ILogCommand command)
    {
        _commands.Add(command);
    }

    public void ExecuteCommands()
    {
        foreach (var command in _commands)
        {
            command.Execute();
        }
        _commands.Clear(); // Clear commands after execution
    }
}
