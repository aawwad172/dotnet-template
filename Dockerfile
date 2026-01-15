FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build  
WORKDIR /src 

# copy only project files first to leverage layer caching  
COPY *.sln .  
COPY src/Dotnet.Template.Application/*.csproj ./src/Dotnet.Template.Application/  
COPY src/Dotnet.Template.Domain/*.csproj    ./src/Dotnet.Template.Domain/  
COPY src/Dotnet.Template.Infrastructure/*.csproj ./src/Dotnet.Template.Infrastructure/  
COPY src/Dotnet.Template.Presentation.API/*.csproj ./src/Dotnet.Template.Presentation.API/  

RUN dotnet restore 

# copy everything else and publish  
COPY . .  
RUN dotnet publish src/Dotnet.Template.Presentation.API \
    -c Release \
    -o /app/publish 

# ---------- 2) Runtime ----------  
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime  
WORKDIR /app 

# configure ASP.NET to listen on port 80  
ENV ASPNETCORE_URLS=http://+:80 

# copy the published output from build stage  
COPY --from=build /app/publish .  

# expose port and set entry point  
EXPOSE 80  
ENTRYPOINT ["dotnet", "Dotnet.Template.Presentation.API.dll"]