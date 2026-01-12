# EduVerse Examination System API

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=JSON%20web%20tokens)

A comprehensive RESTful API for online examination and course management system built with ASP.NET Core 8

[Features](#-features) • [Tech Stack](#-tech-stack) • [Getting Started](#-getting-started) • [API Documentation](#-api-documentation)

</div>

---

## 📋 Overview

EduVerse Examination System is a robust and scalable web API designed to facilitate the complete lifecycle of online education - from course creation and student enrollment to exam administration and automated grading.

### Key Capabilities

- **Course Management**: Create, update, and manage courses with instructor assignments
- **Student Enrollment**: Enroll students in courses and track their progress
- **Exam Creation**: Build comprehensive exams with multiple question types (MCQ, True/False, Essay)
- **Online Testing**: Students can take exams with time tracking and auto-submission
- **Automated Grading**: Instant grading for objective questions with manual review for subjective answers
- **Assignment Management**: Create and submit assignments with file attachments
- **Social Learning**: Timeline posts, comments, and course reviews
- **Analytics & Reporting**: Comprehensive exam results and performance metrics

---

## 🛠️ Tech Stack

### Core Technologies

| Technology | Version | Purpose |
|-----------|---------|---------|
| **ASP.NET Core** | 8.0 | Web API Framework |
| **C#** | 12.0 | Programming Language |
| **Entity Framework Core** | 8.0.7 | ORM (Object-Relational Mapping) |
| **MS SQL Server** | 2019+ | Relational Database |
| **ASP.NET Core Identity** | 8.0.7 | Authentication & User Management |

### Key Libraries

- **JWT (JSON Web Tokens)** - Stateless authentication
- **Autofac** (8.0.0) - Advanced IoC container
- **AutoMapper** (13.0.1) - Object-to-object mapping
- **Swagger/OpenAPI** - API documentation

---

## 🏗️ Architecture & Design Patterns

The project follows **Clean Architecture** principles with:

### Design Patterns Used

**1. Repository Pattern**
- Abstracts data access layer
- Centralized query logic
- Easier unit testing

**2. CQRS (Command Query Responsibility Segregation)**
- Separates read and write operations
- Commands for state changes, Queries for data retrieval

**3. Service Layer Pattern**
- Business logic encapsulation
- Keeps controllers thin

**4. DTO Pattern**
- API contract separation from domain models
- Enhanced security and versioning

**5. Dependency Injection**
- Loose coupling via Autofac
- Testable and maintainable code

---

## 📁 Project Structure

```
EduVerseWebAPI/
│
├── Controllers/              # API Endpoints
├── Models/                   # Domain Entities
├── Data/                     # Database Context
├── Configurations/           # EF Core Fluent API
├── Repositories/             # Data Access Layer
├── Services/                 # Business Logic Layer
├── Mediators/                # CQRS Implementation
├── DTO/                      # Data Transfer Objects
├── ViewModels/               # API Response Models
├── Middlewares/              # Custom Middleware
├── Helpers/                  # Utility Classes
├── Profiles/                 # AutoMapper Profiles
├── Enums/                    # Enumeration Types
├── Exceptions/               # Custom Exceptions
├── Migrations/               # EF Core Migrations
├── AutoFacModule.cs          # DI Configuration
└── Program.cs                # Entry Point
```

---

## 🚀 Getting Started

### Prerequisites

- **Visual Studio 2022** (latest version) or **VS Code**
- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server 2019+** - [Download](https://www.microsoft.com/sql-server/sql-server-downloads)
- **SSMS** - Optional but recommended

### Installation Steps

#### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/EduVerseWebAPI.git
cd EduVerseWebAPI
```

#### 2. Configure Database Connection

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=EduVerseDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**Alternative with SQL Authentication:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=EduVerseDB;User ID=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True"
  }
}
```

#### 3. Configure JWT Authentication

Update JWT settings in `appsettings.json`:

```json
{
  "JWT": {
    "ValidIssuer": "https://localhost:7001",
    "ValidAudiance": "https://localhost:4200",
    "Key": "YOUR_SECRET_KEY_MUST_BE_AT_LEAST_32_CHARACTERS_LONG",
    "DurationInMinutes": 60
  }
}
```

**Generate a secure key at:** https://8gwifi.org/jwsgen.jsp

#### 4. Install NuGet Packages

```bash
dotnet restore
```

**Or manually via Package Manager Console:**
```powershell
Install-Package Microsoft.EntityFrameworkCore -Version 8.0.7
Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 8.0.7
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 8.0.7
Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore -Version 8.0.7
Install-Package Microsoft.AspNetCore.Authentication.JwtBearer -Version 8.0.7
Install-Package AutoMapper.Extensions.Microsoft.DependencyInjection -Version 13.0.1
Install-Package Autofac -Version 8.0.0
Install-Package Autofac.Extensions.DependencyInjection -Version 9.0.0
Install-Package Swashbuckle.AspNetCore -Version 6.6.2
```

#### 5. Apply Database Migrations

```bash
dotnet ef database update
```

**Using Package Manager Console:**
```powershell
Update-Database
```

#### 6. Run the Application

```bash
dotnet run
```

**Or press F5 in Visual Studio**

The API will be available at `https://localhost:7xxx` (check console for exact port)

---

## 📚 API Documentation

**Swagger UI:** `https://localhost:PORT/swagger`

### Authentication Flow

**1. Register:**
```http
POST /Account/Register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123!",
  "confirmPassword": "SecurePassword123!",
  "role": "Student"
}
```

**2. Login:**
```http
POST /Account/Login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```

**3. Use Token:**
```http
GET /Course/GetAll
Authorization: Bearer YOUR_JWT_TOKEN
```

### Main Endpoints

| Endpoint | Method | Description | Auth |
|----------|--------|-------------|------|
| `/Account/Register` | POST | Register user | ❌ |
| `/Account/Login` | POST | User login | ❌ |
| `/Course/GetAll` | GET | Get courses | ✅ |
| `/Course/Create` | POST | Create course | ✅ Instructor |
| `/Exam/Create` | POST | Create exam | ✅ Instructor |
| `/Exam/TakeExam/{id}` | GET | Start exam | ✅ Student |
| `/Exam/SubmitExam` | POST | Submit answers | ✅ Student |

---

## 🗄️ Database Schema

### Main Entities

- **Users** (Student, Instructor) - TPH inheritance
- **Courses** - Course information
- **Exams** - Exam details and timing
- **Questions** - Question bank (MCQ, TrueFalse, Essay)
- **Choices** - Multiple choice options
- **ExamQuestions** - Exam-Question relationship
- **ExamStudents** - Student enrollments
- **ExamAnswers** - Student submissions
- **Assignments** - Assignment details
- **AssignmentSubmissions** - Student submissions
- **CourseStudents** - Course enrollments
- **TimelineItems** - Course posts
- **Comments** - Post comments
- **CourseReviews** - Student ratings

---

## 👥 User Roles

### Student
✅ View enrolled courses  
✅ Take exams  
✅ Submit assignments  
✅ View results  
✅ Post reviews  

### Instructor
✅ Create courses & exams  
✅ Grade subjective answers  
✅ View all results  
✅ Manage timeline  

---

## 🔐 Security Features

- JWT Authentication with refresh tokens
- Role-based Authorization
- Password Hashing (ASP.NET Identity)
- SQL Injection Prevention (EF Core)
- HTTPS Enforcement
- Soft Delete pattern

---

## 📧 Contact

**Project Link:** [https://github.com/MinaMagdy484/EduVerseWebAPI](https://github.com/yourusername/EduVerseWebAPI)

---

<div align="center">

**Built with ❤️ using ASP.NET Core 8**

⭐ Star this repo if you find it helpful!

</div>
