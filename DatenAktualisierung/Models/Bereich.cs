using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatenAktualisierung.Models
{
    public class Bereich
    {
        [Key]
        public int ID { get; set; }

        public String Name { get; set; }

        public String Nummer { get; set; }

        public String Betrieb_ID { get; set; }


    }
}
