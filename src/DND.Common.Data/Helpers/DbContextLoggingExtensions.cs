﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DND.Common.Data.Helpers
{
    public enum LoggingCategories
    {
        All = 0,
        SQL = 1
    }

    public static class DbContextLoggingExtensions
    {
        public static void ConfigureLogging(this ILoggerFactory loggerFactory, Action<String> logger, Func<string, LogLevel, bool> filter)
        {
            LogProvider.CreateOrModifyLoggerForDbContext(loggerFactory, logger, filter);
        }

        public static void ConfigureLogging(this ILoggerFactory loggerFactory, Action<String> logger,
                                           LoggingCategories categories = LoggingCategories.All)
        {

            if (categories == LoggingCategories.SQL)
            {
                var SqlCategories = new List<string> { DbLoggerCategory.Database.Command.Name,
                                                       DbLoggerCategory.Query.Name,
                                                       DbLoggerCategory.Update.Name};
                LogProvider.CreateOrModifyLoggerForDbContext(loggerFactory,
                                                             logger,
                                                             (c, l) => SqlCategories.Contains(c) && (l == LogLevel.Information));
            }
            else if (categories == LoggingCategories.All)
            {
                LogProvider.CreateOrModifyLoggerForDbContext(loggerFactory, logger,
                                                             (c, l) => true);
            }
        }
    }
    class LogProvider : ILoggerProvider
    {

        //volatile to allow the configuration to be switched without locking
        public volatile LoggingConfiguration Configuration;
        static bool DefaultFilter(string CategoryName, LogLevel level) => true;

        static LogProvider provider = null;

        public static void CreateOrModifyLoggerForDbContext(ILoggerFactory loggerFactory,
                                                            Action<string> logger,
                                                            Func<string, LogLevel, bool> filter = null)
        {
            if(provider == null)
            {
                provider = new LogProvider(logger, filter ?? DefaultFilter);
                loggerFactory.AddProvider(provider);
            }
            else
            {
                provider.Configuration = new LoggingConfiguration(logger, filter ?? DefaultFilter);
            }
        }

        public class LoggingConfiguration
        {
            public LoggingConfiguration(Action<string> logger, Func<string, LogLevel, bool> filter)
            {
                this.logger = logger;
                this.filter = filter;
            }
            public readonly Action<string> logger;
            public readonly Func<string, LogLevel, bool> filter;
        }


        private LogProvider(Action<string> logger, Func<string, LogLevel, bool> filter)
        {
            this.Configuration = new LoggingConfiguration(logger, filter);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(categoryName, this);
        }

        public void Dispose()
        { }

        private class Logger : ILogger
        {

            readonly string categoryName;
            readonly LogProvider provider;
            public Logger(string categoryName, LogProvider provider)
            {
                this.provider = provider;
                this.categoryName = categoryName;
            }
            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                                    Exception exception, Func<TState, Exception, string> formatter)
            {
                //grab a reference to the current logger settings for consistency, 
                //and to eliminate the need to block a thread reconfiguring the logger
                var config = provider.Configuration;
                if (config.filter(categoryName, logLevel))
                {
                    config.logger(formatter(state, exception));
                }
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }
    }
}