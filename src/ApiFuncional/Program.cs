using ApiFuncional.Data;
using ApiFuncional.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()  
    .ConfigureApiBehaviorOptions(options =>//suprimindo validação de entidade do asp.net
    { 
        options.SuppressModelStateInvalidFilter = true;//Se a model estiver num formato inválido, suprima(ignore)
    
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>//config swagger
{//suporte a jwt no swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme//tipo de token, esquema de segurança do OpenApi
    {
        Description = "Insira o token JWT desta maneira: Bearer {seu token}",
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddDbContext<ApiDbContext>(options => //Suporte ao db Context
{ 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); 
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()//Informando uso do Identity - Autenticaçãoe Autorização
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApiDbContext>();

// Pegando o Token e gerando a chave encodada
var JwtSettingsSection = builder.Configuration.GetSection("JwtSettings");//pegando dados especificados na app settings/configuration section
builder.Services.Configure<JwtSettings>(JwtSettingsSection);//Configurando JWT com os dados da minha section

var jwtSettings = JwtSettingsSection.Get<JwtSettings>(); //instancia da minha classe populada/buscando
var key = Encoding.ASCII.GetBytes(jwtSettings.Segredo);//criada chave //encoding de uma sequencia de bytes da minha chave

builder.Services.AddAuthentication(options =>//Adicionando e Configurando autenticação
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;//esquema padrão de autenticação vai ser JWT
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//desafio par verificar se o token vai ser válido
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;//preciso trabalhar dentro de https
    options.SaveToken = true;//permitir que o token seja salvo apos uma uma autenticação com sucesso
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audiencia,
        ValidIssuer = jwtSettings.Emissor
    };
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();//Informando uso

app.UseAuthorization();

app.MapControllers();

app.Run();
