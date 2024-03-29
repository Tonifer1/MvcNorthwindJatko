using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace MvcNorthwindJatko.ViewModels
{
    public class CategorySalesClass
    {
        public string CategoryName { get; set; }
        public decimal? CategorySales {  get; set; }

        public string CategoryIDCategoryName { get; set; }
    }
}