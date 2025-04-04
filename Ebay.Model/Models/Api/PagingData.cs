using System;

namespace Ebay.Model.Models.Api
{

    public static class PagingConfig
    {
        public const int DefaultPageSize = 10;
    }

    public class PagingRequest
    {
        public int PageIndex { get; set; }
        private int? pageSize { get; set; }
        public int? PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = value;
            }
        }
    }


    public class PagingData : PagingRequest
    {

        public int TotalPage { get { return GetPageTotal(); } set { } }


        public int TotalItem { get; set; }
        public PagingData()
        {
            this.PageSize = PagingConfig.DefaultPageSize;
            this.PageIndex = 1;
        }
        public int GetPageTotal()
        {
            if (PageSize.HasValue == false)
            {
                return 1;
            }
            int pageTotal = 0;

            if (TotalItem >= 0 && PageSize > 0)
            {
                pageTotal = (int)(TotalItem / PageSize) + (TotalItem % PageSize > 0 ? 1 : 0);
            }

            return pageTotal;
        }
    }

    public class PagingData<T> : PagingData
    {
        public List<T> DataList { get; set; }
        public int TotalItem { get; set; }

        public PagingData()
        {
            this.DataList = new List<T>();
        }

        public int Skip { get; set; }

        public bool HasMore
        {
            get
            {
                return PageIndex < TotalPage;
            }
        }

        public int GetSkipItems()
        {
            return PageSize.Value * PageIndex;
        }
    }
}