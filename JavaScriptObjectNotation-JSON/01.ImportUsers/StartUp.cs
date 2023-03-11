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
            using (ProductShopContext context = new ProductShopContext())
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProductShopProfile>();
                });

                var mapper = config.CreateMapper();


                string jsonFilePath = File.ReadAllText("../../../Datasets/users.json");

                Console.WriteLine(ImportUsers(context, jsonFilePath));
            }
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = context.Users;

            var data = JsonConvert.DeserializeObject<List<User>>(inputJson);

            users.AddRange(data);
            context.SaveChanges();

            return $"Successfully imported {data.Count}";
        }
    }
}