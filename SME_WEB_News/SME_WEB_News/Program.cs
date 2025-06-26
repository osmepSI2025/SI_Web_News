using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using SME_WEB_News.Services;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.WebSso;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Update the Serilog configuration to use the correct method
builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) // Removed GetSection("Serilog")
    .WriteTo.File(
        path: "Logs/app-log.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
);

// อ่านค่า SAML2 จาก appsettings.json
var saml2Section = builder.Configuration.GetSection("Saml2:Saml2");
var entityId = saml2Section["EntityId"];
var idpEntityId = saml2Section["IdpEntityId"];
var ssoUrl = saml2Section["SingleSignOnServiceUrl"];
var sloUrl = saml2Section["SingleLogoutServiceUrl"];
var signingCertBase64 = saml2Section["SigningCertificate"];
var AcsUrl = saml2Section["AcsUrl"];
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Saml2";
})
.AddCookie()
.AddSaml2(options =>
{
    builder.Configuration.GetSection("SustainsysSaml2").Bind(options);
    options.SPOptions.EntityId = new EntityId(entityId);
    options.SPOptions.ReturnUrl = new Uri(AcsUrl);

    var idp = new IdentityProvider(
        new EntityId(idpEntityId),
        options.SPOptions)
    {
        SingleSignOnServiceUrl = new Uri(ssoUrl),
        SingleLogoutServiceUrl = new Uri(sloUrl),
        Binding = Saml2BindingType.HttpRedirect,
        AllowUnsolicitedAuthnResponse = true
    };

    // แปลง base64 string เป็น X509Certificate2 แล้วเพิ่มเข้า SigningKeys
    idp.SigningKeys.AddConfiguredKey(new X509Certificate2(Convert.FromBase64String(signingCertBase64)));

    options.IdentityProviders.Add(idp);
});

// Add services for MVC, authentication, and session management

builder.Services.AddScoped<ICallAPIService, CallAPIService>();
builder.Services.AddHttpClient<CallAPIService>();
builder.Services.AddScoped<SME_WEB_News.DAO.ServiceCenter>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseRouting();
app.UseStaticFiles(); // Add this if you need to serve static files
app.UseSession(); // <-- Add this line
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();

app.MapGet("Account/Login", async context =>
{
    // Check if the user is already authenticated
    if (context.User.Identity?.IsAuthenticated == true)
    {
        try
        {
            // Store claims in session as a dictionary
            var claimsDict = context.User.Claims.ToDictionary(c => c.Type, c => c.Value);
            context.Session.SetString("UserClaims", System.Text.Json.JsonSerializer.Serialize(claimsDict));

            // Log all claims
            foreach (var claim in context.User.Claims)
            {
                Log.Information("Claim: {Type} = {Value}", claim.Type, claim.Value);
            }
            context.Response.Redirect("/");
            return;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in SAMLLogin");
            context.Response.Redirect("/account/login?error=" + Uri.EscapeDataString("SAML login failed: " + ex.Message));
            return;
        }
    }
    await context.ChallengeAsync("Saml2");
});

app.MapGet("account/logout", async context =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

// Remove or comment out this line if present:
// app.MapGet("/", () => "SAML2 Integration Ready. Go to /account/login to initiate SSO.");

// Add this before app.Run();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");


try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}