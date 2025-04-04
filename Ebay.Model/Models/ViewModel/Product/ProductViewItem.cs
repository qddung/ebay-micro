namespace Ebay.Model.Models.ViewModel.Product
{
    public class ProductViewItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public List<string> Images { get; set; }
        public int ViewCount { get; set; }
        public decimal Price { get; set; }
    }
}