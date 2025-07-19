# 🧠 Heavy String Filter API

A scalable, performant, and observable string filtering API built with **.NET 9**.
This project was developed as part of a technical task for **PASHA Insurance**.

---

## 📌 Purpose

The system accepts large-scale text data (up to 100MB), splits it into chunks, filters out undesirable words based on string similarity (Jaro-Winkler algorithm), and returns the cleaned content. It ensures:

* Efficient memory usage during processing
* Async-safe chunk upload support
* Background task-based filtering
* Tracing and structured logging
* High testability and performance insight

---

## ⚙️ Technologies Used

* **.NET 9** (ASP.NET Core Web API)
* **Serilog** (Logging to Console & Seq)
* **OpenTelemetry** (Distributed tracing via Jaeger)
* **FluentValidation** (Request validation)
* **xUnit** (Unit testing)
* **BenchmarkDotNet** (Performance testing)
* **Docker & Docker Compose** (For observability infrastructure)

---

## 🚀 Getting Started

### 🛠 Prerequisites

* [.NET 9 SDK (preview or stable)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
* [Docker Desktop](https://www.docker.com/)
* Optional: [Seq](https://datalust.co/seq) & [Jaeger](https://www.jaegertracing.io/)

---

### 🧪 Run the Application Locally

```bash
dotnet restore
dotnet run --project src/HeavyStringFilter.Api
```

---

### 🐳 Run Observability Stack

#### ▶️ Run Jaeger for Distributed Tracing

```bash
docker run -d --name jaeger \
  -e COLLECTOR_ZIPKIN_HOST_PORT=:9411 \
  -p 6831:6831/udp \
  -p 16686:16686 \
  jaegertracing/all-in-one:1.50
```

📍 Access: [http://localhost:16686](http://localhost:16686)

---

#### ▶️ Run Seq for Structured Logs

```bash
docker run -d --name seq \
  -e ACCEPT_EULA=Y \
  -p 5341:80 \
  datalust/seq
```

📍 Access: [http://localhost:5341](http://localhost:5341)

---

## 🔌 API Endpoints

| Method | Route                | Description                     |
| ------ | -------------------- | ------------------------------- |
| `POST` | `/upload`      | Uploads a chunk of text         |

---

## 📦 Project Structure

```
HeavyStringFilter/
│
├── src/
│   ├── HeavyStringFilter.Api              # API entry point
│   ├── HeavyStringFilter.Application      # Core business logic
│   ├── HeavyStringFilter.Infrastructure   # Workers, filters, queue, logging
│
├── tests/
│   ├── HeavyStringFilter.Tests            # Unit tests
│   ├── HeavyStringFilter.IntegrationTests # API behavior & validation tests
│   ├── HeavyStringFilter.PerformanceTests # BenchmarkDotNet tests
│   └── TestCommon                         # Reusable test components
```

---

## ✅ Features

* ✅ Upload text in distributed chunks
* ✅ Background queue processing
* ✅ Similarity filtering via Jaro-Winkler
* ✅ Serilog integration with Console & Seq
* ✅ OpenTelemetry-based distributed tracing
* ✅ FluentValidation for request validation
* ✅ Docker support for logging/tracing infrastructure
* ✅ BenchmarkDotNet performance profiling


---

## 🧪 Running Tests

```bash
dotnet test
```

### Performance Testing:

```bash
cd tests/HeavyStringFilter.PerformanceTests
dotnet run -c Release
```

---

## 🐳 Docker Compose (Optional)

To run the API along with Seq and Jaeger:

```bash
docker compose up --build
```

Make sure `docker-compose.yml` includes:

* `heavy-string-filter-api` service (using `.NET 9 SDK`)
* `seq` service on port `5341`
* `jaeger` service on ports `6831/udp`, `16686`

---

## 🙋 Contact

**Author:** Shahriyar Safarov

**Task:** PASHA Insurance Technical Task

**Contact:** [shahriyar.safarov.1995@gmail.com](mailto:shahriyar.safarov.1995@gmail.com)

---
