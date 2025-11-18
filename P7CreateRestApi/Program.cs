using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Config;
using P7CreateRestApi.Data;
using P7CreateRestApi.Middlewares;
using P7CreateRestApi.Services;

var builder = WebApplication.CreateBuilder(args);

// --- LOGGING ---
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// --- EF Core ---
builder.Services.AddDbContext<LocalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Identity & JWT ---
builder.Services.AddAppIdentity();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<JwtService>();

// --- Swagger / MVC ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

// --- Vérification de la clé JWT ---
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ ERREUR : Clé JWT introuvable dans appsettings.json !");
    Console.ResetColor();
}
else
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"✅ Clé JWT chargée ({jwtKey.Length} caractères)");
    Console.ResetColor();
}

// --- PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --- Seed rôles et admin ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleInitializer.SeedRolesAndAdminAsync(services);
}

await app.RunAsync();
