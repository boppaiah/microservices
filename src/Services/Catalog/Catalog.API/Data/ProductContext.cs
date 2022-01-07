using Catalog.API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Data
{
    public class ProductContext : IProductContext
    {
        private readonly IConfiguration _configuration;
        public IMongoCollection<Product> Products { get; }

        public ProductContext(IConfiguration configuration)
        {
            _configuration = configuration;
            //create a client
            //creates a connection with the mongoDB
            var client = new MongoClient(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            //getting the database
            var database = client.GetDatabase(_configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            //ppopulate the products
            Products = database.GetCollection<Product>(_configuration.GetValue<string>("DatabaseSettings:CollectionName"));

            //seed data 
            ProductContextSeed.SeedData(Products);
           
        }
    }
}
