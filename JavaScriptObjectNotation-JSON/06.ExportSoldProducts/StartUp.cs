using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            Console.WriteLine(GetSoldProducts(context));
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = context.Users;

            var data = JsonConvert.DeserializeObject<List<User>>(inputJson);

            users.AddRange(data);
            context.SaveChanges();

            return $"Successfully imported {data.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            ProductDto[] productDtos =
                JsonConvert.DeserializeObject<ProductDto[]>(inputJson);
            Product[] products = mapper.Map<Product[]>(productDtos);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            var data = JsonConvert.DeserializeObject<List<CategoryDto>>(inputJson);

            ICollection<Category> categories = new List<Category>();

            foreach (var cDto in data)
            {
                if (cDto.Name == null)
                {
                    continue;
                }
                categories.Add(mapper.Map<Category>(cDto));
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            var data = JsonConvert.DeserializeObject<List<CategoryProductDto>>(inputJson);

            ICollection<CategoryProduct> categoriesProducts = new List<CategoryProduct>();

            foreach (var cpDto in data)
            {
                categoriesProducts.Add(mapper.Map<CategoryProduct>(cpDto));
            }

            context.CategoriesProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            IMapper mapper = CreateMapper();

            var products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .ProjectTo<ExportProductsInRange>(mapper.ConfigurationProvider)
                .ToList();

            var json = JsonConvert.SerializeObject(products, Formatting.Indented);

            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            IContractResolver contractResolver = ConfigureCamelCaseNaming();

            var users = context
                .Users
                .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    SoldProducts = u.ProductsSold
                        .Where(ps => ps.Buyer != null)
                        .Select(ps => new
                        {
                            ps.Name,
                            ps.Price,
                            BuyerFirstName = ps.Buyer.FirstName,
                            BuyerLastName = ps.Buyer.LastName
                        })
                        .ToArray()
                })
                .AsNoTracking()
                .ToArray();


            return JsonConvert.SerializeObject(users,
               Formatting.Indented,
               new JsonSerializerSettings()
               {
                   ContractResolver = contractResolver
               });
        }

        private static IMapper CreateMapper()
        {
            return new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
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