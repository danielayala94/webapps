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
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using System.Diagnostics;

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

            // Get the window's AppWindow instance
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);

            var appWindow = AppWindow.GetFromWindowId(windowId);

            // Extend the title bar into the client area
            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            appWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall; // Keep it minimal

            // Make the title bar buttons transparent
            appWindow.TitleBar.BackgroundColor = Colors.Transparent;
            appWindow.TitleBar.ForegroundColor = Colors.Transparent;
            appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            appWindow.TitleBar.ButtonForegroundColor = Colors.Transparent;

            // Define a custom draggable region
            //int titleBarHeight = 50; // Change this to your desired height
            //var dragArea = new RectInt32(0, 0, appWindow.Size.Width, titleBarHeight);

            // Apply draggable region
            //appWindow.TitleBar.SetDragRectangles(new RectInt32[] { dragArea });



            InitializeWebView();
        }

        /*private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }*/

        private async void InitializeWebView()
        {
            await TitleBarWebView.EnsureCoreWebView2Async();

            string htmlContent = @"
            <html>
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Custom Title Bar</title>

                <style> 
                    /* Ensure no scrollbar issues */
                    html, body {
                        margin: 0;
                        padding: 0;
                        width: 100%;
                        height: 50px;
                        overflow: hidden; /* Prevents scrolling */
                        background: transparent;
                    }

                    #titleBar {
                        font-family: Comic Sans MS, sans-serif; 
                        width: 100%;
                        height: 50px;
                        background: #0078D4;
                        color: white;
                        display: flex;
                        align-items: center;
                        justify-content: space-between;
                        padding: 0 10px;
                        -webkit-app-region: drag; /* Enables dragging */
                        box-sizing: border-box;
                    }

                    #titleSearch {
                        flex: 2;
                        display: flex;
                        justify-content: center;
                        -webkit-app-region: no-drag; /* Allows interaction */
                    }

                    /* Search input styling */
                    #searchInput {
                        width: 200px;
                        padding: 5px;
                        border: none;
                        border-radius: 5px 0 0 5px;
                        font-size: 14px;
                        outline: none;
                    }

                    /* Search button styling */
                    #searchButton {
                        background: #005a9e;
                        color: white;
                        border: none;
                        padding: 6px 10px;
                        border-radius: 0 5px 5px 0;
                        cursor: pointer;
                    }

                    #searchButton:hover {
                        background: #0078D4;
                    }
                </style>
            </head>
            <body>
                <div id=""titleBar"">
                    <span>My first WebView2 app 😊</span>

                    <div id=""titleSearch"">
                        <input type=""text"" id=""searchInput"" placeholder=""Search..."" />
                        <button id=""searchButton"" onclick=""performSearch()"">🔍</button>
                    </div>
                </div>

                <script>
                    function performSearch() {
                        let query = document.getElementById(""searchInput"").value;
                        if (query.trim() !== """") {
                            window.chrome.webview.postMessage(""search:"" + query);
                        }
                    }
                </script>
            </body>
            </html>";

            TitleBarWebView.NavigateToString(htmlContent);
            TitleBarWebView.WebMessageReceived += WebView_WebMessageReceived;

            await WebView.EnsureCoreWebView2Async();
            string htmlContentMainWebView = @"
                <style> body { 
                    font-family: Comic Sans MS, sans-serif; 
                    text-align: center;
                    padding-top: 50px; /* Adjust as needed */
                } </style>

                <h1>My first WinUI 3 WebView2 App</h1>
                <button onclick='showMessage()'>Click Me</button>

                <script>
                    function showMessage() {
                        window.chrome.webview.postMessage('Hello from JavaScript!');
                    }
                </script>
            ";

            WebView.NavigateToString(htmlContentMainWebView);
            WebView.WebMessageReceived += WebView_WebMessageReceived;
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

            string message = args.TryGetWebMessageAsString();

            if (message.StartsWith("search:"))
            {
                string searchQuery = message.Substring(7);
                Debug.WriteLine("User searched for: " + searchQuery);

                WebView.Source = new Uri("https://www.bing.com/search?q=" + searchQuery);
            }
            else if (message.Equals("Hello from JavaScript!"))
            {
                var appNotification = new AppNotificationBuilder()
                .AddArgument("action", "ToastClick")
                //.SetAppLogoOverride(new System.Uri("file://" + App.GetFullPathToAsset("Square150x150Logo.png")), AppNotificationImageCrop.Circle)
                .AddText("My first WebView2 app 😊")
                .AddButton(new AppNotificationButton("Open App")
                    .AddArgument("action", "OpenApp"))
                .BuildNotification();

                AppNotificationManager.Default.Show(appNotification);
            }
        }
    }
}
