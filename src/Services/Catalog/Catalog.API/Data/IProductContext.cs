using Catalog.API.Entities;
using MongoDB.Driver;


namespace Catalog.API.Data
{
    public interface IProductContext
    {
        IMongoCollection<Product> Products { get; }
    }
}
