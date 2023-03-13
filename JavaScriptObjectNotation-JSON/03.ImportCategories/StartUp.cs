using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            string jsonFilePath = File.ReadAllText("../../../Datasets/categories.json");

            Console.WriteLine(ImportCategories(context, jsonFilePath));
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = context.Users;

            var data = JsonConvert.DeserializeObject<List<User>>(inputJson);

            users.AddRange(data);
            context.SaveChanges();

            return $"Successfully imported {data.Count}";
        }

        //public static string ImportProducts(ProductShopContext context, string inputJson)
        //{
        //    var data = JsonConvert.DeserializeObject<List<ProductDto>>(inputJson);

        //    ICollection<Product> products = new List<Product>();

        //    foreach (var pDto in data)
        //    {
        //        products.Add(mapper.Map<Product>(pDto));
        //    }

        //    context.Products.AddRange(products);
        //    context.SaveChanges();

        //    return $"Successfully imported {products.Count}";
        //}

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

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
    }
}