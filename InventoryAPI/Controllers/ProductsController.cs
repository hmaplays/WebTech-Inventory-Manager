using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryAPI.Controllers
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMongoCollection<Product> _productsCollection;

        public ProductsController()
        {
            var client = new MongoClient("mongodb+srv://hma_proj:HMAxBSIT-Air@m0.mfc5hzt.mongodb.net/?appName=M0");
            var database = client.GetDatabase("InventoryDB");
            _productsCollection = database.GetCollection<Product>("Products");
        }

        [HttpGet]
        public async Task<List<Product>> Get() => 
            await _productsCollection.Find(_ => true).ToListAsync();

        [HttpGet("{id}")]
        public async Task<Product> Get(string id) => 
            await _productsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        [HttpPost]
        public async Task<IActionResult> Post(Product newProduct)
        {
            await _productsCollection.InsertOneAsync(newProduct);
            return CreatedAtAction(nameof(Get), new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Product updatedProduct)
        {
            updatedProduct.Id = id;
            await _productsCollection.ReplaceOneAsync(x => x.Id == id, updatedProduct);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _productsCollection.DeleteOneAsync(x => x.Id == id);
            return NoContent();
        }
    }
}