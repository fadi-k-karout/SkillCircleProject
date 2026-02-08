# SkillCircle API 

A robust, scalable E-learning platform backend built with **.NET 8** following **Clean Architecture** principles. This project focuses on high maintainability, testability, and strict separation of concerns.

##  Architecture
The project is organized into four distinct layers:
- **Domain**: Core entities (Course, Review, Skill), enums, and enterprise-wide business rules.
- **Application**: Business logic, DTOs, Mappings (AutoMapper), Validators (FluentValidation), and Service interfaces.
- **Infrastructure**: Data persistence (EF Core), Repository implementations, Unit of Work, and external service integrations (Email, CDN).
- **Web API**: REST Controllers, Middlewares, Dependency Injection registration, and API configurations.

##  Tech Stack & Patterns
- **Framework**: .NET 8 Web API
- **Database**: SQL Server with Entity Framework Core
- **Security**: ASP.NET Core Identity with JWT (JSON Web Tokens)
- **Validation**: FluentValidation for request-level data integrity
- **Mapping**: AutoMapper for seamless DTO/Entity transitions
- **Patterns**: Repository Pattern & Unit of Work for decoupled data access
- **Testing**: xUnit & Moq for Unit Testing across Application and Infrastructure layers
- **Mailing**: Razor-based HTML email templates for user verification and password resets

##  Key Features
- **Course & Content Management**: Comprehensive CRUD for courses, skills, and video lessons.
- **Advanced Identity**: Secure registration and login with role-based and policy-based authorization (e.g., `OwnerOrAdmin`).
- **Media Integration**: Integration with CDN services for video content delivery.
- **Feedback System**: Integrated student reviews and course rating logic.
- **Payment Processing**: Data models and services for tracking student enrollments and payments.
- **Automated Emails**: System-generated emails using custom Razor templates for a professional user experience.

##  Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or Express)

### Installation
1. **Clone the repository**:
```bash
git clone https://github.com/fadi-k-karout/SkillCircleProject.git
```
2. **Update Database**: Apply the migrations to your local SQL Server instance:
```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```
3. **Run the application**:
```bash
dotnet run --project src/Web
```

## Testing
Run the comprehensive test suite to verify business logic:
```bash
dotnet test
```
