namespace ZeeDash.API.IdentityServer;

using System.Runtime.InteropServices;
using Orleans.Configuration;

internal static class HostExtensions {

    public static void LogApplicationStarted(this IHost host) {
        var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.ApplicationStarted(
            hostEnvironment.ApplicationName,
            hostEnvironment.EnvironmentName,
            RuntimeInformation.FrameworkDescription,
            RuntimeInformation.OSDescription);
    }

    public static void LogApplicationStopped(this IHost host) {
        var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.ApplicationStopped(
            hostEnvironment.ApplicationName,
            hostEnvironment.EnvironmentName,
            RuntimeInformation.FrameworkDescription,
            RuntimeInformation.OSDescription);
    }

    public static void LogSiloClusterConnection(this IHost host) {
        var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var cluster = host.Services.GetRequiredService<ClusterOptions>();
        logger.SiloClusterConnecting(
            hostEnvironment.ApplicationName,
            hostEnvironment.EnvironmentName,
            cluster.ClusterId,
            cluster.ServiceId);
    }

    public static void LogSiloClusterConnected(this IHost host) {
        var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var cluster = host.Services.GetRequiredService<ClusterOptions>();
        logger.SiloClusterConnected(
            hostEnvironment.ApplicationName,
            hostEnvironment.EnvironmentName,
            cluster.ClusterId,
            cluster.ServiceId);
    }

    public static void LogApplicationTerminatedUnexpectedly(this IHost host, Exception exception) {
        if (host is null) {
            LogToConsole(exception);
        } else {
            try {
                var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
                var logger = host.Services.GetRequiredService<ILogger<Program>>();
                logger.ApplicationTerminatedUnexpectedly(
                    exception,
                    hostEnvironment.ApplicationName,
                    hostEnvironment.EnvironmentName,
                    RuntimeInformation.FrameworkDescription,
                    RuntimeInformation.OSDescription);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                LogToConsole(exception);
            }
        }

        static void LogToConsole(Exception exception) {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{AssemblyInformation.Current.Product} terminated unexpectedly.");
            Console.WriteLine(exception.ToString());
            Console.ForegroundColor = foregroundColor;
        }
    }
}
