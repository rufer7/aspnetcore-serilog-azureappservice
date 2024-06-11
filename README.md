# aspnetcore-serilog-azureappservice

[![License](https://img.shields.io/badge/license-Apache%20License%202.0-blue.svg)](https://github.com/rufer7/aspnetcore-serilog-azureappservice/blob/main/LICENSE)

Simple ASP.NET Core Web API using Serilog to log to Azure Application Insights and App Service Log Stream

## Logging

The .NET Core Web API in this repository uses `ILogger` of NuGet package `Microsoft.Extensions.Logging`. `ILogger<T>` is injected into the constructor of the class that needs to log.
The underlying logging framework is [Serilog](https://serilog.net/). Serilog is configured in `Program.cs` and `appsettings.json` (`appsettings.Development.json` for local development environment) of the application.

Serilog is configured so that the application logs to the following sinks:

- Startup logs are logged to the console
- Any other application logs are logged ...
  - ... to the console, if the application runs in local development environment (i.e. when started in `Visual Studio` or `Visual Studio Code` where `ASPNETCORE_ENVIRONMENT` is by default set to `Development`)
  - ... to Azure Application Insights, if the the application runs in Azure on an Azure App Service (where `ASPNETCORE_ENVIRONMENT` is by default set to `Production`, therefore `appsettings.Development.json` is ignored)
  - ... to a log file, if the application runs in Azure on an Azure App Service to allow viewing near-realtime logs in Azure App Service `Log stream`

> [!IMPORTANT]  
> In this sample Serilog is configured for Linux Azure App Services

Serilog creates traces in Application Insights (`TraceTelemetry`) for log levels defined in `appsettings.json` `MinimumLevel`. However, if the log event contains any exceptions it will always be sent as `ExceptionTelemetry` to Application Insights.
To make Serilog sending custom application logs of log level `Debug` to Application Insights, the following application setting has to be added to the Azure App Service (see `Settings` > `Configuration` of App Service):

- Name: `Serilog__MinimumLevel__Default`
- Value: `Debug`

> [!IMPORTANT]
> This setting should only be added to the Azure App Service in case of troubleshooting. Otherwise, the logs will be flooded with debug logs.

### Smart Request Logging

The NuGet package [Serilog.AspNetCore](https://www.nuget.org/packages/Serilog.AspNetCore) includes middleware for request logging which can be configured to make logs and exceptions from .NET core middleware visible in the logs.

For more details see [here](https://github.com/serilog/serilog-aspnetcore?tab=readme-ov-file#request-logging)

### Azure Application Insights

To query the application logs in Azure Application Insights, proceed as follows:

1. Log in to the [Azure Portal](https://portal.azure.com/)
2. Switch to the corresponding directory (Azure tenant)
3. Search for `Application Insights` in the search bar on the top
4. Select the application insights resource the app sends its logs to
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
     ```

### Stream logs

> [!IMPORTANT]
> Make sure you check the whole stream for your logs as they seem to not be ordered by date but by log file

To stream the application logs in near-realtime, proceed as follows:

1. Log in to the [Azure Portal](https://portal.azure.com/)
2. Switch to the corresponding directory (Azure tenant)
3. Search for `App Service` in the search bar on the top
4. Select the app service resource the app runs on
5. Navigate to `Log stream` in section `Monitoring` in the menu on the left

## Deployment to Azure

### Prerequisites

- Azure tenant
- Azure subscription

### Step by Step Manual

1. Create a new resource group (i.e. `aspnetcore-serilog-azureappservice`)
1. Create a new workspace-based application insights resource in the before created resource group (i.e. `aspnetcore-serilog-azureappservice-appinsights`)
1. Create a new app service plan in the before created resource group (i.e. `aspnetcore-serilog-azureappservice-appplan`)

   - Operating system: `Linux`
   - Pricing tier: `F1`

1. Create a new web app (i.e. `aspnetcore-serilog-azureappservice-app`)

   - Publish: `Code`
   - Runtime stack: `.NET 8 (LTS)`
   - Operating system: `Linux`
   - Linux Plan: select before created app service plan
   - Enable and configure continuous deployment under `Deployment` tab

1. Ensure the following application setting is set in the Azure App Service:

   This setting is already set, if deployment is done using GitHub Actions integration.

   - Name: `APPLICATIONINSIGHTS_CONNECTION_STRING`
   - Value: connection string of the before created application insights resource

> [!IMPORTANT]
> Without this setting, the application will not log to Application Insights.

After successful deployment, the weather forecast API endpoint is available under `APP_SERVICE_DEFAULT_DOMAIN/WeatherForecast`

## Screenshots

### Console logs

![image](https://github.com/rufer7/aspnetcore-serilog-azureappservice/assets/5937292/8e993089-81f0-4b35-a261-01e5b333de09)

### Azure Application Insights

![image](https://github.com/rufer7/aspnetcore-serilog-azureappservice/assets/5937292/82ad950e-0668-4328-bd4b-a98c858293d9)

![image](https://github.com/rufer7/aspnetcore-serilog-azureappservice/assets/5937292/4fb0705f-dd0b-42e6-869d-309b16e1ffc1)

### Azure App Service Log Stream

![image](https://github.com/rufer7/aspnetcore-serilog-azureappservice/assets/5937292/69c87ce9-f772-49a0-af78-e541c75785d6)

## Links

- [Serilog](https://serilog.net/)
- https://learn.microsoft.com/en-us/azure/app-service/troubleshoot-diagnostic-logs
