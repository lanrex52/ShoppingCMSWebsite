﻿using ShoppingCMSWebsite.Models.Data;
using ShoppingCMSWebsite.Models.ViewModel;
using ShoppingCMSWebsite.Models.ViewModel.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCMSWebsite.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            //Get/Set page slug
            if (page == "")
                page = "home";
            
            //Declare model and dto
            PageVM model;
            PageDTO dto;

            //Check if page exists
            using (Db db = new Db ())
            {
                if (! db.Pages.Any(x=> x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }

            // Get pageDto
            using (Db db = new Db ())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            //Set page title
            ViewBag.PageTitle = dto.Title;

            //Check for sidebar
            if (dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            //Init model
            model = new PageVM(dto);

            //Return view with model

            return View(model);
        }

        //Pages partial view
        public ActionResult PagesMenuPartial()
        {
            //declare list of PageVm
            List<PageVM> pageVMList;
            //Get all pages except homepage
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x => new PageVM(x)).ToList();

            }
            //Return partial view with list
            return PartialView(pageVMList);
        }
        public ActionResult SidebarPartial()
        {
            //Declare the model
            SidebarVM model;
            //init the model
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebar.Find(1);
                
                model = new SidebarVM(dto);
            }
            //Return Partial view with model
            return PartialView(model);
        }
    }
 }