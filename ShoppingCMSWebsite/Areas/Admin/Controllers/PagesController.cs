using ShoppingCMSWebsite.Models.Data;
using ShoppingCMSWebsite.Models.ViewModel;
using ShoppingCMSWebsite.Models.ViewModel.Pages;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCMSWebsite.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // declare list of PageVM
            List<PageVM> pagelist;
            using (Db db = new Db())
            {
                 pagelist = db.Pages.ToArray().OrderBy(x=> x.Sorting).Select(x => new PageVM(x)).ToList();
               

            }
            return View(pagelist);
        }
        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        // Post: Admin/Pages/AddPage
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddPage(PageVM model)
        {

            // check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                //declare slug
                string slug;

                //initialize the pageDTO
                PageDTO dto = new PageDTO();

                //DTO Title
                dto.Title = model.Title;

                //check for and set slug if needed
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //Ensure Title and Slug are Unique
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "The Title or Slug Already Exists.");
                    return View(model);

                }

                //DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //Save the DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            // Set TempData Message 
            TempData["SuccessMessage"] = "You Have Added a New Page";

            // Redirect
            return RedirectToAction("AddPage");
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Declare pagevm
            PageVM model;
            using (Db db = new Db())
            {
                //Get the Page

                PageDTO  dto = db.Pages.Find(id);

                //Confirm Page Exists

                if (dto==null)
                {
                    return Content("The Page does not exist.");
                }

                //Init pagevm
                model = new PageVM(dto);

            }

            // return view with model

            return View(model);
        }
        // Post: Admin/Pages/EditPage/id
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditPage(PageVM model)
        {
            //Check model state 
            if (! ModelState.IsValid )
            {
                return View(model);
            }

            using (Db db = new Db())
            {


                //get page id
              int id = model.Id;

                //Init slug

                string slug = "home";

                // get the page  
              
                PageDTO  dto = db.Pages.Find(id);
                //dto the title
                dto.Title = model.Title;
                //check for slug and set if need be

                if (model.Slug != "home")
                {

                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();


                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }

                }

                //Make sure title and slug are unique

                if (db.Pages.Where(x=> x.Id !=  id).Any(x=> x.Title == model.Title) || db.Pages.Where(x => x.Id != id).Any(x => x.Slug  == slug))
                { 
                   ModelState.AddModelError("", "That title or slug already exists");
                    return View(model);
                }



                // dto the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;


                //save the dto



                 db.Entry(dto).State = EntityState.Modified;
               // db.tblPages.Add(dto);
                    db.SaveChanges();
               

            }
            // set TempData
            TempData["SuccessMessage"] = "You Have Successfully Edited the Page";

            //Redirect
              return   RedirectToAction("EditPage");
        }
        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            //Declare Pagevm
            PageVM model;
            using (Db db = new Db())
            {
                //Get the page
                PageDTO dto = db.Pages.Find(id);
                //Confirm if the page exists
                if (dto == null)
                {
                    return Content("This Page does not Exist");
                }
                
                //Init pagevm
                model = new PageVM(dto);
            }
            //return View with model
            return View(model);
        }
        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {


                //Get the page
                PageDTO dto = db.Pages.Find(id);

                //Remove the page
                db.Pages.Remove(dto);
                //save
                db.SaveChanges();
            }
            //TempData

            TempData["SucessMessage"] = "Page Deleted Successfully";
            //Redirect
            return RedirectToAction("Index");
        }

        // Post: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                // set initial count
                int count = 1;

                //declare tblPage
                PageDTO dto;
                //set sorting for each page 
                foreach (var pageID in id)
                {
                    dto = db.Pages.Find(pageID);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }


            }
        }

        // Get: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            //declare model
            SidebarVM model;
            using (Db db = new Db())
            {
                //Get the dto
                SidebarDTO dto = db.Sidebar.Find(1);
                //init model
                model = new SidebarVM(dto);
            }
            //return view with model

            return View(model);
        }
        // Post: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db() )
            {
                //Get the dto
                SidebarDTO dto = db.Sidebar.Find(1);
                //dto the body
                dto.Body = model.Body;
                //save
                db.SaveChanges();
            }
            //set temp data
            TempData["SuccessMessage"] = "You have Edited the Sidebar Successfully";

            //redirect


            return RedirectToAction("EditSidebar");
        }












        } 
}