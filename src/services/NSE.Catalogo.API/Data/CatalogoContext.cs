using Microsoft.EntityFrameworkCore;
using NSE.Catalogo.API.Models;
using NSE.Core.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.Catalogo.API.Data
{
    public class CatalogoContext : DbContext, IUnitOfWork
    {
        public CatalogoContext(DbContextOptions<CatalogoContext> options) 
            : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Setar colunas string que nao foram mapeadas para varchar(100)
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            // Ira aplicar as configurações do CatalogoContext
            // Entao qualquer coisa de Mapping que esteja implementando o IEntityTypeConfiguration para uma entidade que está sendo representada em nosso contexto CatalogoContext
            // será mapeada e assim automaticamento ele vai aplicar esta e qualquer outra classe de configuração de mapeamento pro banco
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogoContext).Assembly); 
        }

        public async Task<bool> Commit()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}
