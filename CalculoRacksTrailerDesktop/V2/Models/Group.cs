using System;
using System.Collections.Generic;
using System.Text;

namespace CalculoRacksTrailerDesktop.V2.Models
{
    /// <summary>
    /// Representa un grupo de unidades que comparten la misma huella en planta (largo x ancho). 
    /// Cada grupo almacena las alturas de las unidades individuales y los códigos identificadores asociados.
    /// </summary>
    /// <remarks>
    /// Las dimensiones <see cref="Largo"/> y <see cref="Ancho"/> suelen expresarse en milímetros.
    /// Las alturas dentro de <see cref="UnitHeights"/> también se esperan en milímetros.
    /// </remarks>
    public class Group
    {
        /// <summary>
        /// Obtiene el largo (profundidad) de la unidad del grupo, en milímetros.
        /// </summary>
        public double Largo { get; }

        /// <summary>
        /// Obtiene el ancho (anchura) de la unidad del grupo, en milímetros.
        /// </summary>
        public double Ancho { get; }

        /// <summary>
        /// Lista de alturas de cada unidad perteneciente al grupo, en milímetros.
        /// Cada entrada representa la altura de una unidad individual que se apila al crear torres en el tráiler.
        /// </summary>
        public List<double> UnitHeights { get; } = new List<double>();

        /// <summary>
        /// Códigos identificadores (por ejemplo referencias de racks) asociados a las unidades del grupo. Se usa para seguimiento y referencias en salida.
        /// </summary>
        public List<string> Codes { get; } = new List<string>();

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="Group"/> con las dimensiones de planta especificadas.
        /// </summary>
        /// <param name="largo">Largo (profundidad) de la unidad en milímetros. Debe ser mayor que 0.</param>
        /// <param name="ancho">Ancho (anchura) de la unidad en milímetros. Debe ser mayor que 0.</param>
        public Group(double largo, double ancho)
        {
            Largo = largo;
            Ancho = ancho;
        }
    }
}
