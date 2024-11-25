using VirtualShopMinimalAPI.Data;
using VirtualShopMinimalAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("defaultconnection")));

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT key is not configured.");
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/Product", async (Product produto, AppDbContext db) =>
{
    if (db.Products == null)
    {
        return Results.NotFound("Produtos não encontrados.");
    }
    db.Products.Add(produto);
    await db.SaveChangesAsync();
    return Results.Created($"/api/Product/{produto.Id}", produto);
})
.WithName("AdicionarProduto")
.WithTags("Produtos")
.RequireAuthorization();

app.MapGet("/api/Product", async (AppDbContext db) =>
{
    if (db.Products == null)
    {
        return Results.NotFound("Products collection is null.");
    }
    var produtos = await db.Products.ToListAsync();
    return Results.Ok(produtos);
})
.WithName("ListarProdutos")
.WithTags("Produtos");

app.MapGet("/api/Product/search", async (string? nome, AppDbContext db) =>
{
    if (db.Products == null)
    {
        return Results.NotFound("Products collection is null.");
    }
    if (string.IsNullOrEmpty(nome))
    {
        return Results.BadRequest("O parâmetro 'nome' é obrigatório.");
    }
    var resultado = await db.Products
        .Where(p => p.Nome != null && p.Nome.ToLower().Contains(nome.ToLower()))
        .ToListAsync();

    return Results.Ok(resultado);
})
.WithName("PesquisarProdutos")
.WithTags("Produtos");

app.MapPost("/api/Sale", async (SaleRequest saleRequest, AppDbContext db) =>
{
    if (db.Sales == null || db.Products == null)
    {
        return Results.NotFound("Sales or Products collection is null.");
    }

    if (saleRequest.ProductIds.Length != saleRequest.Quantidades.Length)
    {
        return Results.BadRequest("Product IDs and quantities must have the same length.");
    }

    for (int i = 0; i < saleRequest.ProductIds.Length; i++)
    {
        var productId = saleRequest.ProductIds[i];
        var quantidade = saleRequest.Quantidades[i];

        var product = await db.Products.FindAsync(productId);
        if (product == null)
        {
            return Results.NotFound($"Product with ID {productId} not found.");
        }

        saleRequest.Sale.SaleProducts.Add(new SaleProduct { ProductId = productId, Quantidade = quantidade });
    }

    db.Sales.Add(saleRequest.Sale);
    await db.SaveChangesAsync();
    return Results.Ok(saleRequest.Sale);
})
.WithName("RegistrarVenda")
.WithTags("Vendas")
.RequireAuthorization();

app.MapPost("/api/User/register", async (User usuario, AppDbContext db, IPasswordHasher<User> passwordHasher) =>
{
    if (db.Users == null)
    {
        return Results.NotFound("Users collection is null.");
    }

    if (string.IsNullOrEmpty(usuario.NomeUsuario) || string.IsNullOrEmpty(usuario.Email))
    {
        return Results.BadRequest("NomeUsuario e Email são obrigatórios.");
    }

    var usuarioExistente = await db.Users.FirstOrDefaultAsync(u => u.Email == usuario.Email);
    if (usuarioExistente != null)
    {
        return Results.Conflict("Email já está em uso.");
    }

    if (!string.IsNullOrEmpty(usuario.Cpf))
    {
        var cpfExistente = await db.Users.FirstOrDefaultAsync(u => u.Cpf == usuario.Cpf);
        if (cpfExistente != null)
        {
            return Results.Conflict("CPF já está em uso.");
        }
    }

    if (!string.IsNullOrEmpty(usuario.Senha))
    {
        usuario.Senha = passwordHasher.HashPassword(usuario, usuario.Senha);
    }

    db.Users.Add(usuario);
    await db.SaveChangesAsync();
    return Results.Ok(usuario);
})
.WithName("RegistrarUsuario")
.WithTags("Usuários");

app.MapPost("/api/User/login", async (LoginRequest loginRequest, AppDbContext db, IPasswordHasher<User> passwordHasher) =>
{
    if (db.Users == null)
    {
        return Results.NotFound("Users collection is null.");
    }
    var usuario = await db.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
    if (usuario == null)
    {
        return Results.Unauthorized();
    }

    if (usuario.Senha == null)
    {
        return Results.Unauthorized();
    }
    if (string.IsNullOrEmpty(loginRequest.Senha))
    {
        return Results.BadRequest("Senha é obrigatória.");
    }
    var resultado = passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, loginRequest.Senha);
    if (resultado == PasswordVerificationResult.Failed)
    {
        return Results.Unauthorized();
    }

    if (string.IsNullOrEmpty(usuario.Email))
    {
        return Results.BadRequest("Email do usuário não pode ser nulo ou vazio.");
    }

    var claims = new[]
    {
        new Claim(ClaimTypes.Email, usuario.Email),
        new Claim("IsAdmin", usuario.IsAdmin.ToString())
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    return Results.Ok(new { Token = tokenString });
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

app.MapPost("/api/Admin", async (User admin, AppDbContext db, IPasswordHasher<User> passwordHasher) =>
{
    if (admin == null)
    {
        return Results.BadRequest("Admin user cannot be null");
    }
    admin.IsAdmin = true;
    if (db.Users == null)
    {
        return Results.NotFound("Users collection is null.");
    }
    db.Users.Add(admin);
    await db.SaveChangesAsync();
    return Results.Ok(admin);
})
.WithName("AdicionarAdmin")
.WithTags("Usuários")
.RequireAuthorization();

app.MapGet("/api/User/profile", async (AppDbContext db, HttpContext http) =>
{
    var userEmail = http.User.FindFirstValue(ClaimTypes.Email);
    if (string.IsNullOrEmpty(userEmail))
    {
        return Results.Unauthorized();
    }

    var usuario = await db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
    if (usuario == null)
    {
        return Results.NotFound("Usuário não encontrado.");
    }

    return Results.Ok(new 
    { 
        usuario.NomeUsuario, 
        usuario.Email, 
        usuario.Cpf, 
        usuario.IsAdmin 
    });
})
.WithName("PerfilUsuario")
.WithTags("Usuários")
.RequireAuthorization();

app.MapGet("/api/auth/google-login", async (HttpContext http) =>
{
    var properties = new AuthenticationProperties { RedirectUri = "http://localhost:5000/api/auth/google-callback" };
    await http.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
})
.WithName("LoginComGoogle")
.WithTags("Autenticação");

app.MapGet("/api/auth/google-callback", async (HttpContext http, AppDbContext db) =>
{
    var result = await http.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
    if (!result.Succeeded || result.Principal == null)
    {
        return Results.Unauthorized();
    }

    var email = result.Principal.FindFirstValue(ClaimTypes.Email);
    var nome = result.Principal.FindFirstValue(ClaimTypes.Name);

    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nome))
    {
        return Results.BadRequest("Nome e Email são obrigatórios.");
    }

    var usuarioExistente = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (usuarioExistente == null)
    {
        var novoUsuario = new User
        {
            NomeUsuario = nome,
            Email = email,
            IsAdmin = false
        };

        db.Users.Add(novoUsuario);
        await db.SaveChangesAsync();
    }

    return Results.Redirect("http://localhost:3000/home");
})
.WithName("GoogleCallback")
.WithTags("Autenticação");

app.Run();