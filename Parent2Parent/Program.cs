var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS services.
builder.Services.AddCors(options =>
{
    // For development, allow localhost.
    options.AddPolicy("FrontendDev", p =>
        p.WithOrigins("http://localhost:4200", "http://localhost:4201")
         .AllowAnyHeader()
         .AllowAnyMethod());

    // For production, allow origins from configuration.
    var originsConfig = builder.Configuration["AllowedOrigins"];
    string[] allowedOrigins;
    
    if (string.IsNullOrEmpty(originsConfig))
    {
        allowedOrigins = [];
    }
    else if (originsConfig.StartsWith("[") && originsConfig.EndsWith("]"))
    {
        // Try parsing as JSON array if it looks like one
        allowedOrigins = System.Text.Json.JsonSerializer.Deserialize<string[]>(originsConfig) ?? [];
    }
    else
    {
        // Otherwise treat as comma-separated string
        allowedOrigins = originsConfig.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    options.AddPolicy("Production", p =>
    {
        if (allowedOrigins.Length > 0)
        {
            p.WithOrigins(allowedOrigins)
             .AllowAnyHeader()
             .AllowAnyMethod();
        }
        else
        {
            // Fallback for troubleshooting: allow all if none configured
            // WARNING: Use with caution in real production
            p.AllowAnyOrigin()
             .AllowAnyHeader()
             .AllowAnyMethod();
        }
    });
});

builder.Services.AddSingleton<Parent2Parent.Data.IDbHelper, Parent2Parent.Data.DbHelper>();
builder.Services.AddScoped<Parent2Parent.Services.IUsersService, Parent2Parent.Services.UsersService>();
builder.Services.AddScoped<Parent2Parent.Services.IRequestsService, Parent2Parent.Services.RequestsService>();
builder.Services.AddScoped<Parent2Parent.Services.IMessagesService, Parent2Parent.Services.MessagesService>();

var app = builder.Build();

app.UseMiddleware<Parent2Parent.Middleware.ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("FrontendDev");
}
else
{
    app.UseHttpsRedirection();
    app.UseCors("Production");
}

app.MapControllers();

app.Run();
