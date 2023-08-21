using ANK13SuperMarket.Context;
using ANK13SuperMarket.Entities;
using ANK13SuperMarket.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace ANK13SuperMarket.Controllers
{
    public class MarketController : Controller
    {
        private readonly MarketDbContext _db;

        private readonly Product _product = new Product();

        public MarketController(MarketDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            ViewBag.StockDurumu = false;
            return View(_db.Products.Where(u => u.IsInStock).ToList());
        }

        public IActionResult StoktaOlmayanlar()
        {
            return View();
        }

        public IActionResult StoktaOlmayanlariGetir()
        {
            ViewBag.StockDurumu = true;
            return View("Index", _db.Products.Where(u => !u.IsInStock).ToList());
        }

        [HttpGet]
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Ekle(ProductViewModel productViewModel)
        {
            if (_db.Products.Any(p => p.Name == productViewModel.Name))
            {
                ModelState.AddModelError("Name", "Bu ürün ismi zaten mevcut.");
                return View(productViewModel);
            }

            if (productViewModel.Price <= 0)
            {
                ModelState.AddModelError("Price", "Ürün fiyatı sıfır veya negatif olamaz.");
                return View(productViewModel);
            }

            _product.Name = productViewModel.Name;
            _product.Price = productViewModel.Price;
            _product.IsInStock = productViewModel.IsInStock;

            if (productViewModel.Image != null)
            {
                var dosyaAdi = productViewModel.Image.FileName;

                _product.ImageName = dosyaAdi;

                var konum = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", dosyaAdi);

                using (var akisOrtami = new FileStream(konum, FileMode.Create))
                {
                    productViewModel.Image.CopyTo(akisOrtami);
                }
            }
            else
            {
                _product.ImageName = "";
            }

            _db.Products.Add(_product);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Ürün başarıyla eklendi.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Guncelle(int id)
        {
            ProductViewModel productViewModel = new ProductViewModel();
            var guncellenecekUrun = _db.Products.Find(id);

            productViewModel.Name = guncellenecekUrun.Name;
            productViewModel.Price = guncellenecekUrun.Price;
            productViewModel.IsInStock = guncellenecekUrun.IsInStock;

            TempData["id"] = guncellenecekUrun.Id;

            return View(productViewModel);
        }

        [HttpPost]
        public IActionResult Guncelle(ProductViewModel productViewModel)
        {
            var existingProduct = _db.Products.Find(TempData["id"]);

            if (_db.Products.Any(p => p.Name == productViewModel.Name && p.Id != existingProduct.Id))
            {
                ModelState.AddModelError("Name", "Bu ürün ismi zaten mevcut.");
                return View(productViewModel);
            }

            if (productViewModel.Price <= 0)
            {
                ModelState.AddModelError("Price", "Ürün fiyatı sıfır veya negatif olamaz.");
                return View(productViewModel);
            }

            existingProduct.Name = productViewModel.Name;
            existingProduct.Price = productViewModel.Price;
            existingProduct.IsInStock = productViewModel.IsInStock;
            existingProduct.UpdatedDate = DateTime.Now;

            if (productViewModel.Image != null)
            {
                var dosyaAdi = productViewModel.Image.FileName;

                existingProduct.ImageName = dosyaAdi;

                var konum = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", dosyaAdi);

                using (var akisOrtami = new FileStream(konum, FileMode.Create))
                {
                    productViewModel.Image.CopyTo(akisOrtami);
                }
            }
            else
            {
                var digerUrunler = _db.Products.Except(_db.Products.Where(x => x.Id == existingProduct.Id));
                existingProduct.ImageName = null;

                if (digerUrunler.All(u => u.ImageName != existingProduct.ImageName))
                {
                    var resimAdresleri = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images"));

                    foreach (var adres in resimAdresleri)
                    {
                        var resimAdi = adres.Substring(adres.LastIndexOf("\\")).Replace("\\", "");

                        if (resimAdi == existingProduct.ImageName)
                        {
                            System.IO.File.Delete(adres);
                            break;
                        }
                    }
                }
            }

            _db.Products.Update(existingProduct);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";

            return RedirectToAction("Index");
        }

        public IActionResult Sil(int id)
        {
            return View(_db.Products.Find(id));
        }

        [HttpPost]
        public IActionResult Sil(Product urun)
        {
            var digerUrunler = _db.Products.Except(_db.Products.Where(x => x.Id == urun.Id));

            var ortakKullanim = digerUrunler.All(u => u.ImageName != urun.ImageName);

            if (ortakKullanim)
            {
                var resimAdresleri = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images"));

                foreach (var adres in resimAdresleri)
                {
                    var resimAdi = adres.Substring(adres.LastIndexOf("\\")).Replace("\\", "");

                    if (resimAdi == urun.ImageName)
                    {
                        System.IO.File.Delete(adres);
                        break;
                    }
                }
            }

            _db.Products.Remove(urun);
            _db.SaveChanges();
            TempData["SuccessMessage"] = "Ürün başarıyla silindi.";

            return RedirectToAction("Index");
        }
    }
}
