using CalculoRacksTrailerDesktop.V2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculoRacksTrailerDesktop.V2.Services
{
    public class AddRackResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ShortMessage { get; set; } = string.Empty;
        public Dictionary<string, Group>? UpdatedGroups { get; set; }    
        public ErrorType ErrorType { get; set; }

        private AddRackResult(bool isSuccess, string message, string shortMessage, Dictionary<string, Group>? updatedGroups, ErrorType errorType)
        {
            IsSuccess = isSuccess;
            Message = message;
            ShortMessage = shortMessage;
            UpdatedGroups = updatedGroups;
            ErrorType = errorType;
        }

        public static AddRackResult Success(string? message, Dictionary<string, Group>? updatedGroups)
            => new AddRackResult(true, message ?? string.Empty, string.Empty, updatedGroups, ErrorType.None);

        public static AddRackResult Failure(ErrorType errorType, string? codigo, int? catalogCount, Rack? rack, string? reason)
        {
            string shortMessage = string.Empty;
            string message = string.Empty;

            switch (errorType)
            {
                case ErrorType.CodeEmpty:
                    shortMessage = "Código requerido";
                    message = "Por favor, introduce un código de rack.";
                    break;
                case ErrorType.CodeNotFound:
                    shortMessage = "Código no encontrado";
                    message = $"El código '{codigo}' no se encuentra en el catálogo.{Environment.NewLine}{Environment.NewLine}" +
                              $"Racks disponibles: {catalogCount ?? decimal.Zero}{Environment.NewLine}" +
                              $"Usa el botón '🔍 Ver Catálogo' para ver la lista completa.";
                    break;
                case ErrorType.InvalidUnits:
                    shortMessage = "Unidades inválidas";
                    message = "Introduce un número válido de unidades (mayor a 0).";
                    break;
                case ErrorType.DoesNotFit:
                    message = $"❌ ERROR → El rack {codigo} ({rack?.Largo}×{rack?.Ancho}×{rack?.Alto}) no cabe en el tráiler.{Environment.NewLine}";
                    break;
                case ErrorType.PlacementFailed:
                    message = $"❌ NO CABE → {codigo}: {reason}{Environment.NewLine}";
                    break;
                default:
                    break;
            }

            return new AddRackResult(false, message, shortMessage, null, errorType);
        }
    }

    public enum ErrorType
    {
        CodeEmpty, 
        CodeNotFound, 
        InvalidUnits, 
        DoesNotFit, 
        PlacementFailed,
        None
    }
}
