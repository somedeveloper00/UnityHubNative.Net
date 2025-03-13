using Avalonia.Controls;
using Avalonia.Input;

namespace UnityHubNative.Net;

class SubmitableListBox : ListBox
{
    static readonly KeyGesture[] DefaultSubmitGesture =
    [
        new KeyGesture(Key.Enter, KeyModifiers.None),
        new KeyGesture(Key.Space, KeyModifiers.None)
    ];

    /// <summary>
    /// Gets invoked when submitted on
    /// </summary>
    public event Action? OnSubmit;

    /// <summary>
    /// List of acceptable <see cref="KeyGesture"/> to invoke <see cref="OnSubmit"/>
    /// </summary>
    public KeyGesture[] submitGestures = DefaultSubmitGesture;

    protected override Type StyleKeyOverride => typeof(ListBox);

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (!e.Handled && e.InitialPressMouseButton == MouseButton.Left)
        {
            OnSubmit?.Invoke();
            e.Handled = true;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (!e.Handled && submitGestures.Any(g => g.Matches(e)))
        {
            if (SelectedItem != null)
                OnSubmit?.Invoke();
            e.Handled = true;
            return;
        }
        base.OnKeyDown(e);
    }

    public SubmitableListBox AddOnSubmit(Action callback)
    {
        OnSubmit += callback;
        return this;
    }
}
