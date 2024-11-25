using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Json;
using System.Security.Claims;
using System.Text;
using VirtualShopMinimalAPI.Data;
using VirtualShopMinimalAPI.Models;
using VirtualShopMinimalAPI.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("defaultconnection")));

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException("JWT key is not configured or is too short.");
}
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;  
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.UseCors("AllowAll");


app.MapPost("/api/User/register", async (User user, IUserService userService) =>
{
    return await userService.RegisterUser(user);
})
.WithName("RegistrarUsuario")
.WithTags("Usuários");

app.MapPost("/api/User/login", async (LoginRequest loginRequest, IUserService userService) =>
{
    return await userService.LoginUser(loginRequest);
})
.WithName("LoginUsuario")
.WithTags("Usuários");

app.MapPost("/api/User/logout", async (HttpContext http) =>
{
    await http.SignOutAsync();
    return Results.Ok(new { Mensagem = "Logout realizado com sucesso" });
})
.WithName("LogoutUsuario")
.WithTags("Usuários")
.RequireAuthorization();

app.MapGet("/api/User/profile", async (HttpContext http, IUserService userService) =>
{
    var email = http.User.FindFirstValue(ClaimTypes.Email);
    if (string.IsNullOrEmpty(email))
    {
        return Results.Unauthorized();
    }

    return await userService.GetUserProfile(email);
})
.WithName("PerfilUsuario")
.WithTags("Usuários")
.RequireAuthorization();

app.MapPut("/api/User/profile", async (User updatedUser, HttpContext http, IUserService userService) =>
{
    var email = http.User.FindFirstValue(ClaimTypes.Email);
    if (string.IsNullOrEmpty(email))
    {
        return Results.Unauthorized();
    }

    return await userService.UpdateUserProfile(email, updatedUser);
})
.WithName("AlterarPerfil")
.WithTags("Usuários")
.RequireAuthorization();

app.MapPost("/api/Admin", async (User admin, IUserService userService) =>
{
    return await userService.AddAdmin(admin);
})
.WithName("AdicionarAdmin")
.WithTags("Usuários")
.RequireAuthorization();

app.MapPost("/api/Product", async (Product product, IProductService productService) =>
{
    return await productService.AddProduct(product);
})
.WithName("AdicionarProduto")
.WithTags("Produtos")
.RequireAuthorization();

app.MapGet("/api/Product", async (IProductService productService) =>
{
    return await productService.GetProducts();
})
.WithName("ListarProdutos")
.WithTags("Produtos");

app.MapGet("/api/Product/search", async (string? name, IProductService productService) =>
{
    return await productService.SearchProducts(name);
})
.WithName("PesquisarProdutos")
.WithTags("Produtos");

app.MapDelete("/api/Product/{id}", async (int id, IProductService productService) =>
{
    return await productService.DeleteProduct(id);
})
.WithName("DeletarProduto")
.WithTags("Produtos")
.RequireAuthorization();

app.MapPost("/api/Sale", async (SaleRequest saleRequest, ISaleService saleService) =>
{
    return await saleService.RegisterSale(saleRequest);
})
.WithName("RegistrarVenda")
.WithTags("Vendas")
.RequireAuthorization();

app.MapGet("/api/auth/google-login", async (IAuthService authService, HttpContext http) =>
{
    await authService.GoogleLogin(http);
})
.WithName("LoginComGoogle")
.WithTags("Autenticação");

app.MapGet("/api/auth/google-callback", async (IAuthService authService, HttpContext http) =>
{
    return await authService.GoogleCallback(http);
})
.WithName("GoogleCallback")
.WithTags("Autenticação");

app.Run();