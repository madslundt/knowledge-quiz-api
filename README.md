Running on .NET Core 2.2
 
 ## Structure
  - API: Source of the application.
  - DataModel: Models for the database/store.
  - UnitTest: Unit tests for the application.
  - IDP: Identity Provider using Identityserver4.

 ### API
 API is structured by having each feature in a single file. That gives the following structure:
  - Controllers: All the controllers with endpoints exposed.
  - Features: All features (eg. User/GetUser.cs).
  - Infrastructure: Infrastructure for the application it self (eg. Middlewares, Filters, Pipeline).
  - ThirdParty: Third party services (eg. Facebook login).

## Setting up application
The application require 2 databases - one for the application it self and one for Hangfire.
 1. Create a new appsettings to your *ASPNETCORE_ENVIRONMENT* (eg appsettings.Development.json) and add the 2 new connection strings for application and Hangfire.
 2. Run database changes to the application database by running the command `dotnet ef database update -s ../API` inside DataModel folder (see commands in *DataModel/DatabaseContext.cs*).
 3. Add connection string to the Identity server (IDP/appsettings.json or change environment and add a new appsettings). The database used for the application can also be used to the Identity server. Once the Identity server project is ran it will run the migrations for it.
 
## Setting up real time metrics
Real time metrics require Grafana and InfluxDb.
 1. Add InfluxDb options to appsettings.
 2. Download Grafana dashboard [here](https://grafana.com/dashboards/2125).
 
## Logging
Logging is set up with Microsoft.Extensions.Logging:
 - Status codes 5xx, that are caused by an exception, are logged as critical.
 - Other status codes, that are caused by an exception, are logged as warning.
 - The whole pipeline (request to response) is logged as information.

Critical and warning logs are named `<endpoint> :: [<status code>] <exception message>` and contain request, stacktrace and correlation id.

The user receives error message and correlation id in production. For development environment the whole 
exception is also included.

Sentry.io logging provider has been added to the project.

## Build and run with Docker
```
$ docker build -t aspnetapp .
$ docker run -d -p 8080:80 --name myapp aspnetapp
```
