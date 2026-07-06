# User Management API

A backend REST API built with ASP.NET Core, MongoDB, JWT authentication, and BCrypt password hashing.

This project provides authentication, user management, profile management, password reset, and role-based authorization APIs.

## Features

- User registration
- User login with JWT token
- JWT authentication and authorization
- Role-based access control
- Get all users
- Get user by ID
- Add user
- Update user
- Delete user
- User profile management
- Change password from profile
- Forgot password
- Reset password using a temporary reset token
- BCrypt password hashing
- MongoDB database integration
- Swagger API documentation
- Request logging middleware
- Input validation and error handling

## Tech Stack

- ASP.NET Core Web API
- C#
- MongoDB
- MongoDB.Driver
- JWT Bearer Authentication
- BCrypt.Net
- Swagger / Swashbuckle

## Project Structure

```text
UserApi/
│
├── Constants/
├── Controllers/
│   ├── AuthController.cs
│   ├── UsersController.cs
│   └── ProfileController.cs
│
├── Dtos/
│   ├── RegisterDto.cs
│   ├── LoginDto.cs
│   ├── ForgotPasswordDto.cs
│   ├── ResetPasswordDto.cs
│   └── UpdateProfileDto.cs
│
├── Middleware/
│   └── RequestLoggingMiddleware.cs
│
├── Models/
│   └── User.cs
│
├── Services/
│   ├── AuthService.cs
│   ├── JwtService.cs
│   └── UserService.cs
│
├── Program.cs
├── UserApi.csproj
├── appsettings.example.json
└── README.md
