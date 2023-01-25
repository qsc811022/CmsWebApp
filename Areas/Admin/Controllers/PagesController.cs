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
    }
}