using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatenAktualisierung.Models
{
    public class MitarbeiterContext : DbContext
    {

        public MitarbeiterContext(DbContextOptions<MitarbeiterContext> options)
            : base(options)
        {
        }

        //public DbSet<DatenAktualisierung.Models.Betrieb> Betrieb { get; set; }

        //public DbSet<DatenAktualisierung.Models.Bereich> Bereich { get; set; }

        public DbSet<DatenAktualisierung.Models.Person> Person { get; set; }

        //public DbSet<DatenAktualisierung.Models.Stammkostenstelle> Stammkostenstelle { get; set; }

        //public DbSet<DatenAktualisierung.Models.Kunde> Kunde { get; set; }


    }
}
