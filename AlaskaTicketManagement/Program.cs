using AlaskaTicketManagement.Data;
using AlaskaTicketManagement.Services;
using AlaskaTicketManagement.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<SyntheticDataGenerator>();
builder.Services.AddSingleton<IPaymentService, PaymentService>();
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddHostedService<ReservationService>();

// Add database context, using inmemory DB
builder.Services.AddDbContext<AlaskaConcertDbContext>(options => options.UseInMemoryDatabase("AlaskaTickets"));

//// Add Authentication
//builder.Services.AddAuthentication("DummyScheme")
//    .AddScheme<AuthenticationSchemeOptions, ApiAuthenticationHandler>("DummyScheme", options => { });

//// Add Authorization Policies
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//    options.AddPolicy("EventManager", policy => policy.RequireClaim("EventManager", "true"));
//});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        var generator = scope.ServiceProvider.GetRequiredService<SyntheticDataGenerator>();
        await generator.GenerateAsync();
    }

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
