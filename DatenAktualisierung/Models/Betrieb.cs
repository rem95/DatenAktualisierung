using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatenAktualisierung.Models
{
    public class Betrieb
    {
        [Key]
        public int Id { get; set; }
        public String Name { get; set; }
        public String Nummer { get; set; }
        public String Kunde_ID { get; set; }
    }
}
