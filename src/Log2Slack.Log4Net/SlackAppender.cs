using log4net.Appender;
using log4net.Core;
using SlackSharp;
using SlackSharp.Models;
using System;
using System.Threading.Tasks;

namespace Log2Slack.Log4Net
{
    public class SlackAppender : AppenderSkeleton
    {
        private IWebHookClient _client;

        public string WebHookUrl { get; set; }

        public Type SerializerType { get; set; }

        public string Channel { get; set; }

        public string Username { get; set; }

        public string IconUrl { get; set; }

        public string IconEmoji { get; set; }

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            if (string.IsNullOrEmpty(WebHookUrl)) throw new LogException($"{nameof(WebHookUrl)} must not be empty or null.");
            if (SerializerType == null) throw new LogException($"{nameof(SerializerType)} must not be empty or null.");

            var serializer = Activator.CreateInstance(SerializerType) as IHttpContentJsonSerializer;

            if (serializer == null)
            {
                throw new LogException($"{nameof(SerializerType)} must implements {typeof(IHttpContentJsonSerializer).FullName}.");
            }

            _client = new WebHookClient(serializer);
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var message = RenderLoggingEvent(loggingEvent);

            _client
                .SendAsync(WebHookUrl, new Payload { Channel = Channel, Username = Username, IconEmoji = IconEmoji, IconUrl = IconUrl, Text = message })
                .ContinueWith(t => ErrorHandler.Error(t.Exception.ToString()), TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
