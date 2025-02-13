# Set default shell
SHELL := /bin/bash

# Run the API project
run:
	dotnet run --project src/Dotnet.Template.Presentation.API

# Run the API project
watch:
	dotnet watch --project src/Dotnet.Template.Presentation.API

# Build the solution
build:
	dotnet build

# Restore NuGet packages
restore:
	dotnet restore

# Clean the project
clean:
	dotnet clean

# Create and apply database migrations (for EF Core)
migrate:
	dotnet ef migrations add $(name) --project src/Dotnet.Template.Infrastructure --startup-project src/Dotnet.Template.Presentation.API
	dotnet ef database update --project src/Dotnet.Template.Infrastructure --startup-project src/Dotnet.Template.Presentation.API