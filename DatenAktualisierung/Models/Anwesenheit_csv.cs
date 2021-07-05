using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DatenAktualisierung.Models
{
    public class Anwesenheit_csv
    {
        public Anwesenheit_csv()
        { }

        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }


        [Index(0)]

        public String Mandant { get; set; }

        [Index(1)]
        public String Personalnummer { get; set; }

        [Index(2)]
        public String Name { get; set; }

        [Index(3)]
        public String Abteilung { get; set; }

        [Index(4)]
        public String Datum { get; set; }

        [Index(5)]
        public String Fehlgrund { get; set; }

        [Index(6)]

        [Column(TypeName = "decimal(18,2)")]
        public Nullable<decimal> Tage { get; set; }
    }
}
