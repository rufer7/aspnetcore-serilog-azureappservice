# aspnetcore-serilog-azureappservice

[![License](https://img.shields.io/badge/license-Apache%20License%202.0-blue.svg)](https://github.com/rufer7/aspnetcore-serilog-azureappservice/blob/main/LICENSE)

Simple ASP.NET Core web API using Serilog to log to Azure Application Insights and App Service Log Stream

## Logging

The .NET Core web API in this repository uses `ILogger` of NuGet package `Microsoft.Extensions.Logging`. `ILogger<T>` is injected into the constructor of the class that needs to log.
The underlying logging framework is [Serilog](https://serilog.net/). Serilog is configured in `Program.cs` and `appsettings.json` of the application.

Serilog is configured so that the application logs to the following sinks:

- Startup logs are logged to the console
- Any other application logs are logged ...
  - ... to the console, if the application runs in local development environment (i.e. when started in `Visual Studio` or `Visual Studio Code` where `ASPNETCORE_ENVIRONMENT` is by default set to `Development`)
  - ... to Azure Application Insights, if the the application runs in Azure on an Azure App Service (where `ASPNETCORE_ENVIRONMENT` is by default set to `Production`, therefore `appsettings.Development.json` is ignored)
  - ... to a log file, if the application runs in Azure on an Azure App Service to allow viewing near-realtime logs in Azure App Service `Log stream`

> [!IMPORTANT]  
> In this sample Serilog is configured for Linux Azure App Services

Serilog creates traces in Application Insights (`TraceTelemetry`) for log levels `Information`, `Warning`, `Error` and `Fatal`. However, if the log event contains any exceptions it will always be sent as `ExceptionTelemetry` to Application Insights.
To make Serilog sending logs of log level `Debug` to Application Insights, the following application setting has to be added to the Azure App Service:

- Name: `Serilog__MinimumLevel__Default`
- Value: `Debug`

> [!IMPORTANT]
> This setting should only be added to the Azure App Service in case of troubleshooting. Otherwise, the logs will be flooded with debug logs.

### Azure Application Insights

To query the application logs in Azure Application Insights, proceed as follows:

1. Log in to the [Azure Portal](https://portal.azure.com/)
2. Switch to the corresponding directory (Azure tenant)
3. Search for `Application Insights` in the search bar on the top
4. Select the application insights resource the app runs on
5. Navigate to `Logs` in section `Monitoring` in the menu on the left
6. Enter one of the following queries in the query editor and click `Run`:
   - To query non exception logs
     ```
     traces
     | where message startswith "START_OF_LOG_MESSAGE"
     ```
   - To query exception logs
     ```
     exceptions
     | where message startswith "START_OF_LOG_MESSAGE"
     ```

### Stream logs

To stream the application logs in near-realtime, proceed as follows:

1. Log in to the [Azure Portal](https://portal.azure.com/)
2. Switch to the corresponding directory (Azure tenant)
3. Search for `App Service` in the search bar on the top
4. Select the app service resource the app runs on
5. Navigate to `App Service logs` in section `Monitoring` in the menu on the left
6. Switch `Application logging` to `File System`
7. Enter desired retention period in days
8. Click Save
9. Navigate to `Log stream` in section `Monitoring` in the menu on the left

## Links

- [Serilog](https://serilog.net/)
- https://learn.microsoft.com/en-us/azure/app-service/troubleshoot-diagnostic-logs
