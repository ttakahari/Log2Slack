using NLog;
using NLog.Common;
using NLog.Targets;
using SlackSharp;
using SlackSharp.Models;
using System;
using System.Threading.Tasks;

namespace Log2Slack.NLog
{
    [Target("Slack")]
    public class SlackTarget : TargetWithLayout
    {
        private IWebHookClient _client;

        public string WebHookUrl { get; set; }

        public Type SerializerType { get; set; }

        public string Channel { get; set; }

        public string Username { get; set; }

        public string IconUrl { get; set; }

        public string IconEmoji { get; set; }

        public SlackTarget()
            : this("Slack")
        {
        }

        public SlackTarget(string name)
            => Name = name ?? throw new ArgumentNullException(nameof(name));

        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            if (string.IsNullOrEmpty(WebHookUrl)) throw new NLogConfigurationException($"{nameof(WebHookUrl)} must not be empty or null.");
            if (SerializerType == null) throw new NLogConfigurationException($"{nameof(SerializerType)} must not be empty or null.");

            var serializer = Activator.CreateInstance(SerializerType) as IHttpContentJsonSerializer;

            if (serializer == null)
            {
                throw new NLogConfigurationException($"{nameof(SerializerType)} must implements {typeof(IHttpContentJsonSerializer).FullName}.");
            }

            _client = new WebHookClient(serializer);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var log = Layout.Render(logEvent);

            _client
                .SendAsync(WebHookUrl, new Payload { Channel = Channel, Username = Username, IconEmoji = IconEmoji, IconUrl = IconUrl, Text = log })
                .ContinueWith(t => InternalLogger.Error(t.Exception.ToString()), TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
