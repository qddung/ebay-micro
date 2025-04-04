using System;
using Ebay.Blazor.Data.ServiceUltil;
using Ebay.Model.Models.Api;
using Ebay.Model.Models.ViewModel.Product;
namespace Ebay.Blazor.Data;

public class EbayProductState
{
    public HttpBackendRequestService _client { get; set; }
    public EbayProductState(HttpBackendRequestService httpService)
    {
        _client = httpService;
    }
    public RequestProductList RequestModel { get; set; } = new RequestProductList()
    {
        CategoryId = null,
        Keyword = null,
        OrderBy = Model.Models.EOrderBy.Asc,
        PageRequest = new PagingRequest()
        {
            PageIndex = 1,
            PageSize = 10,
        },
    };

    private PagingData PagingData { get; set; } = new PagingData()
    {
        PageIndex = 1,
        PageSize = 10,
    };

    private List<OptionItem<int?>> ListCategory { get; set; } = new List<OptionItem<int?>>();

    private List<ProductViewItem> Products { get; set; } = new List<ProductViewItem>();

    public List<ProductViewItem> GetProduct() => Products;
    public List<OptionItem<int?>> GetCategory() => ListCategory;

    public async Task GetData()
    {
        var pageModel = await _client.PostRequest<PagingData<ProductViewItem>, RequestProductList>
                            ("/api/Product/GetProductWithRequestModel", RequestModel);
        var cateGory = await _client.GetRequest<List<OptionItem<int?>>>
                            ("/api/Category/GetCategoryOption");
        // Update Category
        ListCategory = cateGory.Data;

        // Update List Data
        var ack = pageModel;
        var data = ack.Data;
        var dataList = data.DataList;
        Products = dataList;

        // Update Paging 
        PagingData = new PagingData()
        {
            PageIndex = data.PageIndex,
            PageSize = data.PageSize,
            TotalItem = data.TotalItem,
            TotalPage = data.TotalPage,
        };

        // State Change
        NotifyStateChanged();
    }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}