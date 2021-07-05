using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatenAktualisierung.Models
{
    public class Person
    {
        


            [Key]
            [Index(0)]
            public int IdInt { get; set; }
            [Index(1)]
            public Int64? PrevTransNr { get; set; }
            [Index(2)]
            public Int64? TransNr { get; set; }
            [Index(3)]
            public bool? isInaktiv { get; set; }

            [Index(4)]
            public String Language { get; set; }


            [Index(5)]
            public String Login { get; set; }

            [Index(6)]
            public String Password { get; set; }

            [Index(7)]
            public String Password2 { get; set; }

            [Index(8)]
            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:tt/mm/yyyy}")]
            public DateTime DatumLastMod { get; set; }

            [Index(9)]
            public String WinAccount { get; set; }

            [Index(10)]
            public bool? WinAuth { get; set; }

            [Index(11)]
            public String Email { get; set; }

            [Index(12)]
            public int BenutzerEinst { get; set; }

            [Index(13)]
            public int Mandant { get; set; }

            [Index(14)]
            public String Nummer { get; set; }

            [Index(15)]
            public String? Anrede { get; set; }

            [Index(16)]
            public String? Titel { get; set; }



            [Index(17)]
            public String? Strasse { get; set; }

            public String Name { get; set; }


            [Index(18)]
            public String PLZ { get; set; }




            [Index(19)]
            public String? Ort { get; set; }

            [Index(20)]
            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:tt/mm/yyyy}")]
            public DateTime Eintritt { get; set; }

            [Index(21)]
            public DateTime? Austritt { get; set; }

            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:tt/mm/yyyy}")]

            [Index(22)]

            public DateTime? Gueltigkeitsanfang { get; set; }

            [Index(23)]
            public String? Foto { get; set; }

            [Index(24)]
            public String? Schriftfarbe { get; set; }

            [Index(25)]
            public String Hintergrundfarbe { get; set; }

            [Index(26)]
            public bool? SchriftFett { get; set; }


            [Index(27)]
            public bool? FrischImportiert { get; set; }


            [DataType(DataType.Date)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:tt/mm/yyyy}")]
            [Index(28)]
            public DateTime Geburtsdatum { get; set; }

            [Index(29)]
            public int SimuPlan { get; set; }

            [Index(30)]
            public String SteuerId { get; set; }

            [Index(31)]
            public bool? Lohn { get; set; }

            [Index(32)]
            public bool? LogaAbrKreis { get; set; }

            [Index(33)]
            public String? LogaAbrKreisNr { get; set; }

            [Index(34)]
            public bool? LogaVertrag { get; set; }

            [Index(35)]
            public int LogaVertragsNr { get; set; }

            [Index(36)]
            public int? AKDBAbr { get; set; }

            [Index(37)]
            public String? DatevAlias { get; set; }

            [Index(38)]
            public bool? WinStempel { get; set; }

            [Index(39)]
            public bool? MailUngekl { get; set; }

            [Index(40)]
            public bool? EnableExchange { get; set; }

            [Index(41)]
            public bool? ExchangeUsePers { get; set; }
            [Index(42)]
            public String Vorname { get; set; }
            [Index(43)]
            public String Nachname { get; set; }
            [Index(44)]
            public int? AbwesNachr { get; set; }
            [Index(45)]
            public int? ExchangeVertr { get; set; }

            [Index(46)]
            public bool LdapAuth { get; set; }

            [Index(47)]
            public String Geschlecht { get; set; }



        
    }
}
