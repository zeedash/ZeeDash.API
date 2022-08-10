namespace ZeeDash.API.GraphQLServer;

/// <summary>
/// <see cref="ILogger"/> extension methods. Helps log messages using strongly typing and source generators.
/// </summary>
internal static partial class LoggerExtensions {

    [LoggerMessage(
        EventId = 5010,
        Level = LogLevel.Information,
        Message = "Started {Application} in {Environment} mode with runtime {Runtime} and OS {OperatingSystem}.")]
    public static partial void ApplicationStarted(
        this ILogger logger,
        string application,
        string environment,
        string runtime,
        string operatingSystem);

    [LoggerMessage(
        EventId = 5011,
        Level = LogLevel.Information,
        Message = "Stopped {Application} in {Environment} mode with runtime {Runtime} and OS {OperatingSystem}.")]
    public static partial void ApplicationStopped(
        this ILogger logger,
        string application,
        string environment,
        string runtime,
        string operatingSystem);

    [LoggerMessage(
        EventId = 5012,
        Level = LogLevel.Information,
        Message = "Connection {Application} in {Environment} to Orleans Cluster {ClusterId} with for service {ServiceId}.")]
    public static partial void SiloClusterConnecting(
        this ILogger logger,
        string application,
        string environment,
        string clusterId,
        string serviceId);

    [LoggerMessage(
        EventId = 5013,
        Level = LogLevel.Information,
        Message = "Connection succeed for {Application} in {Environment} to the Orleans Cluster {ClusterId} with for service {ServiceId}.")]
    public static partial void SiloClusterConnected(
        this ILogger logger,
        string application,
        string environment,
        string clusterId,
        string serviceId);

    [LoggerMessage(
        EventId = 5002,
        Level = LogLevel.Critical,
        Message = "{Application} terminated unexpectedly in {Environment} mode with runtime {Runtime} and OS {OperatingSystem}.")]
    public static partial void ApplicationTerminatedUnexpectedly(
        this ILogger logger,
        Exception exception,
        string application,
        string environment,
        string runtime,
        string operatingSystem);
}
