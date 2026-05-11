using CetBookStore.Data;
using CetBookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CetBookStore.Models;

namespace CetBookStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<CetUser>(
                options => options.SignIn.RequireConfirmedAccount = false
                
                )
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            
            //ai generated data creating code for testing
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
                // Eğer hiç kategori yoksa ekle
                if (!context.Categories.Any())
                {
                    var roman = new Category { Name = "Roman", IsVisibleInMenu = true };
                    var yazilim = new Category { Name = "Yazılım", IsVisibleInMenu = true };
                    context.Categories.AddRange(roman, yazilim);
                    context.SaveChanges();

                    // Kategori ekledikten sonra kitapları ekle
                    if (!context.Books.Any())
                    {
                        context.Books.Add(new Book 
                        { 
                            Title = "Nutuk", 
                            Author = "Mustafa Kemal Atatürk", 
                            Description = "Türkiye Cumhuriyeti'nin kuruluş belgesi.",
                            Price = 150, 
                            CategoryId = roman.Id,
                            PublicationDate = DateTime.SpecifyKind(new DateTime(1927, 10, 15), DateTimeKind.Utc),
                            CreatedDate = DateTime.UtcNow,
                            Publisher = "Kültür Yayınları"
                        });

                        context.Books.Add(new Book 
                        { 
                            Title = "C# ve .NET", 
                            Author = "Güney Can Yılmaz", 
                            Description = "Mac üzerinde .NET geliştirme rehberi.",
                            Price = 250, 
                            CategoryId = yazilim.Id,
                            PublicationDate = DateTime.UtcNow,
                            CreatedDate = DateTime.UtcNow,
                            Publisher = "Boğaziçi Press"
                        });
                        context.SaveChanges();
                    }
                }
            }
            
            
            app.Run();
        }
    }
}
