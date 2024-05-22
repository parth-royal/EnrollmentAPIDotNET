using Microsoft.AspNetCore.Builder; // Provides classes for creating and configuring ASP.NET Core applications.
using Microsoft.AspNetCore.Http; // Contains classes and interfaces for handling HTTP requests and responses.
using Microsoft.Extensions.DependencyInjection; // Provides classes for registering and configuring services for dependency injection.
using Microsoft.EntityFrameworkCore; // Contains classes required to work with Entity Framework Core, an ORM for .NET.
using System; // Provides basic system functions and types.

var builder = WebApplication.CreateBuilder(args); // Creates a new instance of the WebApplication class for setting up configuration, logging, and dependency injection services for the web application.

builder.Services.AddDbContext<EnrollmentDbContext>(options =>
{
    options.UseSqlite("Data Source=testA.db"); // Configures the EnrollmentDbContext to use a SQLite database with the specified connection string.
});

var app = builder.Build(); // Builds the WebApplication instance, which is used to configure the request pipeline.

using (var scope = app.Services.CreateScope()) // Creates a new scope for resolving services.
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EnrollmentDbContext>(); // Retrieves an instance of EnrollmentDbContext from the service provider.
    dbContext.Database.EnsureCreated(); // Ensures that the database for the context exists. If it doesn't, it creates the database.
    dbContext.Database.Migrate(); // Applies pending migrations.
}

app.UseHttpsRedirection(); // Enables HTTPS redirection.

app.MapGet("/", (HttpContext context) =>
{
    return context.Response.WriteAsync("<!DOCTYPE html>\n" +
                                        "<html>\n" +
                                        "<head>\n" +
                                        "    <title>Enrollment Form</title>\n" +
                                        "</head>\n" +
                                        "<body>\n" +
                                        "    <h1>Add New Enrollment</h1>\n" +
                                        "    <form action=\"/enrollments\" method=\"post\">\n" +
                                        "        <label for=\"studentId\">Student ID:</label>\n" +
                                        "        <input type=\"number\" id=\"studentId\" name=\"StudentId\" required><br><br>\n" +
                                        "        <label for=\"studentName\">Student Name:</label>\n" +
                                        "        <input type=\"text\" id=\"studentName\" name=\"StudentName\" required><br><br>\n" +
                                        "        <label for=\"dateOfBirth\">Date of Birth:</label>\n" +
                                        "        <input type=\"date\" id=\"dateOfBirth\" name=\"DateOfBirth\" required><br><br>\n" +
                                        "        <label for=\"gender\">Gender:</label>\n" +
                                        "        <select id=\"gender\" name=\"Gender\">\n" +
                                        "            <option value=\"Male\">Male</option>\n" +
                                        "            <option value=\"Female\">Female</option>\n" +
                                        "            <option value=\"Other\">Other</option>\n" +
                                        "        </select><br><br>\n" +
                                        "        <label for=\"address\">Address:</label>\n" +
                                        "        <textarea id=\"address\" name=\"Address\" required></textarea><br><br>\n" +
                                        "        <label for=\"email\">Email:</label>\n" +
                                        "        <input type=\"email\" id=\"email\" name=\"Email\" required><br><br>\n" +
                                        "        <label for=\"phoneNumber\">Phone Number:</label>\n" +
                                        "        <input type=\"tel\" id=\"phoneNumber\" name=\"PhoneNumber\" required><br><br>\n" +
                                        "        <label for=\"courseId\">Course ID:</label>\n" +
                                        "        <input type=\"number\" id=\"courseId\" name=\"CourseId\" required><br><br>\n" +
                                        "        <label for=\"enrollmentDate\">Enrollment Date:</label>\n" +
                                        "        <input type=\"date\" id=\"enrollmentDate\" name=\"EnrollmentDate\" required><br><br>\n" +
                                        "        <label for=\"registrationDate\">Registration Date:</label>\n" +
                                        "        <input type=\"date\" id=\"registrationDate\" name=\"RegistrationDate\" required><br><br>\n" +
                                        "        <button type=\"submit\">Submit</button>\n" +
                                        "    </form>\n" +
                                        "</body>\n" +
                                        "</html>"); // Handles GET requests to the root URL by returning an HTML form for adding new enrollments.
});

app.MapPost("/enrollments", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync(); // Reads form data asynchronously from the HTTP request.
    if (!int.TryParse(form["StudentId"], out var studentId) || // Tries to parse the student ID from the form data.
        !int.TryParse(form["CourseId"], out var courseId) || // Tries to parse the course ID from the form data.
        !DateTime.TryParse(form["EnrollmentDate"], out var enrollmentDate) || // Tries to parse the enrollment date from the form data.
        !DateTime.TryParse(form["RegistrationDate"], out var registrationDate) || // Tries to parse the registration date from the form data.
        !DateTime.TryParse(form["DateOfBirth"], out var dateOfBirth)) // Tries to parse the date of birth from the form data.
    {
        context.Response.StatusCode = 400; // Sets the status code to 400 (Bad Request) if parsing fails.
        return;
    }

    using (var scope = app.Services.CreateScope()) // Creates a new scope for resolving services.
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<EnrollmentDbContext>(); // Retrieves an instance of EnrollmentDbContext from the service provider.
        var enrollment = new Enrollment // Creates a new enrollment object with data from the form.
        {
            StudentId = studentId,
            StudentName = form["StudentName"],
            DateOfBirth = dateOfBirth,
            Gender = form["Gender"],
            Address = form["Address"],
            Email = form["Email"],
            PhoneNumber = form["PhoneNumber"],
            CourseId = courseId,
            EnrollmentDate = enrollmentDate,
            RegistrationDate = registrationDate
        };
        dbContext.Enrollments.Add(enrollment); // Adds the enrollment object to the DbSet<Enrollment> in the DbContext.
        await dbContext.SaveChangesAsync(); // Saves changes to the database asynchronously.

        context.Response.StatusCode = 201; // Sets the status code to 201 (Created) indicating successful enrollment.
        context.Response.Headers.Append("Location", $"/enrollments/{enrollment.Id}"); // Adds the location header with the URL of the newly created enrollment.
        await context.Response.WriteAsJsonAsync(enrollment); // Writes the enrollment data as JSON to the response body.
    }
});

app.Run(); // Runs the application, starting the HTTP server.
