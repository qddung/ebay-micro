using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebay.Model.Models.Api
{
    public class ResponseHttpModel
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public DateTime dataTime { get; set; }
    }

    public class ResponseHttpModel<T> : ResponseHttpModel
    {

        public T content { get; set; }
    }
}
