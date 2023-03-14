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

            string jsonFilePath = File.ReadAllText("../../../Datasets/categories-products.json");

            Console.WriteLine(ImportCategoryProducts(context, jsonFilePath));
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

            var data = JsonConvert.DeserializeObject<List<ProductDto>>(inputJson);

            List<Product> products = new List<Product>();

            foreach (var pDto in data)
            {
                if (pDto.BuyerId == null)
                {
                    continue;
                }
                products.Add(mapper.Map<Product>(pDto));
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
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

        private static IMapper CreateMapper()
        {
            return new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));
        }
    }
}