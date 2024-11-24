using DotnetAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:4200", "https://localhost:3000", "https://localhost:8000")
            .AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
    options.AddPolicy("ProdCors", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://myProductionSite.com")
            .AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    }); 
});

builder.Services.AddScoped<IUserRepository, UserRepository>(); 



var app = builder.Build();

if (app.Environment.IsDevelopment()) 
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

app.UseCors("devCors"); 
app.MapControllers();

app.Run();