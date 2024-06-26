﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using MvcNorthwindJatko.Models;
using MvcNorthwindJatko.ViewModels;
using PagedList;

namespace MvcNorthwindJatko.Controllers
{
    public class ProductsController : Controller
    {
        private NorthwindOriginalEntities db = new NorthwindOriginalEntities();

        // GET: Products
        public ActionResult Index(string sortOrder,string currentFilter1,string searchString1,string ProductCategory,int? page, int?  pagesize)
        {
            ViewBag.CurrentSort = sortOrder;    /*if sortOrderia ei ole annettu,siirtyy productname _desc jos taas oli valittu, vaihtaa tyhjäksi""*/
            ViewBag.ProductNameSortParm = String.IsNullOrEmpty(sortOrder) ? "productname_desc" : "";
            ViewBag.UnitPriceSortParm = sortOrder == "UnitPrice" ? "UnitPrice_desc" : "UnitPrice";
            
            if(searchString1 != null)
            {
                page = 1;
            }
            else
            {
                searchString1 = currentFilter1;
            }

            ViewBag.currentFilter1 = searchString1;

            
            var tuotteet = db.Products.Include(p => p.Categories).Include(p => p.Suppliers);


            if (!String.IsNullOrEmpty(searchString1))
            {
                tuotteet = tuotteet.Where(p => p.ProductName.Contains(searchString1) || p.Suppliers.CompanyName.Contains(searchString1));
            }

            if (!String.IsNullOrEmpty(ProductCategory) && (ProductCategory != "0"))
            {
                int para = int.Parse(ProductCategory);
                tuotteet = tuotteet.Where(p => p.CategoryID == para);
            }

            if (!String.IsNullOrEmpty(searchString1)) //Jos hakufiltteri on käytössä, 
                                                      //niin käytetään sitä ja sen lisäksi lajitellaan tulokset
            {
                switch (sortOrder)
                {
                    case "productname_desc":
                        tuotteet = tuotteet.Where(p =>
                       p.ProductName.Contains(searchString1)).OrderByDescending(p => p.ProductName);
                        break;
                    case "UnitPrice":
                        tuotteet = tuotteet.Where(p =>
                       p.ProductName.Contains(searchString1)).OrderBy(p => p.UnitPrice);
                        break;
                    case "UnitPrice_desc":
                        tuotteet = tuotteet.Where(p =>
                       p.ProductName.Contains(searchString1)).OrderByDescending(p => p.UnitPrice);
                        break;
                    default:
                        tuotteet = tuotteet.Where(p =>
                       p.ProductName.Contains(searchString1)).OrderBy(p => p.ProductName);
                        break;
                }

                //int para = int.Parse(ProductCategory);

            }
            else if (!String.IsNullOrEmpty(ProductCategory) && (ProductCategory != "0")) //Jos käytössä on tuoteryhmärajaus, niin käytetään sitä ja sen lisäksi lajitellaan tulokset 
            {
                int para = int.Parse(ProductCategory);
                switch (sortOrder)
                {
                    case "productname_desc":
                        tuotteet = tuotteet.Where(p => p.CategoryID == para).OrderByDescending(p => p.ProductName);
                        break;
                    case "UnitPrice":
                        tuotteet = tuotteet.Where(p => p.CategoryID == para).OrderBy(p => p.UnitPrice);
                        break;
                    case "UnitPrice_desc":
                        tuotteet = tuotteet.Where(p => p.CategoryID == para).OrderByDescending(p => p.UnitPrice);
                        break;
                    default:
                        tuotteet = tuotteet.Where(p => p.CategoryID == para).OrderBy(p => p.ProductName);
                        break;
                }
            }

            //Pelkkä lajittelu
            else 
            switch (sortOrder)
            {
                case "productname_desc":
                    tuotteet = tuotteet.OrderByDescending(p =>
                   p.ProductName);
                                        
                    break;
                case "UnitPrice":
                    tuotteet = tuotteet.OrderBy(p => p.UnitPrice);
                    break;
                case "UnitPrice_desc":
                    tuotteet = tuotteet.OrderByDescending(p =>
                    p.UnitPrice);
                    break;
                    default:
                    tuotteet = tuotteet.OrderBy(p =>
                    p.ProductName);
                    break;
            }

            List<Categories> lstCategories = new List<Categories>();
            var categoryList = from cat in db.Categories
                               select cat;
            Categories tyhjaCategory = new Categories();
            tyhjaCategory.CategoryID = 0;
            tyhjaCategory.CategoryName = "";
            tyhjaCategory.CategoryIDCategoryName = " ";
            lstCategories.Add(tyhjaCategory);
            foreach (Categories category in categoryList)
            {
                Categories yksiCategory = new Categories();
                yksiCategory.CategoryID = category.CategoryID;
                yksiCategory.CategoryName = category.CategoryName;
                yksiCategory.CategoryIDCategoryName = category.CategoryID.ToString() + " - " + category.CategoryName;
                //Taulun luokkamääritykseen Models-kansiossa piti lisätä tämä "uusi" kenttä = CategoryIDCategoryName
                lstCategories.Add(yksiCategory);
            }
            ViewBag.CategoryID = new SelectList(lstCategories, "CategoryID", "CategoryIDCategoryName", ProductCategory);//Yhdistäminen






            int pageSize = (pagesize ?? 10);
            int pageNumber = (page ?? 1);
            
                return View(tuotteet.ToPagedList(pageNumber, pageSize));


        } // public ActionResult Index



        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Products products = db.Products.Find(id);
                if (products == null)
                {
                    return HttpNotFound();
                }
                return View(products);
        }

            // GET: Products/Create
            public ActionResult Create()
            {
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
                ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName");
                return View();
            }

            // POST: Products/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued,imagelink")] Products products)
            {
                if (ModelState.IsValid)
                {
                    db.Products.Add(products);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
                ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", products.SupplierID);
                return View(products);
            }

            // GET: Products/Edit/5
            public ActionResult Edit(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Products products = db.Products.Find(id);
                if (products == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
                ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", products.SupplierID);
                return View(products);
            }

            // POST: Products/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued,imagelink")] Products products)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(products).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
                ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", products.SupplierID);
                return View(products);
            }

            // GET: Products/Delete/5
            public ActionResult Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Products products = db.Products.Find(id);
                if (products == null)
                {
                    return HttpNotFound();
                }
                return View(products);
            }

            // POST: Products/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                Products products = db.Products.Find(id);
                db.Products.Remove(products);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult _ProductSalesPerDate(string productName)

        {

            //if (String.IsNullOrEmpty(productName)) productName = "Lakkalikööri";  //debuggausta varten



                List<DailyProductSales> dailyproductsalesList = new List<DailyProductSales>();



            var orderSummary = from pds in db.ProductsDailySales

                               where pds.ProductName == productName

                               orderby pds.OrderDate



                               select new DailyProductSales

                               {

                                   OrderDate = SqlFunctions.DateName("year", pds.OrderDate) + "." + SqlFunctions.DateName("MM", pds.OrderDate) + "." + SqlFunctions.DateName("day", pds.OrderDate),



                                   DailySales = (float)pds.DailySales,

                                   ProductName = pds.ProductName

                               };

            return Json(orderSummary, JsonRequestBehavior.AllowGet);

        }
    }//Public class

}//namespace
