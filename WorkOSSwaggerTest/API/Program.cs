using API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddSwaggerConfig(builder.Configuration);
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var clientId = builder.Configuration["ClientId"];
app.UseSwagger();
app.UseSwaggerUI(
    options =>
    {
        options.OAuthClientId(clientId);
        options.OAuthUsePkce();
    });

app.UseAuthorization();

app.MapControllers();

app.Run();