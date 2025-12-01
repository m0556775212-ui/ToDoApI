using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;

// -------------------- BUILD --------------------
var builder = WebApplication.CreateBuilder(args);

// -------------------- SERVICES --------------------

// הוספת DbContext עם MySQL
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ToDoDB"),
        new MySqlServerVersion(new Version(8, 0, 44))
    )
);

// הגדרת CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -------------------- APP --------------------
var app = builder.Build();
// -------------------- בדיקת חיבור למסד --------------------
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
    db.Database.CanConnect(); // מנסה להתחבר למסד
    Console.WriteLine("✅ Database connection successful!");
}
catch (Exception ex)
{
    Console.WriteLine("❌ Database connection failed: " + ex.Message);
}

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

// -------------------- ROUTES --------------------

// שליפת כל המשימות
app.MapGet("/tasks", async (ToDoDbContext db) =>
{
    return await db.Tasks.ToListAsync();
});

// הוספת משימה חדשה
app.MapPost("/tasks", async (ToDoTask task, ToDoDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{task.Id}", task);
});

// עדכון משימה
app.MapPut("/tasks/{id}", async (int id, ToDoTask updatedTask, ToDoDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task == null) return Results.NotFound();

    task.Name = updatedTask.Name;
    task.IsComplete = updatedTask.IsComplete;

    await db.SaveChangesAsync();
    return Results.Ok(task);
});

// מחיקת משימה
app.MapDelete("/tasks/{id}", async (int id, ToDoDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task == null) return Results.NotFound();

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

// -------------------- MODELS --------------------
public class ToDoTask
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}

// -------------------- DbContext --------------------
public class ToDoDbContext : DbContext
{
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options) { }

    public DbSet<ToDoTask> Tasks { get; set; } = null!;
}
