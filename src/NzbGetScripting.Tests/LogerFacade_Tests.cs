using System;
using Xunit;
using NSubstitute;
using Microsoft.Extensions.Logging;
using NzbGetScripting.Logging;
using System.Diagnostics;

namespace NzbGetScripting.Tests
{
    public class LoggerFacadeTests
    {
        [Fact]
        public void Ctor_with_ILogger()
        {
            // Arrange
            var logger = Setup();

            // Act
            var sut = new LoggerFacade(logger);
            sut.Log(LogLevel.Debug, "Test");

            // Asset
            logger.LogReceived();

            Assert.NotNull(sut.Stopwatch);

            // Local function to set up everything
            ILogger Setup()
            {
                return Substitute.For<ILogger>();
            }
        }

        [Fact]
        public void Ctor_ILogger_and_Stopwatch()
        {
            // Arrange
            var (
                logger,
                timer
            ) = Setup();

            // Act
            var sut = new LoggerFacade(logger, timer);
            sut.Log(LogLevel.Debug, "Test");

            // Asset
            logger.LogReceived();

            Assert.Equal(timer, sut.Stopwatch);

            // Local function to set up everything
            (ILogger, Stopwatch) Setup()
            {
                return (
                    Substitute.For<ILogger>(),
                    Stopwatch.StartNew()
                );
            }
        }

        [Fact]
        public void Log_calls_ILogger_Log_with_expected_args()
        {
            // Arrange
            var (
                logger,
                logLevel,
                ex,
                eventId,
                format,
                args,
                sut
            ) = Setup();

            // Act
            sut.Log(logLevel, eventId, ex, format, args);

            // Assert
            logger.LogReceived(
                    logLevel,
                    eventId,
                    args);

            // Local function to set up everything
            (ILogger, LogLevel, Exception, int, string, object[], LoggerFacade) Setup()
            {
                var lgr = Substitute.For<ILogger>();
                var tmr = Stopwatch.StartNew();

                return (
                    lgr,
                    LogLevel.Critical,
                    new Exception("Test Exception"),
                    666,
                    "Testing {0}",
                    new string[] { "System Under Test" },
                    new LoggerFacade(lgr, tmr)
                );
            }
        }
    }
}
