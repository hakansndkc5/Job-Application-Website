
using Newtonsoft.Json;
using PagedList;
using stajdeneme.Controllers;
using stajdeneme.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace basvurudeneme.Controllers
{

    public class BasvuruController : Controller
    {

        // GET: Basvuru
        stajdenemeEntities3 db = new stajdenemeEntities3();


        //public ActionResult Index(int sayfa = 1)
        //{
        //    var degerler = db.tbl_basvuru.ToList().ToPagedList(sayfa, 2);
        //    return View(degerler);
        //}
        [_SessionControl]
        public ActionResult Index(string ara, int sayfa = 1)
        {



            var aranacak = from d in db.tbl_basvuru select d;
            var aranacak1 = from d in db.tbl_basvuru select d;


            if (!string.IsNullOrEmpty(ara))
            {

                aranacak = aranacak.Where(m => m.basvuru_ad.Contains(ara) || m.basvuru_soyad.Contains(ara));

            }
            
            

            return View(aranacak.ToList().ToPagedList(sayfa, 50));
        }

        [HttpGet]
        public ActionResult Insert(string basvuru_ilanid, int tip = 0) { 
            
            List<SelectListItem> bos = new List<SelectListItem>();
            if (basvuru_ilanid == null || tip == 0)
            {
                return RedirectToAction("ilan", "ilan");

            }
                            
            ViewBag.bolum = bos;
            ViewBag.il = bos;
            ViewBag.ilce = bos;
            ViewBag.fak = bos;
            ViewBag.tt = tip;
            ViewBag.uni = bos;
            string result = basvuru_ilanid;
            ViewBag.ilanid = result;
           
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Insert(tbl_basvuru p, HttpPostedFileBase file, HttpPostedFileBase file2,int tip = 0)
        {

           
            if (!ModelState.IsValid) {
                List<SelectListItem> bos = new List<SelectListItem>();
                ViewBag.uni = bos;
                ViewBag.fak = bos;
                ViewBag.bolum = bos;
              
                ViewBag.il = bos;
                ViewBag.ilce = bos;
                ViewBag.m = false;
                return View("Insert"); }



            if (ModelState.IsValid)
            {
                var response = Request["g-recaptcha-response"];
                const string secret = "6LeOE8AbAAAAAAT-hOMFm0UzTrZIMfHEzpgHKV4Z";
                //Kendi Secret keyinizle değiştirin.

                var client = new WebClient();
                var reply =
                    client.DownloadString(
                        string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

                var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

                if (!captchaResponse.Success) {
                    TempData["Message"] = "Lütfen güvenliği doğrulayınız.";
                    return RedirectToAction("Insert");
                }
                else
                    TempData["Message"] = "Güvenlik başarıyla doğrulanmıştır.";


                List<SelectListItem> university = (from u in db.tbl_universite.ToList()
                                                   select new SelectListItem
                                                   {
                                                       Text = u.name,
                                                       Value = u.universite_id.ToString()


                                                   }).ToList();



                List<SelectListItem> fakulte = (from u in db.tbl_fakulte.ToList()
                                                select new SelectListItem
                                                {
                                                    Text = u.name,
                                                    Value = u.fakulte_id.ToString()


                                                }).ToList();



                List<SelectListItem> bolum = (from u in db.tbl_bolum.ToList()
                                              select new SelectListItem
                                              {
                                                  Text = u.name,
                                                  Value = u.bolum_id.ToString()


                                              }).ToList();
                List<SelectListItem> il = (from u in db.iller.ToList()
                                              select new SelectListItem
                                              {
                                                  Text = u.sehir,
                                                  Value = u.id.ToString()


                                              }).ToList();
                List<SelectListItem> ilce = (from u in db.ilceler.ToList()
                                              select new SelectListItem
                                              {
                                                  Text = u.ilce,
                                                  Value = u.id.ToString()


                                              }).ToList();
                //List<SelectListItem> ilan = (from u in db.tbl_ilan.ToList()
                //                                   select new SelectListItem
                //                                   {
                //                                       Text = u.ilan_baslik,
                //                                       Value = u.ilan_id.ToString()


                //                                   }).ToList();

                ViewBag.uni = university;
                ViewBag.fak = fakulte;
                ViewBag.bolum = bolum;
                ViewBag.il = il;
                ViewBag.ilce = ilce;
                ViewBag.tt = tip;


                var ins = db.tbl_universite.Where(m => m.universite_id == p.tbl_universite.universite_id).FirstOrDefault();
                var fak = db.tbl_fakulte.Where(m => m.fakulte_id == p.tbl_fakulte.fakulte_id).FirstOrDefault();
                var bol = db.tbl_bolum.Where(m => m.bolum_id == p.tbl_bolum.bolum_id).FirstOrDefault();
                var il1 = db.iller.Where(m => m.id == p.iller.id).FirstOrDefault();
                var ilce1= db.ilceler.Where(m => m.id == p.ilceler.id).FirstOrDefault();

                p.tbl_bolum = bol;
                p.tbl_fakulte = fak;
                p.tbl_universite = ins;
                p.iller = il1;
                p.ilceler = ilce1;

                //p.basvuru_tip = tip;
                p.basvuru_basvurutarihi = DateTime.Now;

               
                p.basvuru_foto = "a.jpg";
                if (file != null)
                {
                    if (file.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 1; //3 MB
                        string[] AllowedFileExtensions = new string[] { ".jpg", ".jpeg" ,".gif", ".png", ".pdf" };

                        if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                        {
                            ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", AllowedFileExtensions));
                            return View();
                        }

                        else if (file.ContentLength > MaxContentLength)
                        {
                            ModelState.AddModelError("File", "Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
                            return View();
                        }
                        else 
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/Uploads/Image"), fileName);
                            file.SaveAs(path);
                            p.basvuru_foto = fileName;
                            //var fileName2 = Path.GetFileName(file2.FileName);
                            //var path2 = Path.Combine(Server.MapPath("~/Uploads/Cv"), fileName2);
                            //file.SaveAs(path2);
                            //p.basvuru_cv = fileName2;


                            ViewBag.Message = "File uploaded successfully";


                        }
                        
                        
                    }
                }
                if (file2 != null)
                {
                    if (file2.ContentLength > 0)
                    {
                        int MaxContentLength2 = 1024 * 1024 * 2; //2 MB
                        string[] AllowedFileExtensions2 = new string[] { ".jpg", ".gif", ".png", ".pdf" };

                        if (!AllowedFileExtensions2.Contains(file2.FileName.Substring(file2.FileName.LastIndexOf('.'))))
                        {
                            ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", AllowedFileExtensions2));
                            return View();
                        }

                        else if (file2.ContentLength > MaxContentLength2)
                        {
                            ModelState.AddModelError("File", "Your file is too large, maximum allowed size is: " + MaxContentLength2 + " MB");
                            return View();
                        }
                        else
                        {
                            //var fileName = Path.GetFileName(file.FileName);
                            //var path = Path.Combine(Server.MapPath("~/Uploads/Image"), fileName);
                            //file.SaveAs(path);
                            //p.basvuru_foto = fileName;
                            var fileName2 = Path.GetFileName(file2.FileName);
                            var path2 = Path.Combine(Server.MapPath("~/Uploads/Cv"), fileName2);
                            file2.SaveAs(path2);
                            p.basvuru_cv = fileName2;


                            ViewBag.Message = "File uploaded successfully";


                        }
                    }
                }
                db.tbl_basvuru.Add(p);
                db.SaveChanges();
          
            bool sonuc = false;

            WebMail.SmtpServer = "smtp.gmail.com";
            WebMail.SmtpPort = 587;
            WebMail.UserName = "basvuruyesilmavi@gmail.com";
            WebMail.Password = "basvuruyesilmavi55";


            WebMail.EnableSsl = true;
            string dosya = Server.MapPath("~/Uploads/Cv/" + p.basvuru_cv);
            string mesaj = p.basvuru_ad + " " + p.basvuru_soyad + " tarafından " + "Yeni başvuru alınmıştır.<br> Üniversite:  " + p.tbl_universite.name + "<br> Bölüm:  " + p.tbl_bolum.name + " <br> Kişinin CV'si ektedir. <br>  <br> Kişinin Becerileri: <br> İngilizce:  " + p.basvuru_yabancıdil + ",<br> Asp.net: " + p.basvuru_becerileraspnet + ", <br>Json :" + p.basvuru_becerilerjson + ",<br> JavaScript: " + p.basvuru_becerilerjs + " ,<br> Xml: " + p.basvuru_becerilerxml + ",<br> HTML: " + p.basvuru_becerilerhtml + ",<br> SQL: " + p.basvuru_becerilersql + ",<br> MVC: " + p.basvuru_becerilermvc + ",<br> JAVA: " + p.basvuru_becerilerjava + ", <br> CSS:  " + p.basvuru_becerilercss + ",<br> C#: " + p.basvuru_becerilerc_ + ", <br>JQuery: " + p.basvuru_becerilerjquery; ;
            try
            {
                if (p.basvuru_cv == null)
                {
                    WebMail.Send(
                to: "redkit.55@hotmail.com", subject: "Yeni basvuru Başvurusu ( " + p.basvuru_ad + " " + p.basvuru_soyad + " )",
                body: mesaj,
                replyTo: "basvuruyesilmavi@gmail.com", isBodyHtml: true);
                }
                WebMail.Send(
                    to: "redkit.55@hotmail.com", subject: "Yeni basvuru Başvurusu ( " + p.basvuru_ad + " " + p.basvuru_soyad + " )",
                    body: mesaj,
                    replyTo: "basvuruyesilmavi@gmail.com", isBodyHtml: true,
                    filesToAttach: new[] { dosya }

                    );

                sonuc = true;
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            }
            //ViewBag.Sonuc = sonuc;
            ViewBag.tt = tip;
            ViewBag.m = true;
            return View();


        }




        [HttpPost]
        public JsonResult UFB(int? universiteID, int? FakulteID, int? ilID, string tip)
        {
            //EntityFramework ile veritabanı modelimizi oluşturduk ve
            //IlilceDBEntities ile db nesnesi oluşturduk.
            //IlilceDBEntities db = new IlilceDBEntities();
            //geriye döndüreceğim sonucListim
            List<SelectListItem> sonuc = new List<SelectListItem>();

            //bu işlem başarılı bir şekilde gerçekleşti mi onun kontrolunnü yapıyorum
            bool basariliMi = true;
            try
            {
                switch (tip)
                {
                    case "UniversiteGetir":
                        //veritabanımızdaki iller tablomuzdan illerimizi sonuc değişkenimze atıyoruz
                        foreach (var universite in db.tbl_universite.ToList())
                        {
                            sonuc.Add(new SelectListItem
                            {
                                Text = universite.name,
                                Value = universite.universite_id.ToString()
                            });
                        }

                        break;
                    case "FakulteGetir":
                        //ilcelerimizi getireceğiz ilimizi selecten seçilen ilID sine göre 
                        foreach (var fakulte in db.tbl_fakulte.Where(universite => universite.universite_id == universiteID).ToList())
                        {
                            sonuc.Add(new SelectListItem
                            {
                                Text = fakulte.name,
                                Value = fakulte.fakulte_id.ToString()
                            });
                        }
                        break;
                    case "bolumGetir":
                        //ilcelerimizi getireceğiz ilimizi selecten seçilen ilID sine göre 
                        foreach (var bolum in db.tbl_bolum.Where(universite => universite.fakulte_id == FakulteID).ToList())
                        {
                            sonuc.Add(new SelectListItem
                            {
                                Text = bolum.name,
                                Value = bolum.bolum_id.ToString()
                            });
                        }
                        break;
                    case "ilGetir":
                        //veritabanımızdaki iller tablomuzdan illerimizi sonuc değişkenimze atıyoruz
                        foreach (var il in db.iller.ToList())
                        {
                            sonuc.Add(new SelectListItem
                            {
                                Text = il.sehir,
                                Value = il.id.ToString()
                            });
                        }

                        break;
                    case "ilceGetir":
                        //ilcelerimizi getireceğiz ilimizi selecten seçilen ilID sine göre 
                        foreach (var ilce in db.ilceler.Where(il => il.sehir == ilID).ToList())
                        {
                            sonuc.Add(new SelectListItem
                            {
                                Text = ilce.ilce,
                                Value = ilce.id.ToString()
                            });
                        }
                        break;

                    default:
                        break;



                }
            }
            catch (Exception)
            {
                //hata ile karşılaşırsak buraya düşüyor
                basariliMi = false;
                sonuc = new List<SelectListItem>();
                sonuc.Add(new SelectListItem
                {
                    Text = "Bir hata oluştu :(",
                    Value = "Default"
                });

            }
            //Oluşturduğum sonucları json olarak geriye gönderiyorum
            return Json(new { ok = basariliMi, text = sonuc });
        }

        [HttpPost]
        public ActionResult EmployeeDetail(int? id , string foto)
        {
            ViewBag.f = foto;
            //gelen id yi Veritabanımızda kontrol ediyoruz ve gelen employee partialviewimize gönderiyoruz
            return PartialView(db.tbl_basvuru.FirstOrDefault(x => x.basvuru_id == id));
        }

        public FileResult DownloadFile(string fileName)
        {
            //Build the File Path.
            string path = Server.MapPath("~/Uploads/Cv/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }

        //[HttpPost]
        //public ActionResult UpdateCustomer(tbl_basvuru _customer)
        //{
        //    using (stajdenemeEntities3 entities = new stajdenemeEntities3())
        //    {
        //        tbl_basvuru updatedCustomer = (from c in entities.tbl_basvuru
        //                                       where c.basvuru_id == _customer.basvuru_id
        //                                       select c).FirstOrDefault();

        //        if (updatedCustomer != null)
        //        {

        //            updatedCustomer.basvuru_okundu = _customer.basvuru_okundu;

        //            entities.SaveChanges();
        //            return Json(true);
        //        }
        //    }

        //    return Json(false);
        //}

        [HttpPost]
        public ActionResult RGuncelle2(tbl_basvuru id)
        {
            var getir = db.tbl_basvuru.Find(id.basvuru_id);

            getir.basvuru_ad = id.basvuru_ad;
            getir.basvuru_adminyorum = id.basvuru_adminyorum;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Guncelle2(int id)
        {
            var getir = db.tbl_basvuru.Find(id);

            return View("Guncelle2", getir);
        }

        public void GridExportToExcel()
        {
            DataTable dbBasvuru = new DataTable();
            dbBasvuru.Columns.AddRange(new DataColumn[36] {
                      new DataColumn("id"),
                      new DataColumn("Ad"),
                      new DataColumn("Soyad"),
                      new DataColumn("Uyruk"),
                      new DataColumn("Tel"),
                      new DataColumn("E-mail"),
                      new DataColumn("Okul"),
                      new DataColumn("Fakulte"),
                      new DataColumn("Bölüm"),
                      new DataColumn("Sınıf"),
                      new DataColumn("Not Ortalama"),
                      new DataColumn("Başlangıç Tarihi"),
                      new DataColumn("Bitiş Tarihi"),
                      new DataColumn("Beceri Sql"),
                      new DataColumn("Beceri C#"),
                      new DataColumn("Beceri Html"),
                      new DataColumn("Beceri Css"),
                      new DataColumn("Beceri Asp.net"),
                      new DataColumn("Beceri Mvc"),
                      new DataColumn("Beceri js"),
                      new DataColumn("Beceri jquery"),
                      new DataColumn("Beceri java"),
                      new DataColumn("Beceri json"),
                      new DataColumn("Beceri xml"),
                      new DataColumn("Hobiler"),
                      new DataColumn("Yabancı dil"),
                      new DataColumn("Saglık durumu"),
                      new DataColumn("Covid durumu"),
                      new DataColumn("Referans"),
                      new DataColumn("Github"),
                      new DataColumn("Nasıl buldunuz"),
                      new DataColumn("Bizi Neden Seçtiniz"),
                      new DataColumn("Eklemek İstediğiniz Birşey"),
                      new DataColumn("Fotograf"),
                      new DataColumn("Cv"),
                      new DataColumn("Admin Yorum"),

          });

            var employees = db.tbl_basvuru.ToList();

            foreach (var employee in employees)
            {
                dbBasvuru.Rows.Add(employee.basvuru_id.ToString(), employee.basvuru_ad, employee.basvuru_soyad,
                    employee.basvuru_uyruk, employee.basvuru_tel, employee.basvuru_email, employee.basvuru_okul,
                    employee.basvuru_fakulte, employee.basvuru_bolum, employee.basvuru_sınıf, employee.basvuru_notortalama,
                    employee.basvuru_başlangıc, employee.basvuru_bitis, employee.basvuru_becerilersql, employee.basvuru_becerilerc_,
                    employee.basvuru_becerilerhtml, employee.basvuru_becerilercss, employee.basvuru_becerileraspnet, employee.basvuru_becerilermvc,
                    employee.basvuru_becerilerjs, employee.basvuru_becerilerjquery, employee.basvuru_becerilerjava, employee.basvuru_becerilerjson,
                    employee.basvuru_becerilerxml, employee.basvuru_hobiler, employee.basvuru_yabancıdil, employee.basvuru_saglık, employee.basvuru_covid,
                    employee.basvuru_referans, employee.basvuru_github, employee.basvuru_nasılbuldunuz, employee.basvuru_bizinedensectiniz,
                    employee.basvuru_diger, employee.basvuru_foto, employee.basvuru_cv, employee.basvuru_adminyorum);
            }


            string filename = "BasvuruList";// Buraya veritabanınından gelen herhangi bir dataSource gelebilir.( DataTable, DataSet, kendi oluşturduğunuz, herhangi bir ICollection tipinde entitiy model)
            var grid = new GridView();
            grid.DataSource = dbBasvuru;
            grid.DataBind();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Response.Charset = "windows-1254";
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xls");

            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());
            Response.End();
        }
        [HttpPost]
        public ActionResult Foto(int? p) {
            //Build the File Path.
            
                //gelen id yi Veritabanımızda kontrol ediyoruz ve gelen employee partialviewimize gönderiyoruz
                return PartialView(db.tbl_basvuru.FirstOrDefault(x => x.basvuru_id == p));
            
        }

        public ActionResult Sil(int id)
        {
            var k = db.tbl_basvuru.Find(id);
            db.tbl_basvuru.Remove(k);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}