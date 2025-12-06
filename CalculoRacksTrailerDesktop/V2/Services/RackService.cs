using CalculoRacksTrailerDesktop.V2.Models;
using CalculoRacksTrailerDesktop.V2.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CalculoRacksTrailerDesktop.V2.Services
{
    public class RackService
    {
        public AddRackResult AddRack(AddRackRequest request)
        {
            request.Codigo = request.Codigo.Trim().ToUpper();

            if (string.IsNullOrEmpty(request.Codigo))
            {
                return AddRackResult.Failure(ErrorType.CodeEmpty, null, null, null, null);
            }

            if (!request.RackCatalog.TryGetValue(request.Codigo, out Rack? rack) || rack?.Codigo != request.Codigo)
            {
                return AddRackResult.Failure(ErrorType.CodeNotFound, request.Codigo, request.RackCatalog?.Count, null, null);
            }

            if (!int.TryParse(request.UnidadesStr, out int unidades) || unidades <= 0)
            {
                return AddRackResult.Failure(ErrorType.InvalidUnits, request.Codigo, null, null, null);
            }

            if (!TrailerCalculator.UnitFitsSingle(rack.Largo, rack.Ancho, rack.Alto, request.TrailerLargo, request.TrailerAncho, request.TrailerAlto))
            {
                return AddRackResult.Failure(ErrorType.DoesNotFit, request.Codigo, null, rack, null);
            }

            var temp = TrailerCalculator.CloneGroups(request.Groups);

            string key = $"{rack.Largo}x{rack.Ancho}";

            if (!temp.ContainsKey(key))
            {
                temp[key] = new Group(rack.Largo, rack.Ancho);
            }

            for (int i = 0; i < unidades; i++)
            {
                temp[key].UnitHeights.Add(rack.Alto);
            }                

            if (!temp[key].Codes.Contains(request.Codigo))
            {
                temp[key].Codes.Add(request.Codigo);
            }               

            bool ok = TrailerCalculator.TryPlaceAllGroupsOptimized(temp, request.TrailerLargo, request.TrailerAncho, request.TrailerAlto, out string reason, request.Strategy);

            if (!ok)
            {
                return AddRackResult.Failure(ErrorType.PlacementFailed, request.Codigo, null, null, reason);
            }

            // Success:
            string desc = !string.IsNullOrEmpty(rack.Descripcion) ? $" ({rack.Descripcion})" : string.Empty;
            string message = $"✓ {unidades}x {request.Codigo}{desc} - {rack.Largo}×{rack.Ancho}×{rack.Alto}mm{Environment.NewLine}";

            if (!string.IsNullOrEmpty(reason))
            {
                message += $"  {reason}{Environment.NewLine}";
            }

            return AddRackResult.Success(message: message, updatedGroups: temp);
        }
    }
}
