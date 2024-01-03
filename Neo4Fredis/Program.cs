var builder = WebApplication.CreateBuilder(args);



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
                            "https://localhost:64788");
    });
});

//ovo sam dodala
builder.Services.AddMvc().AddJsonOptions(p =>
{
    p.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CORS");

app.UseAuthorization();

app.MapControllers();

app.Run();