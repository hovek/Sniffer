using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.ModelConfiguration;
using System.Reflection;
using System.IO;

namespace Sniffer.Sniffs.Dvoznak
{
	public class TipElderDb : DbContext
	{
		public DbSet<Tip> Tip { get; set; }

		static TipElderDb()
		{
			//Database.SetInitializer<TipElderDb>(new TipElderDbInitializer());
			Database.SetInitializer<TipElderDb>(null);
		}

		/// <summary>
		/// Returns new instance.
		/// </summary>
		public static TipElderDb I
		{
			get
			{
				return new TipElderDb();
			}
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Configurations.Add(new TipConfig());
		}
	}

	public class TipElderDbInitializer : DropCreateDatabaseIfModelChanges<TipElderDb>
	{
		protected override void Seed(TipElderDb context)
		{
			base.Seed(context);

			context.Database.ExecuteSqlCommand("CREATE INDEX [IX_Odigravanje] ON [Tip] ([Odigravanje] ASC)");
			context.Database.ExecuteSqlCommand("CREATE INDEX [IX_Sport] ON [Tip] ([Sport] ASC)");
			context.Database.ExecuteSqlCommand("CREATE INDEX [IX_Tipster] ON [Tip] ([Tipster] ASC)");
			context.Database.ExecuteSqlCommand("CREATE INDEX [IX_TipOdigravanja] ON [Tip] ([TipOdigravanja] ASC)");
			context.Database.ExecuteSqlCommand("CREATE INDEX [IX_Zarada] ON [Tip] ([Zarada] ASC)");
			context.Database.ExecuteSqlCommand("CREATE INDEX [IX_Liga] ON [Tip] ([Liga] ASC)");

			ExecuteDDL(context);
		}

		public static void ExecuteDDL(TipElderDb context)
		{
			Assembly ass = Assembly.GetExecutingAssembly();
			List<string> resources = new List<string>(ass.GetManifestResourceNames());
			List<string> sps = resources.Where(r => r.Contains("Sniffer.Sniffs.Dvoznak.SQLObjects.") && r.Contains(".sql")).ToList();

			foreach (string res in sps)
			{
				context.Database.ExecuteSqlCommand(new StreamReader(ass.GetManifestResourceStream(res)).ReadToEnd());
			}
		}
	}

	public class TipConfig : EntityTypeConfiguration<Tip>
	{
		public TipConfig()
		{
			this.Ignore(p => p.DogadjajUrl);
			this.Ignore(p => p.TipsterUrl);
			this.Ignore(p => p.TipsterBrojOklada);
			this.Ignore(p => p.TipsterBrojPogodaka);
			this.Ignore(p => p.TipsterBrojPovrata);
			this.Ignore(p => p.TipsterUlog);
			this.Ignore(p => p.TipsterPovrat);
			this.Ignore(p => p.KladionicaUrl);
			this.Ignore(p => p.KladionicaImgUrl);
			this.Ignore(p => p.SportImgUrl);
			this.Ignore(p => p.ZastavaClass);
			this.Ignore(p => p.Uspjesnost);
			this.Ignore(p => p.UspjesnostTitle);
			this.Ignore(p => p.Opis);
			this.Ignore(p => p.Misc);

			Property(p => p.Liga).HasColumnType("varchar").HasMaxLength(900);
			Property(p => p.Dogadjaj).HasColumnType("varchar").HasMaxLength(900);
			Property(p => p.TipOdigravanja).HasColumnType("nvarchar").HasMaxLength(450);
			Property(p => p.Tipster).HasColumnType("varchar").HasMaxLength(900);
			Property(p => p.Koeficijent).HasPrecision(10, 3);
			Property(p => p.MinKoef).HasPrecision(10, 3);
			Property(p => p.Kladionica).HasColumnType("varchar").HasMaxLength(900);
			Property(p => p.Rezultat).HasColumnType("varchar").HasMaxLength(900);
			Property(p => p.Sport).HasColumnType("varchar").HasMaxLength(900);
			Property(p => p.Zarada).HasPrecision(10, 2);
			Property(p => p.IsMargina).HasColumnType("varchar").HasMaxLength(900);
		}
	}
}
