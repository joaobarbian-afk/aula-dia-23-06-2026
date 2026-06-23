using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BancadDigitalAPI.Models;

namespace BancadDigitalAPI.Services
{
    public class BancadaState
    {
        // Guarda sempre a ÚLTIMA leitura recebida do sensor
        public EstadoBancada Atual { get; set; } = new EstadoBancada();
    }
}