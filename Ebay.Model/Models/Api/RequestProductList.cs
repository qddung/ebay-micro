using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebay.Model.Models.Api
{

    public class RequestProductList
    {
        public RequestProductList()
        {
            PageRequest = new PagingRequest();
        }
        public PagingRequest PageRequest { get; set; }
        public int? CategoryId { get; set; }
        public string? Keyword { get; set; }
        public EOrderBy OrderBy { get; set; }
    }
}
