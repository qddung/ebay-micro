using Ebay.Backend.Entities.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<EBayDbContext>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("allow_origin", policy =>
                      {
                          policy.WithOrigins("http://localhost:5018/").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                      });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("allow_origin");

app.UseHttpsRedirection();



app.UseAuthorization();

app.MapControllers();

app.Run();
