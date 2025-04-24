using RestoranUygulama.Models;
using System.Collections.Generic;

namespace RestoranUygulama.ViewModel
{
    public class MasaDetayViewModel
    {
        public Masa Masa { get; set; }
        public List<Rezervasyon> BugunRezervasyonlar { get; set; }
        public int? AktifRezervasyonId { get; set; }
    }
}
