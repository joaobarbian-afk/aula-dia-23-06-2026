using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BancadDigitalAPI.Data;
using BancadDigitalAPI.Models;
using MQTTnet;
using MQTTnet.Client;

namespace BancadDigitalAPI.Services
{
    public class MqttWorker : BackgroundService
    {
        private readonly IMqttClient _mqttClient;
        private readonly BancadaState _estadoGlobal;
        private readonly IServiceScopeFactory _scopeFactory;
        // Injeção de dependências via construtor
        public MqttWorker(BancadaState estadoGlobal, IServiceScopeFactory scopeFactory)
        {
            _estadoGlobal = estadoGlobal;
            _scopeFactory = scopeFactory; // Fábrica para criar instâncias curtas (Scoped) num serviço longo(Singleton)
            _mqttClient = new MqttFactory().CreateMqttClient();
        }
        // Método principal que corre em background de forma contínua
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Configurações de ligação ao broker público HiveMQ
            var options = new MqttClientOptionsBuilder()
            .WithClientId($"BancadaAPI_{Guid.NewGuid()}")
            .WithTcpServer("broker.hivemq.com")
            .Build();
            // Define o que fazer QUANDO uma mensagem chegar
            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {

                // 1. Descodifica os bytes recebidos para uma String de texto (Formato JSON)
                var json = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                try
                {

                    // 2. Converte o JSON para o Objeto C# (EstadoBancada)

                    var estado = JsonSerializer.Deserialize<EstadoBancada>(json, new JsonSerializerOptions
                    { PropertyNameCaseInsensitive = true });
                    if (estado != null)
                    {

                        // 3. Atualiza a memória partilhada de imediato (Para o painel frontal ler sem atrasos)
                        _estadoGlobal.Atual = estado;

                        // 4. IMPORTANTE: Cria um "Micro-Ambiente" isolado para guardar no banco de dados
                        // Isto previne "Memory Leaks" num serviço que corre 24/7
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                            // Prepara o novo registo e adiciona à tabela

                            db.Historico.Add(new RegistroBancada
                            {
                                Nivel = estado.Nivel,
                                ValvulaAberta =
                estado.ValvulaAberta
                            });

                            // Confirma a gravação no SQL Server
                            await db.SaveChangesAsync();
                        }
                    }
                }
                catch { /* Tratamento de erro Omitido para clareza didática */ }
            };
            // Ciclo infinito para manter a aplicação ligada ao Broker MQTT
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_mqttClient.IsConnected)
                {
                    await _mqttClient.ConnectAsync(options, stoppingToken);
                    await _mqttClient.SubscribeAsync("bancada/telemetria"); // O tópico que a máquina está a publicar
                }
                await Task.Delay(5000, stoppingToken); // Tenta religar a cada 5 segundos se a rede falhar
            }
        }
    }
}