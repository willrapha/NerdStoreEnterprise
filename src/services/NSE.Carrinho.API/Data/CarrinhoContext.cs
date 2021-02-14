using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.Carrinho.API.Data
{
    public class CarrinhoContext : DbContext, IUnitOfWork
    {
        public CarrinhoContext(DbContextOptions<CarrinhoContext> options)
            : base(options)
        {
            // Desabilitamos pq nossa arquitetura nao depente desses ChangeTracker
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<ValidationResult>();

            // Setar colunas string que nao foram mapeadas para varchar(100)
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            // Onde houver relacionamento iremos desligar o delete cascade
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            // Ira aplicar as configurações do ClienteContext
            // Entao qualquer coisa de Mapping que esteja implementando o IEntityTypeConfiguration para uma entidade que está sendo representada em nosso contexto ClienteContext
            // será mapeada e assim automaticamento ele vai aplicar esta e qualquer outra classe de configuração de mapeamento pro banco
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CarrinhoContext).Assembly);
        }

        public async Task<bool> Commit()
        {
            var sucesso = await base.SaveChangesAsync() > 0;

            return sucesso;
        }
    }
}
