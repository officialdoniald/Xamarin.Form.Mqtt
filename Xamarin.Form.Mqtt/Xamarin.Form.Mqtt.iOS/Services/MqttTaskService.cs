using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Receiving;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.Form.Mqtt.iOS.Services
{
    public class MqttTaskService
    {
        private IMqttClient _mqttClient;
        private string _YourTopic;

        public MqttTaskService()
        {
            _YourTopic = "YOUR_TOPIC";

            _mqttClient = new MqttClientRepository().Create(
             "MqttServer",
             //"MqttPort IN INTEGER"
             1234,
             "MqttUserName",
             "MqttPassword",
             new List<string> { _YourTopic });

            _mqttClient.ApplicationMessageReceivedHandler = new SubscribeCallback(_YourTopic);
        }

        private void SomeEventThatYouHaveToChangeYouSubscribtions(object sender, System.EventArgs e)
        {
            _mqttClient.UnsubscribeAsync(_YourTopic);

            _YourTopic = "CHANGED";

            _mqttClient.SubscribeAsync(
                new MQTTnet.Client.Subscribing.MqttClientSubscribeOptions()
                {
                    TopicFilters = new List<MqttTopicFilter>()
                    {
                        new MqttTopicFilter()
                        {
                            Topic = _YourTopic
                        }
                    }
                });
        }

        public void UnSubscribe()
        {
            _mqttClient.ApplicationMessageReceivedHandler = null;
        }
    }

    public class SubscribeCallback : IMqttApplicationMessageReceivedHandler
    {
        private readonly string _YourTopic;

        public SubscribeCallback(string YourTopic)
        {
            _YourTopic = YourTopic;
        }

        public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            if (e.ApplicationMessage.Topic == _YourTopic)
            {
                MessagingCenter.Send("SENDER_MAYBE_YOUR_TOPIC_NAME_BUT_WHATEVER", "SOME_MESSAGE_TITLE", message);
            }

            return Task.CompletedTask;
        }
    }
}