using CalculoRacksTrailerDesktop.V2.Services;
using CalculoRacksTrailerDesktop.V2.Models;
using CalculoRacksTrailerDesktop.V2.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CalculoRacksTrailerDesktop.Tests.V2.Services
{
    [TestClass]
    public class RackServiceTests
    {
        private RackService? service;

        [TestInitialize]
        public void Setup()
        {
            service = new RackService();
        }

        private Dictionary<string, Rack> GetSampleCatalog()
        {
            return new Dictionary<string, Rack>
            {
                ["R1"] = new Rack
                {
                    Codigo = "R1",
                    Largo = 100,
                    Ancho = 50,
                    Alto = 30,
                    Descripcion = "Test Rack"
                }
            };
        }

        private AddRackRequest BuildRequest(string codigo, string unidades, 
                                            Dictionary<string, CalculoRacksTrailerDesktop.V2.Models.Group> groups, 
                                            PlacementStrategy strategy, Dictionary<string, Rack>? catalog = null)
        {
            return new AddRackRequest
            {
                Codigo = codigo,
                UnidadesStr = unidades,
                TrailerLargo = 13600,
                TrailerAncho = 2500,
                TrailerAlto = 2900,
                Groups = groups,
                RackCatalog = catalog ?? GetSampleCatalog(),
                Strategy = strategy
            };
        }

        [TestMethod]
        public void AddRack_ShouldReturn_CodeEmpty_WhenCodigoIsEmpty()
        {
            var req = BuildRequest("", "1", new Dictionary<string, CalculoRacksTrailerDesktop.V2.Models.Group>(), PlacementStrategy.GreedyByWidth);

            var result = service.AddRack(req);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ErrorType.CodeEmpty, result.ErrorType);
        }

        [TestMethod]
        public void AddRack_ShouldReturn_CodeNotFound_WhenCodigoNotInCatalog()
        {
            var req = BuildRequest("XYZ", "1", new Dictionary<string, CalculoRacksTrailerDesktop.V2.Models.Group>(), PlacementStrategy.GreedyByWidth);

            var result = service.AddRack(req);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ErrorType.CodeNotFound, result.ErrorType);
        }

        [TestMethod]
        public void AddRack_ShouldReturn_InvalidUnits_WhenUnitsAreNotNumberOrNonPositive()
        {
            var req1 = BuildRequest("R1", "zero", new Dictionary<string, CalculoRacksTrailerDesktop.V2.Models.Group>(), PlacementStrategy.GreedyByWidth);
            var res1 = service.AddRack(req1);
            Assert.IsFalse(res1.IsSuccess);
            Assert.AreEqual(ErrorType.InvalidUnits, res1.ErrorType);

            var req2 = BuildRequest("R1", "0", new Dictionary<string, CalculoRacksTrailerDesktop.V2.Models.Group>(), PlacementStrategy.GreedyByWidth);
            var res2 = service.AddRack(req2);
            Assert.IsFalse(res2.IsSuccess);
            Assert.AreEqual(ErrorType.InvalidUnits, res2.ErrorType);
        }

        [TestMethod]
        public void AddRack_ShouldReturn_DoesNotFit_WhenRackTooLarge()
        {
            // Use dimensions that won't fit a tiny trailer
            var catalog = new Dictionary<string, Rack>
            {
                // TrailerLargo = 13600, TrailerAncho = 2500, TrailerAlto = 2900,
                ["R1"] = new Rack { Codigo = "R1", Largo = 13601, Ancho = 1000, Alto = 1000 },
                ["R2"] = new Rack { Codigo = "R2", Largo = 1000, Ancho = 2501, Alto = 1000 },
                ["R3"] = new Rack { Codigo = "R3", Largo = 1000, Ancho = 1000, Alto = 2901 }
            };

            var request1 = BuildRequest("R1", "1", new Dictionary<string, CalculoRacksTrailerDesktop.V2.Models.Group>(), PlacementStrategy.GreedyByWidth, catalog);
            var result1 = service.AddRack(request1);
            Assert.IsFalse(result1.IsSuccess);
            Assert.AreEqual(ErrorType.DoesNotFit, result1.ErrorType);

            var request2 = BuildRequest("R2", "1", new Dictionary<string, CalculoRacksTrailerDesktop.V2.Models.Group>(), PlacementStrategy.GreedyByWidth, catalog);
            var result2 = service.AddRack(request2);
            Assert.IsFalse(result2.IsSuccess);
            Assert.AreEqual(ErrorType.DoesNotFit, result2.ErrorType);

            var request3 = BuildRequest("R3", "1", new Dictionary<string, CalculoRacksTrailerDesktop.V2.Models.Group>(), PlacementStrategy.GreedyByWidth, catalog);
            var result3 = service.AddRack(request3);
            Assert.IsFalse(result3.IsSuccess);
            Assert.AreEqual(ErrorType.DoesNotFit, result3.ErrorType);
        }

        [TestMethod]
        public void AddRack_ShouldReturn_PlacementFailed_WhenPlacementFails()
        {
            // Simulate placement fail by feeding groups that can't be placed
            var groups = new Dictionary<string, CalculoRacksTrailerDesktop.V2.Models.Group>();
            var catalog = GetSampleCatalog();
            var req = BuildRequest("R1", "3", groups, PlacementStrategy.GreedyByWidth, catalog);

            // We choose unrealistic trailer dimensions so that placement fails
            req.TrailerLargo = 100;
            req.TrailerAncho = 100;
            req.TrailerAlto = 30;

            var result = service.AddRack(req);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(ErrorType.PlacementFailed, result.ErrorType);
        }

        [TestMethod]
        public void AddRack_ShouldReturn_SuccessAndUpdateGroups_WhenInputIsValid()
        {
            var groups = new Dictionary<string, CalculoRacksTrailerDesktop.V2.Models.Group>();
            var req = BuildRequest("R1", "2", groups, PlacementStrategy.GreedyByWidth);

            var result = service.AddRack(req);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.UpdatedGroups);
            Assert.IsTrue(result.UpdatedGroups!.ContainsKey("100x50"));
            Assert.AreEqual(2, result.UpdatedGroups["100x50"].UnitHeights.Count);
            Assert.IsTrue(result.UpdatedGroups["100x50"].Codes.Contains("R1"));
        }
    }
}
