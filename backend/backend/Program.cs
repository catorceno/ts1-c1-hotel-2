// http://localhost:5162

using Supabase;
using backend.Mappings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));

var frontendUrl = builder.Configuration["AllowedOrigin"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(frontendUrl)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];
var options = new SupabaseOptions
{
    AutoRefreshToken = true,
    AutoConnectRealtime = true
};
builder.Services.AddScoped<Supabase.Client>(_ =>
    new Supabase.Client(supabaseUrl, supabaseKey, options));


builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IGuestBookingRepository, GuestBookingRepository>();
builder.Services.AddScoped<IGuestRepository, GuestRepository>();
builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IGuestService, GuestService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendPolicy");
app.MapControllers();
app.Run();