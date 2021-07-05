using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatenAktualisierung.Data
{
    public class KrankenquoteContext: DbContext
    {
        public KrankenquoteContext(DbContextOptions<KrankenquoteContext> options)
           : base(options)
        {
        }
        public DbSet<DatenAktualisierung.Models.Anwesenheit> Anwesenheit { get; set; }

        public DbSet<DatenAktualisierung.Models.Anwesenheit_csv> Anwesenheit_csv { get; set; }
    }
}
