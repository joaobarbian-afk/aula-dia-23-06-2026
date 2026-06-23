using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BancadDigitalAPI.Models
{
    public class RegistroBancada
    {
        // Chave Primária, auto-incrementada pela base de dados
        public int Id { get; set; }
        public double Nivel { get; set; }
        public bool ValvulaAberta { get; set; }
        // Timestamp automático: Regista o exato momento em que o dado chegou ao servidor (Fuso horário UTC para evitar problemas regionais)
        public DateTime DataHora { get; set; } = DateTime.UtcNow;
    }
}