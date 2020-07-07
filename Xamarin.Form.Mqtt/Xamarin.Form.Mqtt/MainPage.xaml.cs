using System.ComponentModel;
using Xamarin.Forms;

namespace Xamarin.Form.Mqtt
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<string, string>("SENDER_MAYBE_YOUR_TOPIC_NAME_BUT_WHATEVER", "SOME_MESSAGE_TITLE", (sender, args) =>
            {
                //You got a message from Mqtt, do some other thing. 

                Device.BeginInvokeOnMainThread(() =>
                {
                    //You got a message from Mqtt, do some UI thing. 
                });
            });
        }
    }
}