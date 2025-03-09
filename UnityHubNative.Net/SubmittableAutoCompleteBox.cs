using Avalonia.Controls;
using Avalonia.Input;

class SubmittableAutoCompleteBox<T> : AutoCompleteBox where T : class
{
    public event Action<T> Submitted;

    protected override Type StyleKeyOverride => typeof(AutoCompleteBox);

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (SelectedItem != null)
            {
                Submitted?.Invoke((T)SelectedItem);
                e.Handled = true;
            }
        }
        base.OnKeyDown(e);
    }

    public SubmittableAutoCompleteBox<T> OnSubmit(Action<T> action)
    {
        Submitted += action;
        return this;
    }
}
