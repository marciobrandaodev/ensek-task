# ENSEK API Solution

A lightweight REST API for processing energy meter readings and validating account data. Built with ASP.NET Core version 9.0. Designed for extensibility, testability, and containerized deployment via Docker.

---

## 🚀 Features

- Meter Reading Ingestion: Accepts bulk readings via CSV with validation.
- Account Validation Service: Ensures readings match known accounts.
- Simple Unit Tests: Basic Test Cases created using NUnit, Moq and Autofixture.
- Docker-Ready: Easily deployable with containerized setup.

---

## 🧱 Tech Stack

| Layer            | Tools / Frameworks               |
|------------------|----------------------------------|
| Backend API      | ASP.NET Core Web API in C#       |
| Data Access      | Entity Framework Core (SqlServer)|
| Validation       | Validation in CSV Service        |
| Testing          | NUnit, Moq, MockQueryable.Moq    |
| Containerization | Docker                           |

---

## ⚙️ Setup & Running Locally

### 1. Clone the Repo

```bash
git clone https://github.com/marciobrandaodev/ensek-task.git
```

### 2. Build & Run with Docker
Open command line in folder src/EnsekMeterReadingApi and run the following command ```docker-compose up -d --build```

The API will be available at:
http://localhost:5000
The URL to Post the CSV is http://localhost:5000/meter-reading-uploads. You can also access the endpoint through http://localhost:5000/api/v1/meterReadings/meter-reading-uploads as a personal preference.

SQL Server is reachable on localhost,1433, you can find username and password in the Dockerfile in folder src/EnsekMeterReadingApi

### 🛠️ Future Improvements
If I had more time, I would like to:
- Create a Validation Service, removing the scope from the CsvMeterReading Service
- Improve on Unit Tests, particularly around the Validation Service
- Use Automapper for mapping Model for MeterReadingDto
- Add Swagger / OpenAPI documentation
- Add Api Versioning
- Create a DbContext factory

### Additional Notes
- The CsvMeterReadingService contains the Interface, Implementation Class and also the MultiFormatDateTimeConverter in the same file.
- For Production code, I usually recommend dealling with EF Migrations outside of the API (manually). For this specific event I use the EnsureDeleted() methiod to help me test more often.