using System;
using Blazorise;
using Ebay.Blazor.Data.ServiceUltil;
using Ebay.Model.Models.Api;
using Ebay.Model.Models.ViewModel.Product;
namespace Ebay.Blazor.Data;

public class EbayProductState
{
    public HttpBackendRequestService _client { get; set; }
    public INotificationService _notificationService { get; set; }
    public EbayProductState(HttpBackendRequestService httpService, INotificationService notificationService)
    {
        _client = httpService;
        _notificationService = notificationService;
    }
    public RequestProductList RequestModel { get; set; } = new RequestProductList()
    {
        CategoryId = null,
        Keyword = null,
        OrderBy = Model.Models.EOrderBy.AscPrice,
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

    public bool ShouldLoadMore()
    {
        if (PagingData.PageIndex <= PagingData.TotalPage)
        {
            return true;
        }
        return false;
    }

    private List<OptionItem<int?>> ListCategory { get; set; } = new List<OptionItem<int?>>();

    private List<ProductViewItem> Products { get; set; } = new List<ProductViewItem>();

    public List<ProductViewItem> GetProduct() => Products;
    public List<OptionItem<int?>> GetCategory() => ListCategory;

    private void ShowNotification(string notification)
    {
        _notificationService.Warning(notification);
    }

    public async Task LoadDataModel()
    {
        var cateGory = await _client.GetRequest<List<OptionItem<int?>>>
                                    ("/api/Category/GetCategoryOption");
        if (cateGory.IsSuccess == false)
        {
            // ShowNotification(cateGory.Message);
            return;
        }
        // Update Category
        ListCategory = cateGory.Data;
        NotifyStateChanged();
    }

    public async Task GetData()
    {
        var pageModel = await _client.PostRequest<PagingData<ProductViewItem>, RequestProductList>
                            ("/api/Product/GetProductWithRequestModel", RequestModel);

        if (pageModel.IsSuccess == false)
        {
            ShowNotification(pageModel.Message);
            return;
        }
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


    public async Task LoadData()
    {
        var pageModel = await _client.PostRequest<PagingData<ProductViewItem>, RequestProductList>
                            ("/api/Product/GetProductWithRequestModel", RequestModel);

        if (pageModel.IsSuccess == false)
        {
            ShowNotification(pageModel.Message);
            return;
        }
        // Update List Data
        var ack = pageModel;
        var data = ack.Data;
        var dataList = data.DataList;
        Products = Products.Concat(dataList).ToList();

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