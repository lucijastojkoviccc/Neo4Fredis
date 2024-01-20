using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Neo4Fredis.Hubs;
using Neo4Fredis.Services.Implementation;
using Neo4Fredis.Services.Usage;
using Neo4jClient;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Neo4Fredis", Version = "v1" });

    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

});

builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Neo4FredisR";
});

var clientNeo4j = new BoltGraphClient(new Uri("neo4j+s://1646b92d.databases.neo4j.io"), "neo4j", "12345678");
clientNeo4j.ConnectAsync();

builder.Services.AddSingleton<IGraphClient>(clientNeo4j);


builder.Services.AddScoped<IMestoService, MestoService>();
builder.Services.AddScoped<IChatServices, ChatService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IKorisnikService, KorisnikService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Login/LoginPage";
        options.Cookie.Name = "Kolacic";
    });

builder.Services.AddMvc();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSignalR();//.AddRedis("localhost:5502:6379");
builder.Services.AddResponseCompression(options => {
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {
        "application/octet-stream"
    });
});
var redisClient = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"));
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));


builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CORS", options =>
    {
        options.AllowAnyHeader()
               .AllowAnyMethod()
               .WithOrigins("http://127.0.0.1:7120",
                            "https://127.0.0.1:7120",
                            "http://localhost:7120",
                            "https://localhost:7120",
                            "http://127.0.0.1:5098",
                            "https://127.0.0.1:5098",
                            "http://localhost:5098",
                            "https://localhost:5098",
                            "http://127.0.0.1:64788",
                            "https://127.0.0.1:64788",
                            "http://localhost:64788",
                            "https://localhost:64788"
                            );
    });
});


var app = builder.Build();

app.UseResponseCompression();

OpenBrowser("http://localhost:5098/");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//
app.UseStaticFiles();

app.UseRouting();
//

app.UseCors("CORS");

app.UseAuthentication();

app.UseAuthorization();

app.UseCookiePolicy();
app.MapRazorPages();

app.MapBlazorHub();

app.MapHub<ChatHub>("/chatHub");

app.UseEndpoints(e => {
    e.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


});


app.Run();


void OpenBrowser(string url)
{
    try
    {
        // Use the default web browser to open the specified URL
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
    catch (Exception ex)
    {
        // Handle any exceptions that may occur while trying to open the browser
        Console.WriteLine($"Error opening web browser: {ex.Message}");
    }
}