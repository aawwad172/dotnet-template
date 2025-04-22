# Dotnet Template

This is a .NET template built using Domain-Driven Design (DDD) with Clean Architecture. It is designed to help you quickly set up an application with the following layers and patterns:

- **Domain:** Contains your core business entities and logic.
- **Application:** Houses use cases, CQRS command and query handlers, and application-specific services.
- **Infrastructure:** Implements data access, repositories, UnitOfWork, and integration with external services (e.g., Postgres).
- **Presentation:** Contains the Web API project, middleware (including JWT authentication, exception handling), and front-facing controllers.

## Features

- **Clean Architecture with DDD:**  
  Organizes your solution into Domain, Application, Infrastructure, and Presentation projects.
  
- **Dependency Injection:**  
  DI is set up for each project, ensuring loose coupling and easier testing.

- **CQRS and UnitOfWork:**  
  Implements the CQRS pattern for separating commands and queries. Also includes a UnitOfWork pattern to manage transactions.

- **Repository Patterns:**  
  - A **Generic Repository** is provided for general CRUD operations.
  - For operations with specific needs, you can create a custom repository by inheriting from the generic repository and overriding the necessary methods.

- **JWT Middleware and Service:**  
  Provides a built-in JWT middleware and a dedicated JWT service for handling authentication and authorization.  
  **Important:** Make sure to change the variables in your `appsettings.json` (connection string, JWT secret, issuer, audience, and **EncryptionKey**) to match your own configuration.

- **Postgres Integration:**  
  The template is built on PostgreSQL. Connection strings and related configurations are set up accordingly.

- **Health Endpoint:**  
  A health endpoint is available to check both the database connection and server health.

- **Husky for Pre-Commit Hooks:**  
  Husky is configured to ensure code consistency before committing.  
  **Note:** Install Husky via `npx` (e.g., `npx husky-init && npm install`) to activate pre-commit hooks for your .NET projects.

- **Makefile for .NET CLI Tasks:**  
  A Makefile is included for managing migrations (using EF Core), updating the database, running, and building the application.  
  **Note:** The command to update the database has been renamed from `update-database` to `database-update` in the Makefile.

- **Exception Handling Middleware:**  
  A global exception handling middleware is provided. Simply throw your exceptions and let the middleware handle them.  
  In addition, you can use the `ApiResponse` class for consistent and clean API responses.

- **Fluent Validation:**  
  This template uses Fluent Validation to handle exceptions and validation in a clean, separated manner. This ensures that your validation logic is decoupled from your controllers and business logic.

- **Query Extension for Pagination:**  
  The template includes a query extension that simplifies pagination in your queries.

## Getting Started

1. **Create Repository of this template:**

2. **Install Dependencies:**

   - For Husky (pre-commit hooks), run:
     ```bash
     npx husky-init && npm install
     ```
   - Restore your .NET packages:
     ```bash
     dotnet restore
     ```
3. **Rename the Project**

   - Use the Renaming Script using `Make`
     ```make
     make rename-project name="<project_name>"
     ```

4. **Update Configuration:**

   - Open `appsettings.json` and `appsettings.Development.json`.
   - Update the following configuration values:
     - **Connection String:** Set your PostgreSQL connection string.
       ```json
       "ConnectionStrings": {
         "DbConnectionString": "Server=your_server;Database=your_db;User Id=your_user;Password=your_password;"
       }
       ```
     - **JWT Settings:** Update your JWT secret key, issuer, and audience.
       ```json
       "Jwt": {
         "Key": "YourVerySecureSecretKey",
         "Issuer": "yourdomain.com",
         "Audience": "yourdomain.com"
       }
       ```
       You can generate a JWT key from this [Website](https://jwtsecret.com/generate)
       
     - **Security Settings:**  
       **Important:** Change the `EncryptionKey` to your own secure key.
       ```json
       "Security": {
         "EncryptionKey": "YourUniqueEncryptionKeyHere",
         "SystemAdminPassword": "YourSystemAdminPasswordHere"
       }
       ```

5. **Run the Application:**

   Use the Makefile to run migrations, update the database, or run the application. For example, to update the database:
   ```bash
   make database-update
   ```
   And to run the application:
   ```bash
   make run
   ```

## Architecture Overview

- **Domain:** Contains entities and business logic.
- **Application:** Implements CQRS, UnitOfWork, and application services.  
  The `IJwtService` interface is defined here for token generation/validation.
- **Infrastructure:** Implements repositories, a generic repository, and data access via EF Core (Postgres).  
  Uses DI for each component.
- **Presentation:** Web API project containing middleware (JWT, exception handling) and controllers.

## Exception Handling & API Responses

- **Exception Handling Middleware:**  
  Automatically catches exceptions thrown during request processing and formats responses using the `ApiResponse` class.
  
- **ApiResponse Class:**  
  Use this class in your controllers to return standardized responses.

## Pagination

- **Query Extension for Pagination:**  
  A query extension is available to simplify pagination. Use it in your query handlers to easily implement paging functionality.

## Contributions & Feedback

Contributions, suggestions, and feedback are welcome!  
If you have any ideas or improvements that could help enhance this template, don't hesitate to contribute or open an issue.

## Conclusion

This template is designed to jumpstart your .NET application development using best practices like DDD, Clean Architecture, DI, UnitOfWork, CQRS, and more. Customize the configuration files, update the connection strings and JWT settings, and extend the provided features to suit your application's needs.

Happy coding!

By Ahmad Awwad :)

