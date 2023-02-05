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

        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Declare pageVM
            PageVM model;
            using (Db db = new Db())
            {
                //Get the page
                PageDTO dto= db.Pages.Find(id);

                if (dto==null)
                {
                    return Content("The page does not exist");
                }

                //Init pageVM
                model=new PageVM(dto);

                //model = new PageVM()
                //{
                //    Id=dto.Id;
                //};

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {



                //Get page id
                int id = model.Id;
                //Get the page
                PageDTO dto = db.Pages.Find(id);
                //init slug
                string slug="home";
                //DTO the title
                dto.Title=model.Title;

                //Check for slug and set it if need be
                if (model.Slug!="home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug=model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug=model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //Make sure title and slug are unique
                if (db.Pages.Where(x=>x.Id !=id).Any(x=>x.Title==model.Title)||
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("","That title or slug already exists");
                    return View(model);
                }

                //DTO the rest
                dto.Slug=slug;
                dto.Body=model.Body;
                dto.HasSidebar=model.HasSidebar;

                //Save the DTO
                db.SaveChanges();
            }

            //Set TempData message
            TempData["SM"]="You have edited the page!";

            //Redirect
            return RedirectToAction("EditPage");
        }
        

        // GET:Admin/Pages/PageDetails
        public ActionResult PageDetails(int id)
        {
            //Declare PageVM
            PageVM model;
            using (Db db = new Db())
            {


                //Get the page
                PageDTO dto = db.Pages.Find(id);

                //Confirm page exists
                if (dto ==null)
                {
                    return Content("The page does not exist");
                }

                //Init PageVM
                model = new PageVM(dto);
            }
            return View(model);
        }

        // GET:Admin/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                //Get the page
                PageDTO dto=db.Pages.Find(id);
                //Remove the page
                db.Pages.Remove(dto);
                //Save
                db.SaveChanges();

            }

            //Redirect
            return RedirectToAction("Index");
        }
        //post: admin/Pages/RecorderPages
        [HttpPost]
        public void ReorderPage(int[] id)
        {
            using (Db db= new Db())
            {
                //Set initial count
                int count=1;
                //Declare PageDTO
                PageDTO dto;
                //Set sorting for each page
                foreach (var pageId in id)
                {
                    dto=db.Pages.Find(pageId);
                    dto.Sorting=count;
                    db.SaveChanges();
                    count++;
                }


            }
        }
    }
}