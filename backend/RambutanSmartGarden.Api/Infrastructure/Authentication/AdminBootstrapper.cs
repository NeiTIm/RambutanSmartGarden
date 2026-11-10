
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RambutanSmartGarden.Api.Features.Users.Models;
using RambutanSmartGarden.Api.Infrastructure.Persistence;

namespace RambutanSmartGarden.Api.Infrastructure.Authentication;

public sealed class AdminBootstrapper
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AdminBootstrapper(
        AppDbContext dbContext,
        IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> RunAsync()
    {
        Console.WriteLine("=== KHỞI TẠO SYSTEM ADMIN ===");

        // Chỉ thao tác trên database đã được chỉ định.
        var databaseName = _dbContext.Database
            .GetDbConnection()
            .Database;

        if (!string.Equals(
                databaseName,
                "RambutanSmartGardenDb",
                StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine(
                "Database không đúng. Đã hủy thao tác.");
            return false;
        }

        // Không tự động tạo database hoặc chạy migration.
        if (!await _dbContext.Database.CanConnectAsync())
        {
            Console.WriteLine(
                "Không thể kết nối RambutanSmartGardenDb.");
            return false;
        }

        var appliedMigrations = await _dbContext.Database
            .GetAppliedMigrationsAsync();

        if (!appliedMigrations.Any(migration =>
                migration.EndsWith(
                    "_CreateFoundationTables",
                    StringComparison.Ordinal)))
        {
            Console.WriteLine(
                "Migration CreateFoundationTables chưa được áp dụng.");
            return false;
        }

        // Chỉ cho phép bootstrap Admin đầu tiên.
        var adminExists = await _dbContext.Users
            .AnyAsync(user => user.IsSystemAdmin);

        if (adminExists)
        {
            Console.WriteLine(
                "System Admin đã tồn tại. Không tạo thêm.");
            return false;
        }

        if (Console.IsInputRedirected)
        {
            Console.WriteLine(
                "Cần chạy trong Terminal tương tác.");
            return false;
        }

        Console.Write("Tên đăng nhập Admin: ");
        var username = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(username) ||
            username.Length < 3 ||
            username.Length > 50 ||
            !System.Text.RegularExpressions.Regex.IsMatch(
                username,
                @"^[a-zA-Z0-9._-]+$"))
        {
            Console.WriteLine(
                "Username phải có 3-50 ký tự, chỉ gồm chữ không dấu, số, dấu chấm, gạch dưới hoặc gạch ngang.");
            return false;
        }

        var normalizedUsername = username.ToUpperInvariant();

        if (await _dbContext.Users.AnyAsync(user =>
                user.NormalizedUsername == normalizedUsername))
        {
            Console.WriteLine("Username đã được sử dụng.");
            return false;
        }

        Console.Write("Tên hiển thị Admin: ");
        var displayName = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(displayName) ||
            displayName.Length > 100)
        {
            Console.WriteLine(
                "Tên hiển thị phải có từ 1 đến 100 ký tự.");
            return false;
        }

        Console.Write("Mật khẩu Admin: ");
        var password = ReadPassword();

        if (password.Length < 12 || password.Length > 128)
        {
            Console.WriteLine(
                "Mật khẩu phải có từ 12 đến 128 ký tự.");
            return false;
        }

        Console.Write("Xác nhận mật khẩu: ");
        var confirmPassword = ReadPassword();

        if (password != confirmPassword)
        {
            Console.WriteLine("Hai mật khẩu không khớp.");
            return false;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            NormalizedUsername = normalizedUsername,
            DisplayName = displayName,
            PhoneNumber = null,
            NormalizedPhoneNumber = null,
            IsSystemAdmin = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(user, password);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        Console.WriteLine(
            "Đã khởi tạo tài khoản System Admin thành công.");

        return true;
    }

    private static string ReadPassword()
    {
        var characters = new List<char>();

        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (characters.Count > 0)
                {
                    characters.RemoveAt(characters.Count - 1);
                }

                continue;
            }

            if (!char.IsControl(key.KeyChar))
            {
                characters.Add(key.KeyChar);
            }
        }

        var password = new string(characters.ToArray());
        characters.Clear();

        return password;
    }
}
