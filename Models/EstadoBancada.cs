using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BancadDigitalAPI.Models
{
    public class EstadoBancada
    {
        // Percentagem do nível de água no tanque (Ex: 75.5)
        public double Nivel { get; set; }
        // Indica se a válvula de escoamento ou enchimento está ativa
        public bool ValvulaAberta { get; set; }
    }
}