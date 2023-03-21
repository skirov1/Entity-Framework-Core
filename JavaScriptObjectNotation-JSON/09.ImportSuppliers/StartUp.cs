using AutoMapper;
using CarDealer.Data;
using CarDealer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            string jsonFilePath = File.ReadAllText("../../../Datasets/suppliers.json");

            Console.WriteLine(ImportSuppliers(context, jsonFilePath));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = context.Suppliers;

            var importSuppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            suppliers.AddRange(importSuppliers);
            context.SaveChanges();

            return $"Successfully imported {importSuppliers.Length}.";
        }

        private static IMapper CreateMapper()
        {
            return new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));
        }

        private static IContractResolver ConfigureCamelCaseNaming()
        {
            return new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy(false, true)
            };
        }
    }
}