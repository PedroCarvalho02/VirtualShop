// Rotas da API:
// 
// UserController:
// - POST /api/user/register: Registrar um novo usuário
// - POST /api/user/login: Fazer login e obter um token JWT
// - POST /api/user/logout: Fazer logout e revogar o token JWT (necessário token JWT no cabeçalho Authorization)
// - GET /api/user/profile: Obter perfil do usuário autenticado (necessário token JWT no cabeçalho Authorization)
// 
// ProductController:
// - GET /api/product: Listar todos os produtos
// - GET /api/product/{id}: Obter um produto específico pelo ID
// - POST /api/product: Adicionar um novo produto (necessário token JWT de administrador no cabeçalho Authorization)

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using LojaVirtualAPI.Data;
using LojaVirtualAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=lojaVirtual.db"));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var configuracao = builder.Configuration;
var configuracaoJwt = configuracao.GetSection("Jwt");
var chaveJwt = configuracaoJwt["Key"];
if (string.IsNullOrEmpty(chaveJwt))
{
    throw new InvalidOperationException("A chave JWT não está configurada corretamente.");
}
var chave = Encoding.UTF8.GetBytes(chaveJwt);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(chave),
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
            {
                var token = securityToken as JwtSecurityToken;
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                var httpContextAccessor = validationParameters.IssuerSigningKey as IHttpContextAccessor;
                if (httpContextAccessor?.HttpContext == null)
                {
                    return false;
                }
                var dbContext = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                var isRevoked = dbContext.RevokedTokens?.Any(rt => rt.Token == tokenString) ?? false;
                return !isRevoked && expires > DateTime.UtcNow;
            }
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var escopo = app.Services.CreateScope())
{
    var contexto = escopo.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    contexto.Database.Migrate();

    if (contexto.Users != null && !contexto.Users.Any(u => u.Email == "admin@admin.com"))
    {
        var usuarioAdmin = new User
        {
            Name = "Admin",
            CPF = "000.000.000-00",
            Email = "admin@admin.com",
            Password = BCrypt.Net.BCrypt.HashPassword("admin"),
            IsAdmin = true
        };
        contexto.Users.Add(usuarioAdmin);
        contexto.SaveChanges();
    }
}

app.Run();