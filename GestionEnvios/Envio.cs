using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GestionEnvios
{
    // Especificamos los tipos derivados para la serialización/deserialización
    [JsonDerivedType(typeof(EnvioNacional), typeDiscriminator: "nacional")]
    [JsonDerivedType(typeof(EnvioExpress), typeDiscriminator: "express")]
    [JsonDerivedType(typeof(EnvioInternacional), typeDiscriminator: "internacional")]
    abstract class Envio
    {
        // Atributo protegido exigido por el enunciado
        protected double peso;

        public int Codigo { get; set; }
        public string Cliente { get; set; }
        public double Peso
        {
            get => peso;
            set => peso = value;
        }
        // Le indicamos a JSON que use este constructor al deserializar
        [JsonConstructor]
        public Envio(int codigo, string cliente, double peso)
        {
            Codigo = codigo;
            Cliente = cliente;
            this.peso = peso;
        }

        // Método abstracto a redefinir
        abstract public double CalcularCosto();
    }
}
