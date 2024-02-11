using stajdeneme.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Web.UI.WebControls;
using System.Net;
using System.Collections.Generic;
using System;

namespace stajdeneme.Controllers
{
    public class ilanController : Controller
    {
        stajdenemeEntities3 db = new stajdenemeEntities3();
        // GET: Kategori
        [_SessionControl]
        public ActionResult Index()
        {
            var aranacak = from d in db.tbl_ilan select d;
            return View(aranacak.ToList());
         
        }
        public ActionResult ilan()
        {

            var aranacak = from d in db.tbl_ilan select d;
            return View(aranacak.ToList());
        }

        [_SessionControl]
        [HttpGet]
        public ActionResult ilanEkle()
        {
            List<SelectListItem> ilan = (from u in db.tbl_tip.ToList()
                                         select new SelectListItem
                                         {
                                             Text = u.tip_name,
                                             Value = u.tip_id.ToString()


                                         }).ToList();
            ViewBag.v = ilan;

            return View();
        }
        [HttpPost]
        public ActionResult ilanEkle(tbl_ilan p)
        {
            if (!ModelState.IsValid) { return View("ilanEkle"); }
            List<SelectListItem> ilan = (from u in db.tbl_tip.ToList()
                                         select new SelectListItem
                                         {
                                             Text = u.tip_name,
                                             Value = u.tip_id.ToString()


                                         }).ToList();
            ViewBag.v = ilan;

            var bol = db.tbl_tip.Where(m => m.tip_id == p.tbl_tip.tip_id).FirstOrDefault();

            p.tbl_tip = bol;
            p.ilan_tarih = DateTime.Now;
            db.tbl_ilan.Add(p);
            db.SaveChanges();
            return RedirectToAction("index");
        }
        
        [HttpPost]
        public ActionResult RGuncelle(tbl_ilan id)
        {
            var getir = db.tbl_ilan.Find(id.ilan_id);

            getir.ilan_aciklama = id.ilan_aciklama;
            getir.ilan_baslik = id.ilan_baslik;
            getir.ilan_tarih = id.ilan_tarih;
            getir.ilan_beceriler = id.ilan_beceriler;
           
            getir.ilan_durum = id.ilan_durum;

            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [_SessionControl]
        [HttpGet]
        public ActionResult Guncelle(int id)
        {
            var getir = db.tbl_ilan.Find(id);
            //List<SelectListItem> ilan = (from u in db.tbl_tip.ToList()
            //                             select new SelectListItem
            //                             {
            //                                 Text = u.tip_name,
            //                                 Value = u.tip_id.ToString()


            //                             }).ToList();
            //ViewBag.v = ilan;
            return View("Guncelle",getir);
        }
        
        public ActionResult Sil(int id)
        {
            var kategori = db.tbl_ilan.Find(id);
            db.tbl_ilan.Remove(kategori);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}