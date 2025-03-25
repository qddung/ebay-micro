using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebay.Model.Models.Api
{

    public class RequestProductList
    {
        public int? CategoryId { get; set; }
        public EOrderBy OrderBy { get; set; }
    }
}
