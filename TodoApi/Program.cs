// קובץ המגדיר חוקים 
// יוצר כתובת
using Microsoft.EntityFrameworkCore; // מאפשר להשתמש ב-DbContext וב-MySQL
using TodoApi.DAL;// מאפשר לקובץ הזה "לראות" את ה-TodoContext שיצרת
// קובץ המגדיר חוקים 
// יוצר כתובת
var builder = WebApplication.CreateBuilder(args);

// ==================יוצר mySQL ======================
// הגדרת חיבור למסד הנתונים מתוך appsettings.json
// הזרקת תולויות DEPENDENCY INJECTION
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

//==============הגדרת CORS ו-Swagger====================
// הגדרת Swagger - מאפשר בדיקה נוחה של ה-API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// הגדרת CORS - מאפשר לאפליקציית ה-React לגשת לשרת
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
var app = builder.Build();

//  והפעלת Swagger הגדרות הרצה 
if (app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
}
// הפעלת ה-CORS (חובה לפני ה-Routes!)
app.UseCors("AllowAll");

//========================= כל הפונקציות ================
// 1. שליפת כל המשימות - GET
app.MapGet("/items", async (TodoContext db) =>
    await db.Items.ToListAsync());

// 2. הוספת משימה חדשה - POST
app.MapPost("/items", async (Item newItem, TodoContext db) =>{
    db.Items.Add(newItem);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{newItem.Id}", newItem);
});
// 3. עדכון משימה - PUT
app.MapPut("/items/{id}", async (int id, Item inputItem, TodoContext db) =>{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();
    item.Name = inputItem.Name;
    item.IsComplete = inputItem.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// 4. מחיקת משימה - DELETE
app.MapDelete("/items/{id}", async (int id, TodoContext db) =>{
    if (await db.Items.FindAsync(id) is Item item){
        db.Items.Remove(item);
        await db.SaveChangesAsync();
        return Results.Ok(item);
    }
    return Results.NotFound();
});

//===================== הרצה =======================
app.Run();




// ===================== :התקנות ========================
// התקנת המנוע הראשי:
// dotnet add package Microsoft.EntityFrameworkCore

// התקנת הכלים לעיצוב:
// dotnet add package Microsoft.EntityFrameworkCore.Design

// התקנת ה"מתרגם" ל-MySQL (חשוב מאוד!):
// dotnet add package Pomelo.EntityFrameworkCore.MySql

// התקנת כלי העזר:
// dotnet add package Microsoft.EntityFrameworkCore.Tools