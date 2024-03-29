using MvcNorthwindJatko.Models;
using MvcNorthwindJatko.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcNorthwindJatko.Controllers
{
    public class GraphicController : Controller
    {
        private NorthwindOriginalEntities db = new NorthwindOriginalEntities();

        // GET: Graphic
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Categorysales() 
        {
            string categoryNamelist;
            string categorySalesList;

            List<CategorySalesClass> CategorySalesList = new List<CategorySalesClass>();
            var categorySalesData = from cs in db.Category_Sales_for_1997
                                    select cs;
            foreach (Category_Sales_for_1997 salesfor1997 in categorySalesData)
            {
                CategorySalesClass OneSalesRow = new CategorySalesClass();
                OneSalesRow.CategoryName = salesfor1997.CategoryName;
                OneSalesRow.CategorySales = (int?)(salesfor1997.CategorySales);
                CategorySalesList.Add(OneSalesRow);
            }
            categoryNamelist = "'" + string.Join("','", CategorySalesList.Select(n=> n.CategoryName).ToList()) + "'";
            categorySalesList = string.Join(",", CategorySalesList.Select(n => n.CategorySales).ToList());
            ViewBag.categoryName = categoryNamelist;
            ViewBag.categorySales = categorySalesList;


            return View();
        }
    }
}