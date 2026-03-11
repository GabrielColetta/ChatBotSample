# SampleAI - Project Documentation

## 🚀 Overview
**SampleAI** is a modern, full-stack AI-powered chat application built with **.NET 10** and **Angular 19**. It features a distributed architecture orchestrated by **.NET Aspire**, supporting real-time streaming chat using **Ollama** and **SignalR**.

---

## 🏗️ Architecture & Design Patterns

### Architectural Style
The project follows **Clean Architecture** principles, separating concerns into distinct layers:
- **Presentation:** `SampleAI.Api` (REST/SignalR), `SampleAI.UI` (Angular), `SampleAI.App` (MAUI Blazor Hybrid).
- **Application:** `SampleAI.Application` (Business Logic & Services).
- **Infrastructure:** `SampleAI.Infrastructure.*` (MongoDB, SignalR Client, HTTP).
- **Domain:** `SampleAI.Domain` (Core entities and logic).
- **Cross-Cutting:** `SampleAI.Shared` (Models, Constants) and `SampleAI.IoC` (DI Configuration).

### Key Design Patterns
- **Producer-Consumer Pattern:** Implemented using `System.Threading.Channels` and `BackgroundService` (`ChatProcessorWorker`) for asynchronous, resilient chat message processing.
- **Service Pattern:** Encapsulation of business logic in services like `ChatService`.
- **Repository/Context Abstraction:** `IDatabaseContext` (implemented via `MongoContext`) abstracts data persistence.
- **Real-time Streaming:** Extensive use of `IAsyncEnumerable` for end-to-end streaming from the AI model to the frontend via SignalR.
- **Options Pattern:** Type-safe configuration management using `IOptions<T>`.
- **Dependency Injection:** Centralized registration in `SampleAI.IoC` to maintain a decoupled architecture.
- **Orchestration:** **.NET Aspire** manages service discovery, lifecycle, and local infrastructure (MongoDB, Ollama).

---

## 🛠️ Technology Stack

### Backend (.NET 10)
- **Framework:** ASP.NET Core API.
- **Orchestration:** .NET Aspire (`SampleAI.AppHost`).
- **AI Integration:** `Microsoft.Extensions.AI` (standardized abstraction) and `OllamaSharp`.
- **Real-time:** SignalR for full-duplex communication.
- **Database:** MongoDB (`Aspire.MongoDB.Driver`).
- **Resilience:** Background processing with `System.Threading.Channels`.
- **API Documentation:** Microsoft OpenApi (Swagger).
- **Health Checks:** Native ASP.NET Core Health Checks for MongoDB and AI services.

### Frontend
- **Web UI:** Angular 19 + PrimeNG + PrimeIcons + FontAwesome.
- **Mobile/Desktop Hybrid:** .NET MAUI with Blazor Hybrid + Blazor Bootstrap.
- **Real-time Client:** `@microsoft/signalr`.

### Testing
- **Framework:** xUnit.
- **Mocking:** NSubstitute.
- **Assertions:** FluentAssertions.
- **Data Generation:** AutoFixture.
- **Integration Testing:** `Microsoft.AspNetCore.Mvc.Testing` and **Testcontainers** (MongoDB).

---

## 📂 Project Structure

- `src/SampleAI.Api`: Web API and SignalR Hub.
- `src/SampleAI.App`: MAUI Blazor Hybrid application (Android, iOS, Windows, Mac).
- `src/SampleAI.AppHost`: Aspire Orchestrator.
- `src/SampleAI.Application`: Core business services (Chat generation logic).
- `src/SampleAI.Domain`: Domain models and interfaces.
- `src/SampleAI.Infrastructure.MongoDB`: Data access implementation.
- `src/SampleAI.Infrastructure.SignalR`: Real-time communication services.
- `src/SampleAI.IoC`: Dependency Injection registry.
- `src/SampleAI.Shared`: Shared contracts, models, and constants.
- `src/SampleAI.UI`: Angular frontend application.
- `tests/SampleAI.Api.Tests`: Integration and unit tests.

---

## 🚦 Getting Started
1. Ensure **Docker Desktop** is running (for MongoDB/Aspire).
2. Install **Ollama** and ensure the desired model is available.
3. Run the `SampleAI.AppHost` project to start the entire ecosystem.

---

## 🛠️ Development & Quality Mandates

### 🧪 Testing & Validation
- **Mandatory Verification:** For every code change, **all existing tests must be executed** to ensure no regressions and guarantee that the system remains fully functional.
- **Always Verify Build and Tests:** Every time C# code is changed, you **must run `dotnet run` and `dotnet test`** to ensure the application starts correctly and all verification logic passes. Additionally, for any changes to component files in the `SampleAI.UI` project, you **must run `npm test`**.
- **Unit Testing for Use Cases:** Every new use case or business logic modification within the `SampleAI.Application` or `SampleAI.Domain` layers **must be accompanied by corresponding unit tests**.
- **Test-Driven Standards:** Use existing patterns (NSubstitute for mocking, FluentAssertions for clarity) to maintain the quality and reliability of the codebase.
