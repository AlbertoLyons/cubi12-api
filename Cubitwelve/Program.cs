using Cubitwelve.Src.Extensions;
using Cubitwelve.Src.Middlewares;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
var allowAllOrigins = "_allowAllOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowAllOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Cache());
});

var app = builder.Build();

app.UseOutputCache();

app.UseHttpsRedirection();

// Because it's the first middleware, it will catch all exceptions
app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(allowAllOrigins);


app.UseAuthentication();
app.UseAuthorization();

// app.UseIsUserEnabled();


app.MapControllers();

// Database Bootstrap
AppSeedService.SeedDatabase(app);

app.Run();