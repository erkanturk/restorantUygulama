using Microsoft.AspNetCore.Mvc;
using System.Linq;
using RestoranUygulama.Models;
using Microsoft.AspNetCore.Http;
using RestoranUygulama.ViewModel;

public class GarsonController : Controller
{
    private readonly RestoranContext _context;

    public GarsonController(RestoranContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciAdi")))
        {
            return RedirectToAction("Login", "Account");
        }

        var masalar = _context.Masalar.ToList();
        return View(masalar);
    }

    [HttpPost]
    public IActionResult DurumDegistir(int masaId)
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

        masa.DoluMu = !masa.DoluMu; // Durumu tersine çevir (doluysa boş, boşsa dolu)
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult RezervasyonAktifEt(int rezervasyonId)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciAdi")))
        {
            return RedirectToAction("Login", "Account");
        }

        var rezervasyon = _context.Rezervasyonlar.Find(rezervasyonId);
        if (rezervasyon == null)
        {
            return NotFound();
        }

        var masa = _context.Masalar.Find(rezervasyon.MasaId);
        if (masa == null)
        {
            return NotFound();
        }

        // Masayı dolu olarak işaretle
        masa.DoluMu = true;

        // Aktif rezervasyon bilgisini Session'da sakla
        HttpContext.Session.SetInt32("AktifRezervasyonId_" + masa.Id, rezervasyonId);

        _context.SaveChanges();

        return RedirectToAction("MasaDetay", new { masaId = masa.Id });
    }
    public IActionResult MasaDetay(int masaId)
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

        // Bugünkü rezervasyonları getir
        var bugunRezervasyonlar = _context.Rezervasyonlar
            .Where(r => r.MasaId == masaId && r.RezervasyonTarihi.Date == DateTime.Today)
            .ToList();

        // Aktif rezervasyon var mı kontrol et
        int? aktifRezervasyonId = HttpContext.Session.GetInt32("AktifRezervasyonId_" + masaId);

        var viewModel = new MasaDetayViewModel
        {
            Masa = masa,
            BugunRezervasyonlar = bugunRezervasyonlar,
            AktifRezervasyonId = aktifRezervasyonId
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult MasayiBosalt(int masaId)
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

        // Masayı boş olarak işaretle
        masa.DoluMu = false;

        // Aktif rezervasyon bilgisini Session'dan kaldır
        HttpContext.Session.Remove("AktifRezervasyonId_" + masaId);

        _context.SaveChanges();

        return RedirectToAction("MasaDetay", new { masaId = masaId });
    }



}
