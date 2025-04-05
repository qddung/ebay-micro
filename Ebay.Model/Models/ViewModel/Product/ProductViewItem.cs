using Ebay.Model.Models.Api;

namespace Ebay.Model.Models.ViewModel.Product
{
    public class ProductViewItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageView { get; set; }
        public List<OptionItem<string>> Images { get; set; }
        public int ViewCount { get; set; }
        public decimal Price { get; set; }
    }
}