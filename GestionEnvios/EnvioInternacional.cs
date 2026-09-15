using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionEnvios
{
    internal class EnvioInternacional:Envio
    {
        public double Arancel { get; set; }

        public EnvioInternacional(int codigo, string cliente, double peso, double arancel)
            : base(codigo, cliente, peso)
        {
            Arancel = arancel;
        }

        public override double CalcularCosto() => (peso * 3.50) + Arancel;
    }
}
