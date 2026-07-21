using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
// Kendi projenizin gerektirdiği diğer using'ler buraya gelebilir (Örn: using Microsoft.EntityFrameworkCore;)

var builder = WebApplication.CreateBuilder(args);

// --- 1. CORS İZNİ EKLENEN KISIM ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// ----------------------------------

// Eğer veritabanı (DbContext) ayarlarınız varsa onlar burada kalmalı
// Örn: builder.Services.AddDbContext<TodoContext>(...);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// --- 2. CORS'U AKTİF ETTİĞİMİZ KISIM (UseRouting ve UseAuthorization arasında olmalı) ---
app.UseCors("AllowAll");
// --------------------------------------------------------------------------------------

app.UseAuthorization();
app.MapControllers();

app.Run();
