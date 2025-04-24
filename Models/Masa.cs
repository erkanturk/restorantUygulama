namespace RestoranUygulama.Models
{
    public class Masa
    {
        public int Id { get; set; }
        public int MasaNumarasi { get; set; }
        public bool DoluMu { get; set; } // Masa şu an rezerve edilmiş mi?
        public virtual List<Rezervasyon> Rezervasyonlar { get; set; } // Masaya ait rezervasyonlar
    }

}
