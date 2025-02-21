using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Counter
{
    public static class ControlsExtensions
    {
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

        public static Button OnClick(this Button button, Action callback)
        {
            button.Click += (obj, args) => callback();
            return button;
        }

        public static Panel AddChildren(this Panel control, params Control[] children)
        {
            control.Children.AddRange(children);
            return control;
        }

        public static ItemsControl AddItems(this ItemsControl control, params object[] items)
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
    }
}
