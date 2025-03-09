using System.Windows.Input;

namespace UnityHubNative.Net;

struct MenuCommand(Func<bool> canExecute, Action executed) : ICommand
{
    public event Func<bool> CanExecute = canExecute;
    public event Action Executed = executed;
    public event EventHandler? CanExecuteChanged;

    public MenuCommand(Action executed) : this(null!, executed) { }

    readonly bool ICommand.CanExecute(object? parameter) => CanExecute?.Invoke() ?? true;
    readonly void ICommand.Execute(object? parameter) => Executed();
}
