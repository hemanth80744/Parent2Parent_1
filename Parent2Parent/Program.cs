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
    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
    options.AddPolicy("Production", p =>
        p.WithOrigins(allowedOrigins)
         .AllowAnyHeader()
         .AllowAnyMethod());
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
