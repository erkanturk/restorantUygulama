using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using RestoranUygulama.Models;
using Microsoft.AspNetCore.Http;

public class RezervasyonController : Controller
{
    private readonly RestoranContext _context;

    public RezervasyonController(RestoranContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Detay(int masaId)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciAdi")))
        {
            return RedirectToAction("Login", "Account");
        }

        var masa = _context.Masalar.Find(masaId);
        if (masa == null)
        {
            return NotFound();
        }

        var rezervasyonlar = _context.Rezervasyonlar
            .Where(r => r.MasaId == masaId && r.RezervasyonTarihi >= DateTime.Today)
            .ToList();

        ViewBag.MasaNumarasi = masa.MasaNumarasi;
        ViewBag.MasaId = masa.Id;
        return View(rezervasyonlar);
    }

    [HttpGet]
    public IActionResult Ekle(int masaId)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciAdi")))
        {
            return RedirectToAction("Login", "Account");
        }

        var masa = _context.Masalar.Find(masaId);
        if (masa == null)
        {
            return NotFound();
        }

        ViewBag.MasaId = masaId;
        ViewBag.MasaNumarasi = masa.MasaNumarasi;
        return View();
    }

    [HttpPost]
    public IActionResult Ekle(int masaId, string musteriAdi, DateTime rezervasyonTarihi)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciAdi")))
        {
            return RedirectToAction("Login", "Account");
        }

        var masa = _context.Masalar.Find(masaId);
        if (masa == null)
        {
            return NotFound();
        }

        var mevcutRezervasyon = _context.Rezervasyonlar
            .FirstOrDefault(r => r.MasaId == masaId && r.RezervasyonTarihi.Date == rezervasyonTarihi.Date);

        if (mevcutRezervasyon != null)
        {
            ViewBag.ErrorMessage = "Bu masada seçilen tarihte zaten bir rezervasyon var.";
            ViewBag.MasaId = masaId;
            ViewBag.MasaNumarasi = masa.MasaNumarasi;
            return View();
        }

        var rezervasyon = new Rezervasyon
        {
            MasaId = masaId,
            MusteriAdi = musteriAdi,
            RezervasyonTarihi = rezervasyonTarihi
        };

        _context.Rezervasyonlar.Add(rezervasyon);
        _context.SaveChanges();

        return RedirectToAction("Detay", new { masaId = masaId });
    }

    [HttpGet]
    public IActionResult Duzenle(int id)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciAdi")))
        {
            return RedirectToAction("Login", "Account");
        }

        var rezervasyon = _context.Rezervasyonlar.Find(id);
        if (rezervasyon == null)
        {
            return NotFound();
        }

        var masa = _context.Masalar.Find(rezervasyon.MasaId);
        ViewBag.MasaNumarasi = masa.MasaNumarasi;
        ViewBag.MasaId = masa.Id;
        return View(rezervasyon);
    }

    [HttpPost]
    public IActionResult Duzenle(int id, string musteriAdi, DateTime rezervasyonTarihi)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciAdi")))
        {
            return RedirectToAction("Login", "Account");
        }

        var rezervasyon = _context.Rezervasyonlar.Find(id);
        if (rezervasyon == null)
        {
            return NotFound();
        }

        var masa = _context.Masalar.Find(rezervasyon.MasaId);
        var mevcutRezervasyon = _context.Rezervasyonlar
            .FirstOrDefault(r => r.MasaId == rezervasyon.MasaId && r.RezervasyonTarihi.Date == rezervasyonTarihi.Date && r.Id != id);

        if (mevcutRezervasyon != null)
        {
            ViewBag.ErrorMessage = "Bu masada seçilen tarihte zaten başka bir rezervasyon var.";
            ViewBag.MasaNumarasi = masa.MasaNumarasi;
            ViewBag.MasaId = masa.Id;
            return View(rezervasyon);
        }

        rezervasyon.MusteriAdi = musteriAdi;
        rezervasyon.RezervasyonTarihi = rezervasyonTarihi;
        _context.SaveChanges();

        return RedirectToAction("Detay", new { masaId = rezervasyon.MasaId });
    }

    [HttpGet]
    public IActionResult Sil(int id)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciAdi")))
        {
            return RedirectToAction("Login", "Account");
        }

        var rezervasyon = _context.Rezervasyonlar.Find(id);
        if (rezervasyon == null)
        {
            return NotFound();
        }

        var masa = _context.Masalar.Find(rezervasyon.MasaId);
        ViewBag.MasaNumarasi = masa.MasaNumarasi;
        ViewBag.MasaId = masa.Id;
        return View(rezervasyon);
    }

    [HttpPost, ActionName("Sil")]
    public IActionResult SilOnay(int id)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciAdi")))
        {
            return RedirectToAction("Login", "Account");
        }

        var rezervasyon = _context.Rezervasyonlar.Find(id);
        if (rezervasyon == null)
        {
            return NotFound();
        }

        var masaId = rezervasyon.MasaId;
        _context.Rezervasyonlar.Remove(rezervasyon);
        _context.SaveChanges();

        return RedirectToAction("Detay", new { masaId = masaId });
    }
}
