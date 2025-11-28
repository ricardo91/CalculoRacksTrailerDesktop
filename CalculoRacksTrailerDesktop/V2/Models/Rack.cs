using System;
using System.Collections.Generic;
using System.Text;

namespace CalculoRacksTrailerDesktop.V2.Models
{
    /// <summary>
    /// Clase para almacenar datos de los racks del catálogo
    /// </summary>
    public class Rack
    {
        public string Codigo { get; set; } = string.Empty;
        public double Largo { get; set; }
        public double Ancho { get; set; }
        public double Alto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
