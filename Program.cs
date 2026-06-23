using BancadDigitalAPI.Data;
using BancadDigitalAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// 1. Avisa o sistema de que vamos usar classes "Controllers" para responder a URLs HTTP
builder.Services.AddControllers();
// 2. Configura a ligação à Base de Dados SQL Server usando a ConnectionString do appsettings.json
builder.Services.AddDbContext<AppDbContext>(opcoes =>
opcoes.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// 3. Regista o estado em Memória (RAM) como Instância Única (Singleton)
builder.Services.AddSingleton<BancadaState>();
// 4. Regista o Worker do MQTT para correr em background
builder.Services.AddHostedService<MqttWorker>();
var app = builder.Build();
// 5. Atalho para laboratório: Cria a base de dados e tabelas automaticamente ao arrancar a aplicação se não existirem
using (var scope = app.Services.CreateScope()) {
scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}
// 6. Permite que o servidor entregue ficheiros HTML/CSS/JS colocados na pasta "wwwroot"
app.UseDefaultFiles();
app.UseStaticFiles();
// 7. Mapeia automaticamente todas as rotas criadas nos nossos [ApiControllers]
app.MapControllers();
// 8. Inicia efetivamente o servidor web na porta 5000/5001
app.Run();