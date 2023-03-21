using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
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

            string jsonFilePath = File.ReadAllText(@"../../../Datasets/parts.json");

            Console.WriteLine(ImportParts(context, jsonFilePath));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = context.Suppliers;

            var importSuppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            suppliers.AddRange(importSuppliers);
            context.SaveChanges();

            return $"Successfully imported {importSuppliers.Length}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            ImportPartDto[] partsDtos = JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson);

            ICollection<Part> validParts = new HashSet<Part>();

            var validIds = context.Suppliers.Select(s => s.Id).ToList();

            foreach (var partDto in partsDtos)
            {
                if (!validIds.Contains(partDto.SupplierId))
                {
                    continue;
                }

                Part part = mapper.Map<Part>(partDto);
                validParts.Add(part);
            }
            context.AddRange(validParts);
            context.SaveChanges();

            return $"Successfully imported {validParts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = context.Cars;

            var importCars = JsonConvert.DeserializeObject<Car[]>(inputJson);

            cars.AddRange(importCars);
            context.SaveChanges();

            return $"Successfully imported {importCars.Length}.";
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