using JsonHttpContentConverter.Jil;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System.Reflection;
using System.Xml;
using Xunit;

namespace Log2Slack.Log4Net.Tests
{
    public class SlackAppenderTests
    {
        private const string WebHookUrl = @"https://hooks.slack.com/services/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";

        [Fact]
        public void InvalidXmlConfig_Tests()
        {
#if NETCOREAPP1_0 || NETCOREAPP1_1
            var hierarchy = LogManager.CreateRepository(GetType().GetTypeInfo().Assembly, typeof(Hierarchy));
#else
            var hierarchy = LogManager.CreateRepository(GetType().Assembly, typeof(Hierarchy));
#endif

            {
                var xml = new XmlDocument();

                xml.LoadXml($@"
<log4net>
    <appender name=""SlackAppender"" type=""Log2Slack.Log4Net.SlackAppender, Log2Slack.Log4Net"">
        <param name=""SerializerType"" value=""JsonHttpContentConverter.Jil.JilHttpContentConverter, JsonHttpContentConverter.Jil"" />
        <layout type=""log4net.Layout.PatternLayout"">
            <ConversionPattern value=""%date|%-5level|%message"" />
        </layout>
        <filter>
            <param name=""LevelMin"" value=""DEBUG"" />
            <param name=""LevelMax"" value=""FATAL"" />
        </filter>
    </appender>
    <logger name=""test"">
        <appender-ref ref=""SlackAppender"" />
    </logger>
</log4net>");

                XmlConfigurator.Configure(hierarchy, xml["log4net"]);

                var logger = LogManager.GetLogger(hierarchy.Name, "test");

                // It dosen't send log.
                logger.Info("log4net.invalidxml.test");
            }

            {
                var xml = new XmlDocument();

                xml.LoadXml($@"
<log4net>
    <appender name=""SlackAppender"" type=""Log2Slack.Log4Net.SlackAppender, Log2Slack.Log4Net"">
        <param name=""WebHookUrl"" value=""{WebHookUrl}"" />
        <layout type=""log4net.Layout.PatternLayout"">
            <ConversionPattern value=""%date|%-5level|%message"" />
        </layout>
        <filter>
            <param name=""LevelMin"" value=""DEBUG"" />
            <param name=""LevelMax"" value=""FATAL"" />
        </filter>
    </appender>
    <logger name=""test"">
        <appender-ref ref=""SlackAppender"" />
    </logger>
</log4net>");

                XmlConfigurator.Configure(hierarchy, xml["log4net"]);

                var logger = LogManager.GetLogger(hierarchy.Name, "test");

                // It dosen't send log.
                logger.Info("log4net.invalidxml.test");
            }

            {
                var xml = new XmlDocument();

                xml.LoadXml($@"
<log4net>
    <appender name=""SlackAppender"" type=""Log2Slack.Log4Net.SlackAppender, Log2Slack.Log4Net"">
        <param name=""WebHookUrl"" value=""{WebHookUrl}"" />
        <param name=""SerializerType"" value=""Log2Slack.Log4Net, Log2Slack.Log4Net.DummySerializer"" />
        <layout type=""log4net.Layout.PatternLayout"">
            <ConversionPattern value=""%date|%-5level|%message"" />
        </layout>
        <filter>
            <param name=""LevelMin"" value=""DEBUG"" />
            <param name=""LevelMax"" value=""FATAL"" />
        </filter>
    </appender>
    <logger name=""test"">
        <appender-ref ref=""SlackAppender"" />
    </logger>
</log4net>");

                XmlConfigurator.Configure(hierarchy, xml["log4net"]);

                var logger = LogManager.GetLogger(hierarchy.Name, "test");

                // It dosen't send log.
                logger.Info("log4net.invalidxml.test");
            }
        }

        [Fact]
        public void ValidXmlConfig_Tests()
        {
#if NETCOREAPP1_0 || NETCOREAPP1_1
            var hierarchy = LogManager.CreateRepository(GetType().GetTypeInfo().Assembly, typeof(Hierarchy));
#else
            var hierarchy = LogManager.CreateRepository(GetType().Assembly, typeof(Hierarchy));
#endif
            var xml = new XmlDocument();

            xml.LoadXml($@"
<log4net>
    <appender name=""SlackAppender"" type=""Log2Slack.Log4Net.SlackAppender, Log2Slack.Log4Net"">
        <param name=""WebHookUrl"" value=""{WebHookUrl}"" />
        <param name=""SerializerType"" value=""JsonHttpContentConverter.Jil.JilHttpContentConverter, JsonHttpContentConverter.Jil"" />
        <layout type=""log4net.Layout.PatternLayout"">
            <ConversionPattern value=""%date|%-5level|%message"" />
        </layout>
        <filter>
            <param name=""LevelMin"" value=""DEBUG"" />
            <param name=""LevelMax"" value=""FATAL"" />
        </filter>
    </appender>
    <logger name=""test"">
        <appender-ref ref=""SlackAppender"" />
    </logger>
</log4net>");

            XmlConfigurator.Configure(hierarchy, xml["log4net"]);

            var logger = LogManager.GetLogger(hierarchy.Name, "test");

            logger.Debug("log4net.validxml.debug");
            logger.Info("log4net.validxml.info");
            logger.Warn("log4net.validxml.warn");
            logger.Error("log4net.validxml.error");
            logger.Fatal("log4net.validxml.fatal");
        }

        [Fact]
        public void InvalidCodeConfig_Tests()
        {
            var layout = new PatternLayout
            {
                ConversionPattern = "%date|%-5level|%message",
            };

            layout.ActivateOptions();

            {
                var appender = new SlackAppender
                {
                    SerializerType = typeof(JilHttpContentConverter),
                    Layout = layout
                };

                Assert.Throws<LogException>(() => appender.ActivateOptions());
            }

            {
                var appender = new SlackAppender
                {
                    WebHookUrl = WebHookUrl,
                    Layout = layout
                };

                Assert.Throws<LogException>(() => appender.ActivateOptions());
            }

            {
                var appender = new SlackAppender
                {
                    WebHookUrl = WebHookUrl,
                    SerializerType = typeof(DummySerializer),
                    Layout = layout
                };

                Assert.Throws<LogException>(() => appender.ActivateOptions());
            }
        }

        [Fact]
        public void ValidCodeConfig_Tests()
        {
            var layout = new PatternLayout
            {
                ConversionPattern = "%date|%-5level|%message",
            };

            layout.ActivateOptions();

#if NETCOREAPP1_0 || NETCOREAPP1_1
            var repository = (Hierarchy)LogManager.CreateRepository(GetType().GetTypeInfo().Assembly, typeof(Hierarchy));
#else
            var repository = (Hierarchy)LogManager.CreateRepository(GetType().Assembly, typeof(Hierarchy));
#endif

            repository.Root.RemoveAllAppenders();

            var appender = new SlackAppender
            {
                WebHookUrl = WebHookUrl,
                SerializerType = typeof(JilHttpContentConverter),
                Layout = layout
            };

            appender.ActivateOptions();

            repository.Root.AddAppender(appender);

            BasicConfigurator.Configure(repository);

            var logger = LogManager.GetLogger(GetType());

            logger.Debug("log4net.validcode.debug");
            logger.Info("log4net.validcode.info");
            logger.Warn("log4net.validcode.warn");
            logger.Error("log4net.validcode.error");
            logger.Fatal("log4net.validcode.fatal");
        }
    }
}
