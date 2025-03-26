using System.Data;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Layout;
using FluentAvalonia.UI.Controls;

namespace UnityHubNative.Net;

public static class CommonTransitions
{
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(0.1);
    private static readonly TimeSpan Durartion = TimeSpan.FromSeconds(0.3);
    private static readonly Easing Easing = new SineEaseInOut();

    public static readonly DoubleTransition Width = new()
    {
        Property = Layoutable.WidthProperty,
        Delay = Delay,
        Duration = Durartion,
        Easing = Easing
    };

    public static readonly DoubleTransition Height = new()
    {
        Property = Layoutable.HeightProperty,
        Delay = Delay,
        Duration = Durartion,
        Easing = Easing
    };

    public static readonly CornerRadiusTransition CornerRadius = new()
    {
        Property = Border.CornerRadiusProperty,
        Delay = Delay,
        Duration = Durartion,
        Easing = Easing
    };

    public static readonly DoubleTransition Opacity = new()
    {
        Property = Avalonia.Visual.OpacityProperty,
        Delay = Delay,
        Duration = Durartion,
        Easing = Easing
    };

    public static readonly Transitions BasicTransitions = [Width, Height, Opacity, CornerRadius];
}
