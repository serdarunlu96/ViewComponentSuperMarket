namespace ANK13SuperMarket.Models
{
    public class ProductViewModel
    {
        public string Name { get; set; }

        public double Price { get; set; }

        public bool IsInStock { get; set; }

        public IFormFile Image { get; set; }
    }
}
