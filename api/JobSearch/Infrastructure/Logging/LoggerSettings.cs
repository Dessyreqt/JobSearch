namespace JobSearch.Infrastructure.Logging
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;
    using Serilog.Events;

    public class LoggerSettings
    {
        public LoggerSettings(IConfiguration config)
        {
            RollingLogFileEnabled = bool.Parse(config["Logging:RollingLogFileEnabled"] ?? "false");

            if (RollingLogFileEnabled)
            {
                if (bool.Parse(config["Logging:AutoGenerateRollingLogFilePath"]))
                {
                    var folder = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{nameof(JobSearch)}\\logs";
                    RollingLogFilePath = folder;
                }
                else
                {
                    var folder = config["Logging:RollingLogFilePath"];
                    RollingLogFilePath = folder;
                }

                var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                var fileName = $"{assemblyName}.Log_{{Date}}.txt";
                var fullFolderPath = Path.Combine(RollingLogFilePath, fileName);
                RollingLogFile = fullFolderPath;
            }

            if (Enum.TryParse<LogEventLevel>(config["Logging:MinimumLogEventLevel"], out var logEventLevel))
            {
                MinimumLogEventLevel = logEventLevel;
            }
            else
            {
                MinimumLogEventLevel = LogEventLevel.Information;
            }
        }

        public bool RollingLogFileEnabled { get; }
        public string RollingLogFilePath { get; }
        public string RollingLogFile { get; }
        public LogEventLevel MinimumLogEventLevel { get; }
    }
}
