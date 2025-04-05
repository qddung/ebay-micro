using Ebay.Backend.Entities.Context;
using Microsoft.AspNetCore.Mvc;
using Ebay.Model.Models.ViewModel.Product;
using Ebay.Model.Models.Api;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Ebay.Model.Models;
using System.Web;
namespace Ebay.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {

        private readonly EBayDbContext EbayContext;

        public ProductController(EBayDbContext context)
        {
            EbayContext = context;
        }

        [HttpPost("GetProductWithRequestModel")]
        public async Task<ActionResult<PagingData<ProductViewItem>>> GetProductWithCategory(RequestProductList request)
        {
            var context = EbayContext;

            var pagingResponse = new PagingData<ProductViewItem>()
            {
                PageIndex = request.PageRequest.PageIndex,
                PageSize = request.PageRequest.PageSize,
            };

            var listingCategoryQuery = (
                from cate in context.Categories
                join ls in context.Listings.Where(i => i.Status == "Active") on cate.Id equals ls.CategoryId
                select new
                {
                    cate
                }
            ).Select(i => i.cate.Id).Distinct();

            var categoryQuery = (
                from cate in context.Categories
                join f in listingCategoryQuery on cate.Id equals f
                select cate
            );

            if (request.CategoryId.HasValue)
            {
                var cId = request.CategoryId.Value;
                categoryQuery = categoryQuery.Where(i => cId == i.Id);
            }
            var queryProduct = (
                from p in context.Products.Where(i => i.Deleted == false)
                join cate in categoryQuery on p.CategoryId equals cate.Id
                select p
            );

            if (string.IsNullOrEmpty(request.Keyword) == false)
            {
                queryProduct = queryProduct.Where(i => i.Name.Contains(request.Keyword));
            }

            var totalProduct = await queryProduct.CountAsync();
            if (request.OrderBy == EOrderBy.AscPrice)
            {
                queryProduct = queryProduct.OrderBy(i => i.Price);
            }
            else
            {
                queryProduct = queryProduct.OrderByDescending(i => i.Price);
            }

            var pageSize = pagingResponse.PageSize.Value;
            var lsProduct = await queryProduct.Skip(pagingResponse.GetSkipItems()).Take(pageSize).ToListAsync();

            var productIds = lsProduct.Select(i => i.Id).ToList();
            // populate Image
            var prodImageQuery = await context.ProductImages.Where(i => productIds.Contains(i.ProductId)).ToListAsync();
            var dictionaryProdImage = prodImageQuery.GroupBy(i => i.ProductId)
                        .ToDictionary(i => i.Key,
                                    j => j.Select(k => new OptionItem<string>() { Label = k.ImageUrl, Value = k.Id.ToString() })
                        .ToList());


            // populate View
            var proIds = lsProduct.Select(i => (int?)i.Id).ToList();
            var views = await context.Ratings
                                            .Where(i => proIds.Contains(i.ProductId))
                                            .GroupBy(i => i.ProductId, groupSelector => groupSelector,
                                            (key, groupList) => new
                                            {
                                                productId = key,
                                                viewCount = groupList.Count(),
                                            })
                                            .ToListAsync();
            var dictionaryProdView = views.ToDictionary(i => i.productId, j => j.viewCount);


            var prodViews = lsProduct.Select(i =>
            {
                var prodImgs = new List<OptionItem<string>>();
                var view = 0;
                if (dictionaryProdImage.ContainsKey(i.Id))
                {
                    prodImgs = dictionaryProdImage[i.Id].ToList();
                }
                if (dictionaryProdView.ContainsKey(i.Id))
                {
                    view = dictionaryProdView[i.Id];
                }

                return new ProductViewItem()
                {
                    ProductId = i.Id,
                    ProductName = i.Name,
                    ImageView = prodImgs.FirstOrDefault()?.Value?.ToString(),
                    Images = prodImgs,
                    ViewCount = view,
                    Price = i.Price
                };
            }).ToList();

            var result = new PagingData<ProductViewItem>()
            {
                DataList = prodViews,
                PageIndex = pagingResponse.PageIndex,
                PageSize = pagingResponse.PageSize,
                TotalItem = totalProduct
            };
            return Ok(result);

        }
    }
}
