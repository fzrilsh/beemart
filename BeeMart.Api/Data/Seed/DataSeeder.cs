using System.Globalization;
using System.Text.RegularExpressions;
using BeeMart.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeeMart.Api.Data.Seed;

public static class DataSeeder
{
    private static readonly Regex ProductRowPattern = new(
        @"\(NEWID\(\),\s*'((?:[^']|'')*)',\s*([\d.]+),\s*(\d+),\s*([\d.]+),\s*([\d.]+),\s*1,\s*GETDATE\(\)",
        RegexOptions.Compiled);

    public static async Task SeedAsync(BeeMartDbContext db, CancellationToken cancellationToken)
    {
        await SeedUsersAsync(db, cancellationToken);
        await SeedPaymentMethodsAsync(db, cancellationToken);

        if (await db.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        var script = ReadSeedScript();
        var matches = ProductRowPattern.Matches(script);
        if (matches.Count == 0)
        {
            return;
        }

        var now = DateTime.Now;
        foreach (Match match in matches)
        {
            db.Products.Add(new Product
            {
                IdProduct = Guid.NewGuid().ToString(),
                Name = match.Groups[1].Value.Replace("''", "'"),
                Weight = decimal.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
                Stock = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture),
                Price = decimal.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture),
                Discount = decimal.Parse(match.Groups[5].Value, CultureInfo.InvariantCulture),
                Stsrc = true,
                Datein = now,
                Userin = "SYSTEM"
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedUsersAsync(BeeMartDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.Now;
        db.Users.AddRange(
            new User
            {
                IdUser = "11111",
                Username = "admin",
                FullName = "Administrator",
                PasswordHash = "AQAAAAIAAYagAAAAEABFtwYe+jJe6SPFQabO71AA+wsChLE4TTHwZ13AW3R7v+14uF+ku4njJEz7qJrN/Q==",
                Role = "Admin",
                Stsrc = true,
                Datein = now,
                Userin = "SYSTEM"
            },
            new User
            {
                IdUser = "22222",
                Username = "kasir1",
                FullName = "Kasir Satu",
                PasswordHash = "AQAAAAIAAYagAAAAEPgXRNqKUwgnvTGDkJdJE6goM4VN+opqY/Z4zKlv1TK36gryRKf9UbFAIiRSPbCmmw==",
                Role = "Cashier",
                Stsrc = true,
                Datein = now,
                Userin = "SYSTEM"
            });

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedPaymentMethodsAsync(BeeMartDbContext db, CancellationToken cancellationToken)
    {
        if (await db.PaymentMethods.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.Now;
        db.PaymentMethods.AddRange(
            new PaymentMethod { MethodName = "Cash", Stsrc = true, Datein = now, Userin = "SYSTEM" },
            new PaymentMethod { MethodName = "Debit", Stsrc = true, Datein = now, Userin = "SYSTEM" },
            new PaymentMethod { MethodName = "Credit", Stsrc = true, Datein = now, Userin = "SYSTEM" });

        await db.SaveChangesAsync(cancellationToken);
    }

    private static string ReadSeedScript()
    {
        var assembly = typeof(DataSeeder).Assembly;
        using var stream = assembly.GetManifestResourceStream("BeeMart.Api.Data.Seed.seed-products.sql");
        if (stream is null)
        {
            return string.Empty;
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
