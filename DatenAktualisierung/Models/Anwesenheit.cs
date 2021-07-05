using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DatenAktualisierung.Models
{
    public class Anwesenheit
    {
        CultureInfo culture = CultureInfo.CreateSpecificCulture("");
        public Anwesenheit()
        { }
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public Nullable<decimal> Anwesend { get; set; }

        public Nullable<decimal> Krank { get; set; }
        public Nullable<int> Mandantennummer { get; set; }
        public String Name { get; set; }
        public String Vorname { get; set; }
        public String Personalnummer { get; set; }
        public String Fehlgrund { get; set; }

        public String Abteilung { get; set; }

        [Required(ErrorMessage = "Das Datum darf nicht leer sein")]

        public DateTime Datum { get; set; }

        public DateTime Startdatum { get; set; }
        public DateTime Enddatum { get; set; }
        public String Bereich { get; set; }

        public String Kundenname { get; set; }

        public String Betrieb { get; set; }
        public Guid Datei_id { get; set; }

        public String Dateiname { get; set; }
        public int Bericht_ID { get; set; }

        public DateTime Eintritt { get; set; }

        public DateTime Austritt { get; set; }

        public DateTime Geburtsdatum { get; set; }

        public bool? steht_im_konflikt { get; set; }
    }
}
