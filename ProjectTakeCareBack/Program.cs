using ProjectTakeCareBack.Data;
using Microsoft.EntityFrameworkCore;
using ProjectTakeCareBack.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TakeCareContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSql")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true);
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();
    
app.MapHub<ChatHub>("/chatHub");

app.Run();