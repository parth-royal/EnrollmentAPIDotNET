


<!-- pandoc -f markdown -t html   --standalone   --css copenhagen.css   --toc   --toc-depth=3   --number-sections   input.md   -o output.html 
 -->

<!-- https://github.com/jez/pandoc-markdown-css-theme -->
<!-- dotnet run 
 -->




<!-- dotnet new web -n MyWebApp -->

<!-- create project first then pandochtml -->
<!-- # requiremtns techn terms  -->
<!-- # getwhat is structred destind  saved for future referance for custom fast iteration etc  -->

<!-- https://github.com/emmi-01/EnrollmentAPIv2 -->


---
title: " ASP.NET web application:minimal API that serves an HTML form and handles form submissions for enrollment:." 
---



<!-- dotnet add package Microsoft.EntityFrameworkCore.Sqlite
 -->
## Program Initialization
```cs
using Microsoft.AspNetCore.Builder; //create and configure an ASP.NET Core application.
using Microsoft.AspNetCore.Http; //classes and interfaces for handling HTTP requests and responses.
using Microsoft.Extensions.DependencyInjection; // - Provides classes to register and configure services for dependency injection.
using Microsoft.EntityFrameworkCore; //classes required to work with Entity Framework Core, an ORM (Object-Relational Mapper) for .NET.
using System; // basic system functions and types.

```


## Build and Configure the Application
```cs
var builder = WebApplication.CreateBuilder(args); // new instance of  class to set up conf logging and depedency injection? services for webap

```
? WebApplication class imported from where 

## Optionl 
```cs

builder.Services.AddEndpointsApiExplorer(); // - Adds services to generate API documentation.
builder.Services.AddSwaggerGen(); //  Adds Swagger generation services to the container, which enables the API documentation and testing interface.

```

## Configure SQLite Database

<!--  - Registers the EnrollmentDbContext with the dependency injection container and configures it to use a SQLite database with the specified connection string. -->
```cs
builder.Services.AddDbContext<EnrollmentDbContext>(options =>
{
    options.UseSqlite("Data Source=dbname.db"); // SQLite connection string
});
```


## Build the Application

```cs
var app = builder.Build(); // Builds the WebApplication instance which is used to configure the request pipeline.
```

## Ensure Database is Created and Apply Migrations

```cs
using (var scope = app.Services.CreateScope()) //Creates a new scope for resolving services.
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EnrollmentDbContext>(); // - Retrieves an instance of EnrollmentDbContext from the service provider.
    dbContext.Database.EnsureCreated(); //- Ensures that the database for the context exists. If it doesn't, it creates the database.

    dbContext.Database.Migrate(); // Apply pending migrations 
}

```

## Configure the HTTP Request Pipeline

```cs
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // - Enables the middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwaggerUI();
}	// - Checks if the application is running in the development environment.

```



