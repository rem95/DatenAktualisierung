using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatenAktualisierung.Models
{
    public class Stammkostenstelle
    {
        [Key]
        public int ID { get; set; }

        public String Nr { get; set; }

        public String Bereich_ID { get; set; }
    }
}
