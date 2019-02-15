using ShoppingCMSWebsite.Models.Data;
using ShoppingCMSWebsite.Models.ViewModel;
using ShoppingCMSWebsite.Models.ViewModel.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCMSWebsite.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index","Pages");
        }
        public ActionResult CategoryMenuPartial()
        {
            //declare list of categoryVM
            List<CategoriesVM> categoryVMList;

            //init list
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoriesVM(x)).ToList();
            }
            //Delclare partial with list
            return PartialView(categoryVMList);
        }
        // GET: Shop/Categories/name
        public ActionResult Categories( string name)
        {
            //Declare list of productVM
            List<ProductVM> productVMList;
            using (Db db = new Db ())
            {
                //Get Category ID
                CategoryDTO catDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = catDTO.Id;
                //Init the list
                productVMList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();
                //get Category name
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();
                ViewBag.CategoryName = productCat.CategoryName;
            }
            //Return view with list
            return View(productVMList);
        }
        // GET: Shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            //Declare ProductVM and Product DTO
            ProductVM model;
            ProductDTO dto;
            //Init product ID
            int id = 0;

            using (Db db = new Db ())
            {
                //Check if product exists
                if (! db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                //Init ProductDTO
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                //Get  ID
                id = dto.Id;

                //Init model
                model = new ProductVM(dto);
            }
            //Get gallery images
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                               .Select(fn => Path.GetFileName(fn));

            //Return view with model
            return View("ProductDetails", model);
        }
    }
}