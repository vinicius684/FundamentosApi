using ApiFuncional.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()  
    .ConfigureApiBehaviorOptions(options =>//suprimindo validação de entidade do asp.net
    { 
        options.SuppressModelStateInvalidFilter = true;//Se a model estiver num formato inválido, suprima(ignore)
    
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiDbContext>(options => //Suporte ao db Context
{ 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); 
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()//Informando uso do Identity - Autenticaçãoe Autorização
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApiDbContext>();
    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();//

app.UseAuthorization();

app.MapControllers();

app.Run();
