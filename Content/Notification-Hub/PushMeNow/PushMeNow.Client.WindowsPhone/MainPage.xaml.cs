using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Microsoft.WindowsAzure.Messaging;
using PushMeNow.Client.WindowsPhone.Models;
using Windows.Networking.PushNotifications;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace PushMeNow.Client.WindowsPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            string message;
            try
            {
                IsEnabled = false;
                var userInfo = new UserInfo { Name = textBoxName.Text.Trim() };
                if (string.IsNullOrWhiteSpace(userInfo.Name))
                {
                    throw new Exception("Please enter your name");
                }

                string hubName = "<HUB-NAME>";
                string listenConnectionString = "<HUB-LISTEN-CONNECTION_STRING>";

                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                var hub = new NotificationHub(hubName, listenConnectionString);
                string tag = userInfo.AsTag();
                var result = await hub.RegisterNativeAsync(channel.Uri, new[] { tag });

                if (result.RegistrationId == null)
                {
                    throw new Exception("No registration id");
                }

                message = "Registration successful: " + result.RegistrationId;
            }
            catch (Exception exception)
            {
                message = "Registration failed: " + exception.Message;
            }
            finally
            {
                IsEnabled = true;
            }

            var dialog = new MessageDialog(message);
            dialog.Commands.Add(new UICommand("OK"));
            await dialog.ShowAsync();
        }
    }
}
