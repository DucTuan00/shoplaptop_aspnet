using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;
using PagedList;

namespace Ictshop.Controllers
{
    public class SanphamController : Controller
    {
        Qlbanhang db = new Qlbanhang();

        // GET: Sanpham
        public ActionResult dtiphonepartial()
        {
            var ip = db.Sanphams.Where(n=>n.Mahang==8).Take(8).ToList();       
            return PartialView(ip);
        }
        public ActionResult dtsamsungpartial()
        {
            var ss = db.Sanphams.Where(n => n.Mahang == 7).Take(8).ToList();
            return PartialView(ss);
        }
        public ActionResult dtxiaomipartial()
        {
            var mi = db.Sanphams.Where(n => n.Mahang == 1).Take(8).ToList();
            return PartialView(mi);
        }      
        public ActionResult xemchitiet(int Masp=0)
        {
            var chitiet = db.Sanphams.SingleOrDefault(n=>n.Masp==Masp);
            if (chitiet == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(chitiet);
        }

        public ActionResult SearchText(string searchText = "")
        {
            if (searchText != "" && searchText != null)
            {
                var dt = db.Sanphams.Where(p => p.Tensp.Contains(searchText)).OrderBy(p => p.Masp);
                return View("./timkiem", dt.ToPagedList(1, 5));
            }
            else
            {
                var sp = db.Sanphams.OrderBy(x => x.Masp);
                return View("./timkiem", sp.ToPagedList(1, 5));
            }
        }

    }

}