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
        // For troubleshooting, we allow everything. 
        // We will restrict this later once we confirm connectivity.
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<Parent2Parent.Data.IDbHelper, Parent2Parent.Data.DbHelper>();
builder.Services.AddScoped<Parent2Parent.Services.IUsersService, Parent2Parent.Services.UsersService>();
builder.Services.AddScoped<Parent2Parent.Services.IRequestsService, Parent2Parent.Services.RequestsService>();
builder.Services.AddScoped<Parent2Parent.Services.IMessagesService, Parent2Parent.Services.MessagesService>();

var app = builder.Build();

// Enable CORS at the very beginning of the request pipeline
app.UseCors(policy => 
{
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
});

app.UseMiddleware<Parent2Parent.Middleware.ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Note: We removed UseHttpsRedirection here because Render handles SSL termination 
// and internal redirection can sometimes interfere with CORS preflight requests.

app.MapControllers();

// Health check endpoint for Render to verify the app is live
app.MapGet("/", () => "Parent2Parent API is live and running!");

app.Run();
