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
    public class CategoryController : ControllerBase
    {

        private readonly EBayDbContext EbayContext;

        public CategoryController(EBayDbContext context)
        {
            EbayContext = context;
        }

        [HttpGet("GetCategoryOption")]
        public async Task<ActionResult<List<OptionItem<int?>>>> GetCategoryOption()
        {
            var context = EbayContext;
            var category = await context.Categories.ToListAsync();
            var result = category.Select(i => new OptionItem<int?>()
            {
                Label = i.Name,
                Value = i.Id
            }).ToList();

            var optionAll = new List<OptionItem<int?>>(){
                new OptionItem<int?>(){
                    Label = "All Category",
                    Value = null as int?,
                }
            };
            result = optionAll.Concat(result).ToList();
            return Ok(result);

        }
    }
}
