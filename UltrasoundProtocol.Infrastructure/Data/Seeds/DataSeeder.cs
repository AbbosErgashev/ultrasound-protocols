using Microsoft.EntityFrameworkCore;
using UltrasoundProtocol.Domain.Entities;
using UltrasoundProtocol.Domain.Enums;
using UltrasoundProtocol.Infrastructure.Security;

namespace UltrasoundProtocol.Infrastructure.Data.Seeds;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        Console.WriteLine("[DataSeeder] Migratsiya qo'llanilmoqda...");
        await context.Database.MigrateAsync();
        Console.WriteLine("[DataSeeder] Migratsiya tayyor");

        if (!await context.Users.AnyAsync())
        {
            Console.WriteLine("[DataSeeder] Boshlang'ich foydalanuvchilar yaratilmoqda...");
            var hasher = new PasswordHasher();

            var patients = new List<User>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Username = "bemor001",
                    FullName = "Alisher Ergashev",
                    DateOfBirth = new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                    Gender = "Erkak",
                    PhoneNumber = "+998901234567",
                    Email = "alisher@example.com",
                    PasswordHash = hasher.Hash("Patient@123"),
                    Role = UserRole.User,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Username = "bemor002",
                    FullName = "Malika Yusupova",
                    DateOfBirth = new DateTime(1992, 7, 20, 0, 0, 0, DateTimeKind.Utc),
                    Gender = "Ayol",
                    PhoneNumber = "+998907654321",
                    Email = "malika@example.com",
                    PasswordHash = hasher.Hash("Patient@123"),
                    Role = UserRole.User,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Users.AddRangeAsync(patients);
            await context.SaveChangesAsync();
        }
    }
}
