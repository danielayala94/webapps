using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace wv2app
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            InitializeWebView();
        }

        /*private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }*/

        private async void InitializeWebView()
        {
            await myWebView.EnsureCoreWebView2Async();

            string htmlContent = @"
            <html>
            <head>
                <style> body { 
                    font-family: Comic Sans MS, sans-serif; 
                    text-align: center;
                    padding-top: 50px; /* Adjust as needed */
                } </style>
            </head>
            <body>
                <h1>My first WinUI 3 WebView2 App</h1>
                <button onclick='showMessage()'>Click Me</button>
                <script>
                    function showMessage() {
                        window.chrome.webview.postMessage('Hello from JavaScript!');
                    }
                </script>
            </body>
            </html>";

            myWebView.NavigateToString(htmlContent);

            myWebView.WebMessageReceived += WebView_WebMessageReceived;
        }

        private void WebView_WebMessageReceived(WebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            /*string message = args.WebMessageAsJson;
            ContentDialog dialog = new ContentDialog
            {
                Title = "Message from JavaScript",
                Content = "Received: " + message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await dialog.ShowAsync();*/

            var appNotification = new AppNotificationBuilder()
            .AddArgument("action", "ToastClick")
            //.SetAppLogoOverride(new System.Uri("file://" + App.GetFullPathToAsset("Square150x150Logo.png")), AppNotificationImageCrop.Circle)
            .AddText("My web app")
            .AddButton(new AppNotificationButton("Open App")
                .AddArgument("action", "OpenApp"))
            .BuildNotification();

            AppNotificationManager.Default.Show(appNotification);

        }
    }
}
