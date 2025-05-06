using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 🔐 Shared secret for HMAC verification
const string sharedSecret = "shared-secret-here";

app.MapPost("/", async (HttpRequest request) =>
{
    // Read body as string
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();

    // Get the signature from header
    if (!request.Headers.TryGetValue("X-Signature-Base64HmacSha256", out var signatureHeader))
    {
        return Results.BadRequest("Missing signature header");
    }

    var signature = signatureHeader.ToString();

    // Compute HMAC-SHA256
    using var hmac = new HMACSHA256(Convert.FromHexString(sharedSecret));
    var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
    var computedSignature = Convert.ToBase64String(hashBytes);

    // Compare computed signature to header
    if (!CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedSignature),
            Encoding.UTF8.GetBytes(signature)))
    {
        Console.WriteLine("🛑 Invalid signature.");
        return Results.Unauthorized();
    }

    Console.WriteLine("✅ Valid signature.");
    Console.WriteLine("📦 Received body:");
    Console.WriteLine(body);

    // Deserialize and process the JSON
    try
    {
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
        Console.WriteLine("🔍 Parsed data:");
        foreach (var kv in data!)
        {
            Console.WriteLine($"{kv.Key}: {kv.Value}");
        }
    }
    catch
    {
        Console.WriteLine("⚠️ Failed to parse JSON.");
    }

    return Results.Ok(new { status = "verified" });
});

app.Run();
