# SecureVault



Secure IoT Home Alarm Monitoring Platform



SecureVault is a secure, web-based IoT platform for home alarm monitoring. The system receives data from simulated sensors, stores it securely, automatically generates alarms when values exceed defined thresholds, and displays everything in real time on a web-based dashboard.



This project is a final thesis within .NET development, with a focus on industrial IT security.



Contents:

* Features
* Tech Stack
* Architecture
* Getting Started
* Configuration
* Running Migrations
* Running the Application
* API Documentation
* Authentication
* Tests
* Project Structure
* Requirements Specification



Features:

* Secure user login via JWT
* Unique API key per sensor, hashed with BCrypt
* Support for 9 sensor types: motion, door, temperature, humidity, smoke, fire, carbon monoxide, water leak, and camera
* Automatic alarm generation based on per-sensor-type thresholds
* Real-time dashboard updates via SignalR
* Per-sensor history visualized as a line chart
* Create and delete sensors directly from the UI
* Full API documentation via Swagger/OpenAPI



Tech Stack

Layer	             Technology

Backend	             .NET 9 / C#, ASP.NET Core Web API

Frontend	     Blazor WebAssembly (Standalone)

Database	     PostgreSQL

ORM	             Entity Framework Core

Authentication	     JWT (users) + API key (sensors)

Real-time	     SignalR

Charts  	     Chart.js (via JavaScript interop)

API Documentation    Swagger / OpenAPI

Testing	             xUnit



Architecture



The solution consists of four projects:



SecureVault/

* SecureVault.API/       # Backend – Web API, authentication, alarm logic, SignalR hub
* SecureVault.Client/    # Frontend – Blazor WebAssembly application
* SecureVault.Shared/    # Shared DTOs and enums between API and Client
* SecureVault.Tests/     # xUnit tests for backend services



Getting Started

Prerequisites

.NET 9 SDK

PostgreSQL (local, or a connection to a hosted instance)

Visual Studio 2022 (recommended) or any IDE with .NET support

Clone the repository

bash

git clone https://github.com/aguara93/SecureVault.git

cd SecureVault

Configuration



Create/update appsettings.json in SecureVault.API with your own database connection and JWT key:



json

{

&#x20; "ConnectionStrings": {

&#x20;   "DefaultConnection": "Host=localhost;Database=SecureVault;Username=postgres;Password=YOUR\_PASSWORD"

&#x20; },

&#x20; "Jwt": {

&#x20;   "Key": "A\_LONG\_RANDOM\_KEY\_AT\_LEAST\_32\_CHARACTERS",

&#x20;   "Issuer": "SecureVaultAPI",

&#x20;   "Audience": "SecureVaultClient"

&#x20; }

}



This file is not committed to the repository for security reasons. Use appsettings.json as a template and fill in your own values.



Running Migrations



In the Package Manager Console, with SecureVault.API set as the default project:



powershell

Add-Migration InitialCreate

Update-Database



This creates the database and all tables (Sensors, SensorReadings, AlarmEvents, Users).



Running the Application



In Visual Studio:



Right-click the Solution → Set Startup Projects

Select Multiple startup projects, set both SecureVault.API and SecureVault.Client to Start

Press F5



The API starts at https://localhost:7061 and the client at https://localhost:7173.



API Documentation



While the API is running, open Swagger UI at:



https://localhost:7061/swagger



Swagger supports two authentication schemes:



Bearer (JWT) — for user-authenticated endpoints

ApiKey — for sensor data submission (X-Api-Key header)

Authentication



SecureVault uses two separate authentication flows:



Who	Method	Lifetime

Users	JWT (login with email + password)	15 minutes

Sensors	Unique API key, hashed with BCrypt	No expiration



Passwords and API keys are never stored in plain text — only hashed versions are persisted in the database.



Tests



The project includes unit tests for the backend's service logic. Run them via Test Explorer in Visual Studio, or:



bash

dotnet test

Project Structure

SecureVault.API/

├── Controllers/     # API endpoints (Auth, Sensors, SensorReadings, AlarmEvents)

├── Models/          # Database models (EF Core)

├── Services/        # Business logic (alarm evaluation, API key handling)

├── Attributes/       # ApiKeyAuthAttribute – sensor authentication

├── Hubs/             # SignalR hub for real-time updates

├── Data/             # EF Core DbContext

└── Migrations/       # Database migrations



SecureVault.Client/

├── Pages/            # Razor pages (Dashboard, Login, CreateSensor, SensorDetails)

├── Layout/           # Navigation menu

├── Services/         # HTTP and SignalR services communicating with the API

└── wwwroot/          # Static files, including Chart.js integration



SecureVault.Shared/

├── DTOs/             # Data Transfer Objects

└── Enums/            # SensorType, SensorStatus, AlarmStatus



SecureVault.Tests/

└── \*.cs              # xUnit tests

Requirements Specification



See the project's final report for the full requirements specification following the MoSCoW method, and the status of each requirement at project completion.



License



This is a school project developed as part of a vocational higher education (YH) program and is not intended for production use in its current form.

