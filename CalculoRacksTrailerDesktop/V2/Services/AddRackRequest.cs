using CalculoRacksTrailerDesktop.V2.Models;
using CalculoRacksTrailerDesktop.V2.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculoRacksTrailerDesktop.V2.Services
{
    public class AddRackRequest
    {
        public string Codigo { get; set; } = string.Empty;
        public string UnidadesStr { get; set; } = string.Empty;

        public double TrailerLargo { get; set; }
        public double TrailerAncho { get; set; }
        public double TrailerAlto { get; set; }

        public Dictionary<string, Group> Groups { get; set; } = new();
        public Dictionary<string, Rack> RackCatalog { get; set; } = new();

        public PlacementStrategy Strategy { get; set; } = PlacementStrategy.GreedyByWidth;
    }
}
