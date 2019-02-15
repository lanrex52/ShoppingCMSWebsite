using PagedList;
using ShoppingCMSWebsite.Models.Data;
using ShoppingCMSWebsite.Models.ViewModel;
using ShoppingCMSWebsite.Models.ViewModel.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ShoppingCMSWebsite.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            // declare list of models
            List<CategoriesVM> categoryList;
            using (Db db = new Db())
            {
                //init the list 
                categoryList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoriesVM(x)).ToList();
            }


            //return view with list
            return View(categoryList);
        }
        // Post: Admin/Shop/Categories
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //Declare Id
            string id;
            using (Db db = new Db())
            {

                //Check that categogry name is unique

                if (db.Categories.Any(x => x.Name == catName))
                 return "titletaken";
                
            
                //Init DTO
                CategoryDTO dto = new CategoryDTO();
                //Add DTO
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                //Save DTO
                db.Categories.Add(dto);
                db.SaveChanges();

                //Get ID
                id = dto.Id.ToString();
            }
            // return Id
            return id;
        }
        // Post: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                // set initial count
                int count = 1;

                //declare tblPage
                CategoryDTO dto;
                //set sorting for each category 
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }


            }
        }
        // GET: Admin/Shop/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {


                //Get the category
                CategoryDTO dto = db.Categories.Find(id);

                //Remove the catrgory
                db.Categories.Remove(dto);
                //save
                db.SaveChanges();
            }
            //TempData

            TempData["SucessMessage"] = "Page Deleted Successfully";
            //Redirect
            return RedirectToAction("Categories");
        }
        // Post: Admin/Shop/RenameCategory
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                //Check category name is unique
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";
                //Get DTO
                CategoryDTO dto = db.Categories.Find(id);

                //Edit DTO
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                //Save
                db.SaveChanges();

            }
            //Return
            return "OK!";

        }
        // GET: Admin/Shop/AddProduct/
        [HttpGet]
        public ActionResult AddProduct()
        {
            //Initialize Model
            ProductVM model = new ProductVM();
            using (Db db = new Db())
            {

                //Add Select list of categories to model
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

            }
            //Return View with model

            return View(model);
        }
        // Post: Admin/Shop/AddProduct/
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            //Check model state
            if (! ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name"); 
                     
                    return View(model);
                }
               
            }
            // Make sure product name is unique
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name) )
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "That Product Name is Taken");

                    return View(model);
                }
               
            }

            //Declare Product Id
            int id;

            //Init and Save Product DTO
            using (Db db = new Db())
            {
                ProductDTO product = new ProductDTO();

                product.Name = model.Name;
               // if (product.Slug == " ")
               // {
                    product.Slug = model.Name.Replace(" ", "-").ToLower();


               // }
                //else
                //{
                //    product.Slug = model.Slug.Replace(" ", "-").ToLower();
                //}


               

             
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                    model.CategoryName = catDTO.Name;

               
               
               
               

                db.Products.Add(product);
                db.SaveChanges();

                //Get inserted Id
                id = product.Id;

                   }


            //Set TempData Message
            
            TempData["SuccessMessage"] = "You Have Added a New Product!";

            #region Upload Image
            //create necessary directories
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "products\\" + id.ToString() + "\\Thumbs" );
            var pathString4 = Path.Combine(originalDirectory.ToString(), "products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "products\\" + id.ToString() + "\\Gallery\\Thumbs");
             
            if (!Directory.Exists(pathString1))
            {
                Directory.CreateDirectory(pathString1);
            }
            if (!Directory.Exists(pathString2))
            {
                Directory.CreateDirectory(pathString2);
            }
            if (!Directory.Exists(pathString3))
            {
                Directory.CreateDirectory(pathString3);
            }
            if (!Directory.Exists(pathString4))
            {
                Directory.CreateDirectory(pathString4);
            }
            if (!Directory.Exists(pathString5))
            {
                Directory.CreateDirectory(pathString5);
            }
            //Check if file was uploaded

            if (file != null && file.ContentLength > 0)
            {
                //Get file extension
                string ext = file.ContentType.ToLower();
                //Verify Extension

                if (ext!="image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/png" &&
                    ext != "image/x-png")
                {
                    using (Db db = new Db())
                    {
                       
                            model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                            ModelState.AddModelError("", "That image was not uploaded - Wrong Image extension.");

                            return View(model);
                    
                    }
                }

                //Initialize Image Name
                string imageName = file.FileName;
                //Save image name to DTO
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }
                //Set original and thumb image paths
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);
                //Save original
                file.SaveAs(path);
                //Create and save Thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);


            }


            #endregion

            //Redirect

            return RedirectToAction("AddProduct");
        }
        // GET: Admin/Shop/Products/
        public ActionResult Products(int? page, int? catId)
        {
            //Declare a list of ProductVM
            List<ProductVM> listOfProduvtVM;

            //Set page number
            var pageNumber = page ?? 1; 
            using (Db db = new Db())
            {

                //init list
                listOfProduvtVM = db.Products.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new ProductVM(x))
                    .ToList();


                //Populate categoty Select list 
                 
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                //set Selected categories 

                ViewBag.SelectedCat = catId.ToString();
            }

            //set pagination
            var onePageOfProducts = listOfProduvtVM.ToPagedList(pageNumber, 3);
            ViewBag.OnePageOfProducts = onePageOfProducts; 

            // return view with list
            return View(listOfProduvtVM);
        }
        // GET: Admin/Shop/EditProduct/Id
        [HttpGet]
        public ActionResult EditProduct (int id)
        {
            //Declare ProductVM
            ProductVM model;

            using (Db db = new Db())
            {
                //Get the product
                ProductDTO dto = db.Products.Find(id);

                //Make sure product exists
                if (dto == null)
                {
                    return Content("The product Does Not Exist.");
                }
                //Init Model

                model = new ProductVM(dto);

                //Make a Select List

                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                //Get all gallery images
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                                .Select(fn => Path.GetFileName(fn));


            }
            //Return view with model
            return View(model);
        }
        // Post: Admin/Shop/EditProduct/Id
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            //Get the product id
            int id = model.Id;

            //Populate categories select list and gallery images
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                               .Select(fn => Path.GetFileName(fn));

            //Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Make sure product name is unique
            using (Db db = new Db())
            {
                if (db.Products.Where(x => x.Id != id ).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That Product Name is Taken!");
                    return View(model);
                }
            }
            //update product
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();

            }
            //Set temp data
            TempData["SuccessMessage"] = "The Product Has Been Edited Successfully";

            #region Image Upload
            //Check for File Upload
            if (file != null && file.ContentLength > 0)
            {
                //Get extension
                string ext = file.ContentType.ToLower();
                //Verify Extension
                if (ext != "image/jpg" &&
                  ext != "image/jpeg" &&
                  ext != "image/pjpeg" &&
                  ext != "image/gif" &&
                  ext != "image/png" &&
                  ext != "image/x-png")
                {
                    using (Db db = new Db())
                    {

                        ModelState.AddModelError("", "That image was not uploaded - Wrong Image extension.");

                        return View(model);

                    }
                }
                //Set Upload Directory path
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
                
                var pathString1 = Path.Combine(originalDirectory.ToString(), "products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "products\\" + id.ToString() + "\\Thumbs");

                //Delete File from Directories
                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in di1.GetFiles())
                    file2.Delete();
                foreach (FileInfo file3 in di2.GetFiles())
                    file3.Delete();


                //Save Image Name
                string imageName = file.FileName;
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }
                //Save Original and Thumb images path
                var path = string.Format("{0}\\{1}", pathString1, imageName);
                var path2 = string.Format("{0}\\{1}", pathString2, imageName);
           
                file.SaveAs(path);
             
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2); 
            }
            
            #endregion

            //redirect
            return RedirectToAction("EditProduct");
        }
        // GET: Admin/Shop/DeleteProduct/Id
        public ActionResult DeleteProduct(int id)
        {
            //delete product from the db
            using (Db db = new Db ())
            {
                ProductDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);

                db.SaveChanges();

            }
            //delete product folder
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            string pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);
            //redirect
            return RedirectToAction("Products");
        }
        //Post: /Admin/Shop/SaveGalleryImages
        [HttpPost] 
        public void SaveGalleryImages(int id)
        {
            //Loop through the file
            foreach (string  fileName in Request.Files)
            {
                //Init the Files
                HttpPostedFileBase file = Request.Files[fileName];

                //check if its not null
                if (file != null && file.ContentLength > 0)
                {
                    //Set directory paths
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    //set image paths
                    var path = string.Format("{0}\\{1}", pathString1, file.FileName);
                    var path2 = string.Format("{0}\\{1}", pathString2, file.FileName);
                    //Save original and thumb
                    file.SaveAs(path);
                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200);
                    img.Save(path2); 
                }
            }
            
        }
        //Post: /Admin/Shop/DeleteImage
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);
            
            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);
            


        }

    }
} 