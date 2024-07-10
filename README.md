# Payment Gateway Service

The Payment Gateway Service is the core service that processes payment requests. It calls the acquiring bank API to authorize the payment and stores the payment record in the database.

## Design

See [DESIGN.md](docs/DESIGN.md).

## Building and running

The Payment Gateway Service is an ASP.NET web application. To build and run the service, you need the [.NET Core SDK](https://dotnet.microsoft.com/download) installed.

Build and run the service within Visual Studio.

## Testing

### Unit tests

Unit tests are defined in [test/PaymentGateway.Api.Tests](test/PaymentGateway.Api.Tests/). They use the XUnit and Moq libraries. To run the unit tests, use the Visual Studio Test Explorer.

### Integrating with bank simulator

To run the bank simulator, run:

```bash
docker compose up
```

The service is configured to talk to the bank simulator when running in a development environment.

When run in a development environment, the service is exposed at `https://localhost:7092`. Swagger UI is available at `https://localhost:7092/swagger` with API documentation.

### Viewing metrics

The service exposes metrics in Prometheus format at `https://localhost:7092/metrics`. These can also be viewed using the [`dotnet-counters`](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-counters) tool:

```powershell
dotnet-counters monitor -n PaymentGateway.Api --counters Microsoft.AspNetCore.Hosting Payments
```
