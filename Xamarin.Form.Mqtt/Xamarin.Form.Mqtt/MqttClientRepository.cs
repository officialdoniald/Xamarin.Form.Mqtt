using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Form.Mqtt.Messages;
using Xamarin.Forms;

namespace Xamarin.Form.Mqtt
{
    public class MqttClientRepository
    {

        Dictionary<string, MqttTopicFilter> _topicFilter;

        private static IMqttClient client;

        public IMqttClient Create(string server, int? port, string userName, string password, List<string> topics)
        {
            _topicFilter = new Dictionary<string, MqttTopicFilter>();

            foreach (var topic in topics)
            {
                MqttTopicFilter topicFilter = new MqttTopicFilter
                {
                    Topic = topic,
                    QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce
                };

                _topicFilter.Add(topic, topicFilter);
            }

            Task.Run(() => MqttClientRunAsync(server, port, userName, password)).Wait();

            return client;
        }

        private async Task MqttClientRunAsync(string server, int? port, string userName, string password)
        {
            try
            {
                var options = new MqttClientOptions
                {
                    ClientId = "YOUR_CLIENT_ID",
                    CleanSession = true,
                    ChannelOptions = new MqttClientTcpOptions
                    {
                        Server = server,
                        Port = port
                    },
                    Credentials = new MqttClientCredentials
                    {
                        Username = userName,
                        Password = Encoding.UTF8.GetBytes(password)
                    }
                };

                var factory = new MqttFactory();

                client = factory.CreateMqttClient();

                client.ConnectedHandler = new MqttConnectedHandler(_topicFilter, client);
                client.DisconnectedHandler = new MqttDisconnectedHandler(options, client);

                try
                {
                    await client.ConnectAsync(options);
                }
                catch (Exception)
                {
                    
                }
            }
            catch (Exception)
            {
                
            }
        }
    }

    public class MqttDisconnectedHandler : IMqttClientDisconnectedHandler
    {
        private IMqttClient _client;
        private MqttClientOptions _options;

        public MqttDisconnectedHandler(MqttClientOptions options, IMqttClient client)
        {
            _options = options;
            _client = client;
        }

        public async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));

            try
            {
                //await _client.ConnectAsync(_options);
                var message = new StartMqttTaskMessage();
                MessagingCenter.Send(message, "StartMqttTaskMessage");
            }
            catch
            {
                
            }
        }
    }

    public class MqttConnectedHandler : IMqttClientConnectedHandler
    {
        private IMqttClient _client;
        private Dictionary<string, MqttTopicFilter> _topicFilter;

        public MqttConnectedHandler(Dictionary<string, MqttTopicFilter> topicFilter, IMqttClient client)
        {
            _topicFilter = topicFilter;
            _client = client;
        }

        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            await _client.SubscribeAsync(_topicFilter.Values.ToArray());
        }
    }
}
