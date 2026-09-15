using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEnvios
{
    internal class EnvioExpress:Envio
    {
        public EnvioExpress(int codigo, string cliente, double peso)
                    : base(codigo, cliente, peso) { }

        public override double CalcularCosto() => (peso * 1.25) + 7.50;
    }
}
