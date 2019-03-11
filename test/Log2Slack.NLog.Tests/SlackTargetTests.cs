using JsonHttpContentConverter.Jil;
using NLog;
using NLog.Config;
using System.IO;
using System.Xml;
using Xunit;

namespace Log2Slack.NLog.Tests
{
    public class SlackTargetTests
    {
        private const string WebHookUrl = @"https://hooks.slack.com/services/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";

        [Fact]
        public void InvalidXmlConfig_Tests()
        {
            {
                var xml = @"
<nlog xmlns=""http://www.nlog-project.org/schemas/NLog.xsd""
    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
    xsi:schemaLocation=""http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd""
    autoReload=""true""
    throwExceptions=""true""
    internalLogLevel=""Debug"" internalLogFile=""c:\temp\nlog-internal.log"">

    <extensions>
        <add assembly=""Log2Slack.NLog"" />
    </extensions>
    
    <targets>
        <target name=""slack""
            xsi:type=""Slack""
            serializerType=""JsonHttpContentConverter.Jil.JilHttpContentConverter, JsonHttpContentConverter.Jil""
            layout=""${longdate}|${level}|${message}"" />
    </targets>

    <rules>
        <logger name=""*"" minlevel=""Debug"" writeTo=""slack"" />
    </rules>
    
</nlog>";

                using (var stringReader = new StringReader(xml))
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    var configuration = new XmlLoggingConfiguration(xmlReader, "nlog.config");

                    Assert.Throws<NLogConfigurationException>(() => new LogFactory(configuration));
                }
            }

            {
                var xml = $@"
<nlog xmlns=""http://www.nlog-project.org/schemas/NLog.xsd""
    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
    xsi:schemaLocation=""http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd""
    autoReload=""true""
    throwExceptions=""true""
    internalLogLevel=""Debug"" internalLogFile=""c:\temp\nlog-internal.log"">

    <extensions>
        <add assembly=""Log2Slack.NLog"" />
    </extensions>
    
    <targets>
        <target name=""slack""
            xsi:type=""Slack""
            webHookUrl=""{WebHookUrl}""
            layout=""${{longdate}}|${{level}}|${{message}}"" />
    </targets>

    <rules>
        <logger name=""*"" minlevel=""Debug"" writeTo=""slack"" />
    </rules>
    
</nlog>";

                using (var stringReader = new StringReader(xml))
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    var configuration = new XmlLoggingConfiguration(xmlReader, "nlog.config");

                    Assert.Throws<NLogConfigurationException>(() => new LogFactory(configuration));
                }
            }

            {
                var xml = $@"
<nlog xmlns=""http://www.nlog-project.org/schemas/NLog.xsd""
    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
    xsi:schemaLocation=""http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd""
    autoReload=""true""
    throwExceptions=""true""
    internalLogLevel=""Debug"" internalLogFile=""c:\temp\nlog-internal.log"">

    <extensions>
        <add assembly=""Log2Slack.NLog"" />
    </extensions>
    
    <targets>
        <target name=""slack""
            xsi:type=""Slack""
            webHookUrl=""{WebHookUrl}""
            serializerType=""Log2Slack.NLog.Tests.DummySerializer, Log2Slack.NLog.Tests""
            layout=""${{longdate}}|${{level}}|${{message}}"" />
    </targets>

    <rules>
        <logger name=""*"" minlevel=""Debug"" writeTo=""slack"" />
    </rules>
    
</nlog>";

                using (var stringReader = new StringReader(xml))
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    var configuration = new XmlLoggingConfiguration(xmlReader, "nlog.config");

                    Assert.Throws<NLogConfigurationException>(() => new LogFactory(configuration));
                }
            }

        }

        [Fact]
        public void ValidXmlConfig_Tests()
        {
            var xml = $@"
<nlog xmlns=""http://www.nlog-project.org/schemas/NLog.xsd""
    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
    xsi:schemaLocation=""http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd""
    autoReload=""true""
    throwExceptions=""true""
    internalLogLevel=""Debug"" internalLogFile=""c:\temp\nlog-internal.log"">

    <extensions>
        <add assembly=""Log2Slack.NLog"" />
    </extensions>
    
    <targets>
        <target name=""slack""
            xsi:type=""Slack""
            webHookUrl=""{WebHookUrl}""
            serializerType=""JsonHttpContentConverter.Jil.JilHttpContentConverter, JsonHttpContentConverter.Jil""
            layout=""${{longdate}}|${{level}}|${{message}}"" />
    </targets>

    <rules>
        <logger name=""*"" minlevel=""Debug"" writeTo=""slack"" />
    </rules>
    
</nlog>";

            using (var stringReader = new StringReader(xml))
            using (var xmlReader = XmlReader.Create(stringReader))
            {
                var configuration = new XmlLoggingConfiguration(xmlReader, "nlog.config");
                var factory = new LogFactory(configuration);
                var logger = factory.GetCurrentClassLogger();

                logger.Trace("nlog.validxml.trace");
                logger.Debug("nlog.validxml.debug");
                logger.Info("nlog.validxml.info");
                logger.Warn("nlog.validxml.warn");
                logger.Error("nlog.validxml.error");
                logger.Fatal("nlog.validxml.fatal");
            }
        }

        [Fact]
        public void InvalidCodeConfig_Tests()
        {
            {
                var configuration = new LoggingConfiguration();
                var target = new SlackTarget
                {
                    SerializerType = typeof(JilHttpContentConverter),
                    Layout = "${longdate}|${level}|${message}"
                };

                configuration.AddTarget("slack", target);
                configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));

                var factory = new LogFactory(configuration);
                var logger = factory.GetCurrentClassLogger();

                // It dosen't send log.
                logger.Info("nlog.invalidxml.test");
            }

            {
                var configuration = new LoggingConfiguration();
                var target = new SlackTarget
                {
                    WebHookUrl = WebHookUrl,
                    Layout = "${longdate}|${level}|${message}"
                };

                configuration.AddTarget("slack", target);
                configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));

                var factory = new LogFactory(configuration);
                var logger = factory.GetCurrentClassLogger();

                // It dosen't send log.
                logger.Info("nlog.invalidxml.test");
            }

            {
                var configuration = new LoggingConfiguration();
                var target = new SlackTarget
                {
                    WebHookUrl = WebHookUrl,
                    SerializerType = typeof(DummySerializer),
                    Layout = "${longdate}|${level}|${message}"
                };

                configuration.AddTarget("slack", target);
                configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));

                var factory = new LogFactory(configuration);
                var logger = factory.GetCurrentClassLogger();

                // It dosen't send log.
                logger.Info("nlog.invalidxml.test");
            }
        }

        [Fact]
        public void ValidCodeConfig_Tests()
        {
            var configuration = new LoggingConfiguration();
            var target = new SlackTarget
            {
                WebHookUrl = WebHookUrl,
                SerializerType = typeof(JilHttpContentConverter),
                Layout = "${longdate}|${level}|${message}"
            };

            configuration.AddTarget("slack", target);
            configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));

            var factory = new LogFactory(configuration);
            var logger = factory.GetCurrentClassLogger();

            logger.Trace("nlog.validcode.trace");
            logger.Debug("nlog.validcode.debug");
            logger.Info("nlog.validcode.info");
            logger.Warn("nlog.validcode.warn");
            logger.Error("nlog.validcode.error");
            logger.Fatal("nlog.validcode.fatal");
        }
    }
}
