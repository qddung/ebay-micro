using Ebay.Backend.Entities.Context;
using Ebay.Backend.Middleware;

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
    app.UseDeveloperExceptionPage();

}


app.UseCors("allow_origin");

app.UseHttpsRedirection();

app.UseRouting();
//app.UseMiddleware<NotFoundMiddleware>();
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint == null)
        {
            Console.WriteLine($"404: No endpoint matched path {context.Request.Path}");
        }
        else
        {
            Console.WriteLine($"404: Endpoint matched but not handled. Path: {context.Request.Path}, Endpoint: {endpoint.DisplayName}");
        }
    }
});


app.UseAuthorization();

// app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.Run();
