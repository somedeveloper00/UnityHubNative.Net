using UnityHubNative.Net;

namespace Windows;

static class Program
{
    [STAThread]
    static void Main(string[] args) => UnityHubNativeNetApp.Run(args);
}
