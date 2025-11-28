using System;
using System.Collections.Generic;
using System.Text;

namespace CalculoRacksTrailerDesktop.V2.Models
{
    public class Tower
    {
        public double Largo { get; }
        public double Ancho { get; }
        public double Alto { get; }
        public Tower(double largo, double ancho, double alto)
        {
            Largo = largo;
            Ancho = ancho;
            Alto = alto;
        }
    }
}
