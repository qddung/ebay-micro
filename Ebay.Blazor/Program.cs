using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Ebay.Blazor.Data;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Ebay.Blazor.Data.ServiceUltil;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazorise(options =>
{
    options.Immediate = true;
})
.AddBootstrap5Providers().AddFontAwesomeIcons();


// builder.Services.AddSingleton<IConfigurationRoot>((sp) =>
// {
//     // Add configuration for in-memory settings 
//     var configurationBuilder = new ConfigurationBuilder();

//     configurationBuilder.AddInMemoryCollection(
//         new Dictionary<string, string?>
//         {
//             ["BackendUrl"] = "http://localhost:5089",
//         });

//     var config = configurationBuilder.Build();

//     return config;
// });




builder.Services.AddHttpClient();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped<HttpBackendRequestService>();
builder.Services.AddScoped<EbayProductState>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.AllowTrailingCommas = true;
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
