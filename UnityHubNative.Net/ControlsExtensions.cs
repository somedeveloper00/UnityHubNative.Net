using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using FluentAvalonia.UI.Controls;

namespace UnityHubNative.Net;

public static class ControlsExtensions
{
    public static T OnValueChanged<T>(this T slider, Action callback) where T : RangeBase
    {
        slider.ValueChanged += (_, _) => callback();
        return slider;
    }

    public static T OnCheckChanged<T>(this T checkbox, Action callback) where T : CheckBox
    {
        checkbox.IsCheckedChanged += (_, _) => callback();
        return checkbox;
    }

    public static T OnKeyDown<T>(this T element, params Action<T, KeyEventArgs>[] callbacks) where T : InputElement
    {
        for (int i = 0; i < callbacks.Length; i++)
        {
            int index = 0;
            element.KeyDown += (obj, args) => callbacks[index]((T)obj!, args);
        }

        return element;
    }

    public static T OnKeyDown<T>(this T element, Action<T, KeyEventArgs> callback) where T : InputElement
    {
        element.KeyDown += (obj, args) => callback((T)obj!, args);
        return element;
    }

    public static Button OnClick(this Button button, params Action<Button, RoutedEventArgs>[] callbacks)
    {
        for (int i = 0; i < callbacks.Length; i++)
        {
            int index = 0;
            button.Click += (obj, args) => callbacks[index]((Button)obj!, args);
        }

        return button;
    }

    public static Button OnClick(this Button button, Action<Button, RoutedEventArgs> callback)
    {
        button.Click += (obj, args) => callback((Button)obj!, args);
        return button;
    }

    public static Button OnClick(this Button button, params Action[] callbacks)
    {
        for (int i = 0; i < callbacks.Length; i++)
        {
            int index = 0;
            button.Click += (obj, args) => callbacks[index]();
        }

        return button;
    }

    public static T OnClick<T>(this T button, Action callback) where T : Button
    {
        button.Click += (obj, args) => callback();
        return button;
    }

    public static NativeMenuItem OnClick(this NativeMenuItem button, Action callback)
    {
        button.Click += (obj, args) => callback();
        return button;
    }

    public static MenuItem OnClick(this MenuItem menuItem, params Action<MenuItem, RoutedEventArgs>[] callbacks)
    {
        for (int i = 0; i < callbacks.Length; i++)
        {
            int index = 0;
            menuItem.Click += (obj, args) => callbacks[index]((MenuItem)obj!, args);
        }

        return menuItem;
    }

    public static MenuItem OnClick(this MenuItem menuItem, Action<MenuItem, RoutedEventArgs> callback)
    {
        menuItem.Click += (obj, args) => callback((MenuItem)obj!, args);
        return menuItem;
    }

    public static MenuItem OnClick(this MenuItem menuItem, params Action[] callbacks)
    {
        for (int i = 0; i < callbacks.Length; i++)
        {
            int index = 0;
            menuItem.Click += (obj, args) => callbacks[index]();
        }

        return menuItem;
    }

    public static MenuItem OnClick(this MenuItem menuItem, Action callback)
    {
        menuItem.Click += (obj, args) => callback();
        return menuItem;
    }

    public static MenuFlyoutItem OnClick(this MenuFlyoutItem menuItem, params Action[] callbacks)
    {
        for (int i = 0; i < callbacks.Length; i++)
        {
            int index = 0;
            menuItem.Click += (obj, args) => callbacks[index]();
        }

        return menuItem;
    }

    public static MenuFlyoutItem OnClick(this MenuFlyoutItem menuItem, Action callback)
    {
        menuItem.Click += (obj, args) => callback();
        return menuItem;
    }

    public static T AddChildren<T>(this T control, params Control[] children) where T : Panel
    {
        control.Children.AddRange(children);
        return control;
    }

    public static T AddItems<T>(this T control, params object[] items) where T : ItemsControl
    {
        for (int i = 0; i < items.Length; i++)
            control.Items.Add(items[i]);
        return control;
    }

    public static MenuItem AddItems(this MenuItem menu, params MenuItem[] menuItems)
    {
        for (int i = 0; i < menuItems.Length; i++)
            menu.Items.Add(menuItems[i]);
        return menu;
    }

    public static MenuFlyout AddItems(this MenuFlyout menu, params MenuItem[] menuItems)
    {
        for (int i = 0; i < menuItems.Length; i++)
            menu.Items.Add(menuItems[i]);
        return menu;
    }

    public static T1 AddItems<T1, T2>(this T1 menu, T2[] items) where T1 : NativeMenu where T2 : NativeMenuItemBase
    {
        for (int i = 0; i < items.Length; i++)
            menu.Items.Add(items[i]);
        return menu;
    }

    public static TabControl AddItems(this TabControl tabControl, TabItem[] items)
    {
        for (int i = 0; i < items.Length; i++)
            tabControl.Items.Add(items[i]);
        return tabControl;
    }

    public static T SetDock<T>(this T control, Dock dock) where T : Control
    {
        control.SetValue(DockPanel.DockProperty, dock);
        return control;
    }

    public static T SetGrid<T>(this T control, int column, int row, int columnSpan = 1, int rowSpan = 1) where T : Control
    {
        if (column != 0)
            SetColumn(control, column);
        if (row != 0)
            SetRow(control, row);
        if (columnSpan != 1)
            SetColumnSpan(control, columnSpan);
        if (rowSpan != 1)
            SetRowSpan(control, rowSpan);
        return control;
    }

    public static T SetColumn<T>(this T control, int value) where T : Control
    {
        control.SetValue(Grid.ColumnProperty, value);
        return control;
    }

    public static T SetRow<T>(this T control, int value) where T : Control
    {
        control.SetValue(Grid.RowProperty, value);
        return control;
    }

    public static T SetColumnSpan<T>(this T control, int value) where T : Control
    {
        control.SetValue(Grid.ColumnSpanProperty, value);
        return control;
    }

    public static T SetRowSpan<T>(this T control, int value) where T : Control
    {
        control.SetValue(Grid.RowSpanProperty, value);
        return control;
    }

    public static T SetTooltip<T>(this T control, string tooltip) where T : Control
    {
        control.SetValue(ToolTip.TipProperty, tooltip);
        return control;
    }

    public static T OnSelectionChanged<T>(this T control, Action action) where T : SelectingItemsControl
    {
        control.SelectionChanged += (_, _) => action();
        return control;
    }

    public static T OnTextChanged<T>(this T control, Action action) where T : Control
    {
        control.SizeChanged += (_, e) => action();
        return control;
    }

    public static T OnOpening<T>(this T menu, Action<object, T> action) where T : MenuFlyout
    {
        menu.Opened += (src, args) => action?.Invoke((object)args, (T)src);
        menu.Opening += (src, args) => action?.Invoke((object)args, (T)src);
        return menu;
    }
}
