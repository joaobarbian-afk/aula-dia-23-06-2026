using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BancadDigitalAPI.Data;
using BancadDigitalAPI.Models;
using BancadDigitalAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancadDigitalAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BancadaController : ControllerBase
    {
        private readonly BancadaState _estadoGlobal;
        private readonly AppDbContext _db;
        // Nos Controllers, podemos injetar a Base de Dados diretamente porque o Controller "morre" após cada pedido.
        public BancadaController(BancadaState estadoGlobal, AppDbContext db)
        {
            _estadoGlobal = estadoGlobal;
            _db = db;
        }
        // Endpoint: GET /api/bancada/atual
        [HttpGet("atual")]
        public ActionResult<EstadoBancada> GetAtual()
        {
            // Retorna a variável em RAM (Tempo de resposta ~1ms)
            return Ok(_estadoGlobal.Atual);
        }
        // Endpoint: GET /api/bancada/historico
        [HttpGet("historico")]
        public async Task<ActionResult<IEnumerable<RegistroBancada>>> GetHistorico()
        {
            // Vai ao SQL Server, ordena da mais recente para a mais antiga, e devolve apenas os últimos 50 registos.
            var dados = await _db.Historico.OrderByDescending(r => r.DataHora).Take(50).ToListAsync();
            return Ok(dados);
        }
    }
}