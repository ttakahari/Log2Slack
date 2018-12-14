# Log2Slack

Extensions that sends logs to slack with loggers.

Supported loggers are [NLog](http://nlog-project.org/) and [log4net](https://logging.apache.org/log4net/).

[![AppVeyor](https://img.shields.io/appveyor/ci/gruntjs/grunt.svg?style=plastic)](https://ci.appveyor.com/project/ttakahari/Log2Slack)
[![NuGet](https://img.shields.io/nuget/v/Log2Slack.NLog.svg?style=plastic)](https://www.nuget.org/packages/Log2Slack.NLog/)
[![NuGet](https://img.shields.io/nuget/v/Log2Slack.Log4Net.svg?style=plastic)](https://www.nuget.org/packages/Log2Slack.Log4Net/)

## Install

from NuGet - [Log2Slack.NLog](https://www.nuget.org/packages/Log2Slack.NLog/)
from NuGet - [Log2Slack.Log4Net](https://www.nuget.org/packages/Log2Slack.Log4Net/)

```ps1
PM > Install-Package Log2Slack.NLog
PM > Install-Package Log2Slack.Log4Net
```

## How to use

### NLog

If you use a XML file, configure as follow.

```xml
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xsi:schemaLocation="http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd"
    autoReload="true"
    throwExceptions="true">

    <!--add extension assembly.-->
    <extensions>
        <add assembly="Log2Slack.NLog" />
    </extensions>
    
    <targets>
        <!--
            add target with configuring as follow.
                xsi:type: Slack
                webHookUrl: your Incoming-WebHook URL.
                serializerType: full name and assembly name of a serializer impletemting SlackSharp.IHttpContentJsonSerializer.
        -->
        <target name="slack"
            xsi:type="Slack"
            webHookUrl="https://hooks.slack.com/services/xxx"
            serializerType="SlackSharp.Serialization.Jil.JilSerializer, SlackSharp.Serialization.Jil"
            layout="${message}" />
    </targets>

    <rules>
        <logger name="*" minlevel="Debug" writeTo="slack" />
    </rules>
    
</nlog>
```

If you write codes instead of a XML file, configure as follow.

```csharp
// create an instance of NLog.Config.LoggingConfiguration.
var configuration = new LoggingConfiguration();

// create an instance of Log2Slack.NLog.SlackTarget.
var target = new SlackTarget
{
    WebHookUrl = "https://hooks.slack.com/services/xxx", // your Incoming-WebHook URL.
    SerializerType = typeof(SlackSharp.Serialization.Jil.JilSerializer), // type of a serializer impletemting SlackSharp.IHttpContentJsonSerializer.
    Layout = "${message}" // log content layout
};

// add the target with a name.
configuration.AddTarget("slack", target);

// according to the rules of NLog.
```

### log4net

If you use a XML file, configure as follow.

```xml
<log4net>

    <!--
        add appender with configuring as follow.
            appender-type: Log2Slack.Log4Net.SlackAppender, Log2Slack.Log4Net
            param WebHookUrl: Your Incoming-WebHook URL.
            param SerializerType: Full name and assembly name of a serializer impletemting SlackSharp.IHttpContentJsonSerializer.
    -->
    <appender name="SlackAppender" type="Log2Slack.Log4Net.SlackAppender, Log2Slack.Log4Net">
        <param name="WebHookUrl" value="{WebHookUrl}" />
        <param name="SerializerType" value="SlackSharp.Serialization.Jil.JilSerializer, SlackSharp.Serialization.Jil" />
        <layout type="log4net.Layout.PatternLayout">
            <ConversionPattern value="%message" />
        </layout>
        <filter>
            <param name="LevelMin" value="DEBUG" />
            <param name="LevelMax" value="FATAL" />
        </filter>
    </appender>

    <logger name="test">
        <appender-ref ref="SlackAppender" />
    </logger>
    
</log4net>
```

If you write codes instead of a XML file, configure as follow.

```csharp
// create an instance of log4net.Layout.PatternLayout and call ActivateOptions method.
var layout = new PatternLayout
{
    ConversionPattern = "%message",
};

layout.ActivateOptions();

// create an instance of Log2Slack.Log4Net.SlackAppender and call ActivateOptions method.
var appender = new SlackAppender
{
    WebHookUrl = "https://hooks.slack.com/services/xxx", // your Incoming-WebHook URL.
    SerializerType = typeof(SlackSharp.Serialization.Jil.JilSerializer), // type of a serializer impletemting SlackSharp.IHttpContentJsonSerializer.
    Layout = layout // log content layout
};

appender.ActivateOptions();

// create an instance of log4nt.Repository.Hierarchy.Hierarchy and add the appender.
var repository = (Hierarchy)LogManager.CreateRepository(GetType().Assembly, typeof(Hierarchy));

repository.Root.AddAppender(appender);

// according to the rules of log4net.
```

## Lisence

under [MIT Lisence](https://opensource.org/licenses/MIT).
