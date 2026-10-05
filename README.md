# Auth System

A simple authentication API built with C# and ASP.NET Core.

The project focuses on implementing authentication and authorization from the ground up using ASP.NET Core Identity, JWT and refresh tokens.

## Stack

* C#
* ASP.NET Core
* ASP.NET Core Identity
* Entity Framework Core
* SQLite
* JWT
* AutoMapper

## Features

* User registration and login
* Password hashing with ASP.NET Core Identity
* JWT access tokens
* Refresh tokens
* Refresh token rotation
* Token revocation
* Role-based authorization
* `User` and `Admin` roles
* Automatic role seeding
* Bootstrap admin account

## Authentication

The API uses JWT Bearer authentication.

After logging in, the user receives an access token and a refresh token. The access token is used to access protected endpoints, while the refresh token can be used to generate a new token pair when the access token expires.

```text
Login
  ↓
Validate credentials
  ↓
Access Token + Refresh Token
  ↓
Authenticated requests
```

Refresh tokens are stored and validated by the application, allowing revoked or expired tokens to be rejected.

## Roles

Authorization is handled using ASP.NET Core Identity roles.

The application currently has two roles:

* `User`
* `Admin`

Roles are created automatically when the application starts.

Endpoints can then be protected using ASP.NET Core's authorization attributes:

```csharp
[Authorize]
```

or:

```csharp
[Authorize(Roles = "Admin")]
```

## Running locally

Clone the repository:

```bash
git clone https://github.com/panicq-dev/auth-system.git
cd auth-system
```

Restore the dependencies:

```bash
dotnet restore
```

Apply the database migrations:

```bash
dotnet ef database update
```

Then run the API:

```bash
dotnet run
```

## Configuration

The application requires JWT configuration and can optionally create an initial administrator account.

Example:

```json
{
  "JWTTokenConfiguration": {
    "Issuer": "your-issuer",
    "Audience": "your-audience"
  },
  "JWTKey": {
    "key": "your-secret-key"
  },
  "BootstrapAdmin": {
    "UserName": "admin",
    "Password": "your-password"
  }
}
```

For anything beyond local development, secrets should be stored outside the repository.

## Project Structure

```text
Auth-System
├── Controllers
├── Data
├── Models
├── Services
├── Migrations
├── Program.cs
└── appsettings.json
```

## Purpose

This is a learning project focused on understanding how authentication works in a .NET backend, especially the interaction between Identity, JWT, refresh tokens and authorization.

The project is being built incrementally as new authentication and security concepts are implemented.
