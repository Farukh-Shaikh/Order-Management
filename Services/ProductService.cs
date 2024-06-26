using OrderManagement.Models;

namespace OrderManagement.Services
{
    public interface IProductService
    {
        IEnumerable<ProductModel> GetAllProducts();
        ProductModel GetProductById(int id);
        void AddProduct(ProductModel product);
        void UpdateProduct(int id, ProductModel product);
        void DeleteProduct(int id);
    }

    public class ProductService : IProductService
    {
        private readonly List<ProductModel> _products = new List<ProductModel>();

        public IEnumerable<ProductModel> GetAllProducts()
        {
            return _products;
        }

        public ProductModel GetProductById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public void AddProduct(ProductModel product)
        {
            _products.Add(product);
        }

        public void UpdateProduct(int id, ProductModel product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
            }
        }

        public void DeleteProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
        }
    }
}
