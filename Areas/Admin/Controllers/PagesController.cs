using CmsWebApp.Models.Data;
using CmsWebApp.Models.ViewModels.Pages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsWebApp.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // list of PageVM
            List<PageVM> pageList;
            // Init the list
            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x=>x.Sorting).Select(x=> new PageVM(x)).ToList();
            }


            return View(pageList);
        }

        //GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Check model stats
            if (! ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db =new Db())
            {
                //Declare slug
                string slug;

                //Init pageDTO
                PageDTO dto = new PageDTO();

                //DTO title
                dto.Title=model.Title;

                //Check for and set slug if need be
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //Make sure title and slug are unique
                if (db.Pages.Any(x=>x.Title==model.Title) || db.Pages.Any(x=>x.Slug==slug))
                {
                    ModelState.AddModelError("","That title or slug already exist");
                    return View(model);
                }


                //DTO the reset
                dto.Slug=slug;
                dto.Body=model.Body;
                dto.HasSidebar=model.HasSidebar;
                dto.Sorting=100;

                //Save DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            }
            //Set TempData message
            TempData["SM"]="You have added a new page!";

            //Redirect
            return RedirectToAction("AddPage");
        }

    }
}