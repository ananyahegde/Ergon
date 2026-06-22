# Ergon

Ergon is a Human Resource Management System (HRMS) built with enterprise-grade practices in mind. It covers the core modules of workforce management — employee lifecycle, attendance, leave, payroll, and performance reviews — exposed through a clean REST API with role-based access control.

## Features

- Employee management with full profile, document storage, and bank account handling
- Role-based access control with four roles: HR Admin, HR, Manager, Employee
- Attendance tracking with clock-in/clock-out, late entry and early exit detection
- Leave management with application, approval workflow, and balance tracking
- Payroll generation with automatic salary calculation, tax deduction, and unpaid leave handling
- Performance review cycles with self-score and manager feedback
- In-app notifications triggered by system events
- JWT authentication with refresh token support
- File uploads with compression for images and PDFs

## Tech Stack

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8 with Npgsql (PostgreSQL)
- JWT Bearer Authentication with BCrypt password hashing
- AutoMapper for DTO mapping
- Serilog for structured logging
- SixLabors ImageSharp for image processing
- PdfSharpCore for PDF compression
- NUnit + Moq for unit testing
- Angular (frontend)

## Getting Started

### Prerequisites

- .NET 8 SDK
- PostgreSQL
- Node.js and Angular CLI

### Backend Setup

1. Clone the repository

```bash
git clone https://github.com/ananyahegde/Ergon.git
cd ergon
```

2. Configure environment — create `Ergon.API/appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ergondb;Username=youruser;Password=yourpassword"
  },
  "JWT": {
    "Key": "your-secret-key-minimum-32-characters",
    "Issuer": "Ergon",
    "DurationInMinutes": 60,
    "RefreshTokenExpiryInDays": 7
  }
}
```

3. Run migrations and start the API

```bash
cd Ergon.API
dotnet ef database update
dotnet run
```

API runs at the port shown in the terminal after `dotnet run`. Swagger available at `http://localhost:{port}/swagger`.

### Frontend Setup

```bash
cd ergon
npm install
ng serve
```

Frontend runs at `http://localhost:4200`.

### Running Tests

Run from the root directory:

```bash
dotnet test
```

## License

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Contributions

PRs and suggestions are welcome.
