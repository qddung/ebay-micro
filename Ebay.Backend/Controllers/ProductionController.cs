using Ebay.Backend.Entities.Context;
using Microsoft.AspNetCore.Mvc;
using Ebay.Model.Models.ViewModel.Product;
using Ebay.Model.Models.Api;
namespace Ebay.Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductionController : ControllerBase
    {

        private readonly EBayDbContext EbayContext;

        public ProductionController(EBayDbContext context)
        {
            EbayContext = context;
        }

        [HttpPost(Name = "GetProductListWithCategory")]
        public IEnumerable<ProductViewItem> GetProductWithCategory(RequestProductList request)
        {
            var context = EbayContext;
            var categoryQuery = context.Categories.AsQueryable();
            if (request.CategoryId.HasValue)
            {
                var cId = request.CategoryId.Value;
                categoryQuery = categoryQuery.Where(i => cId == i.Id);
            }
            var query = (
                from p in context.Products
                join cate in categoryQuery on p.CategoryId equals cate.Id
                join img in context.ProductImages on p.Id equals img.ProductId into join1
                from img in join1.DefaultIfEmpty()
                group img by new { p, cate } into g
                select new
                {
                    ProductImages = g.Select(i => i).ToList(),

                }
            );

            var result = new List<ProductViewItem>();
            return result;

        }
    }
}
