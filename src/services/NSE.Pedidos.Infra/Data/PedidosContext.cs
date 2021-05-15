using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using NSE.Core.DomainObjects;
using NSE.Core.Mediator;
using NSE.Core.Messages;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.Pedidos.Infra.Data
{
    public class PedidosContext : DbContext, IUnitOfWork
    {
        private IMediatorHandler _mediatorHandler;

        public PedidosContext(DbContextOptions<PedidosContext> options, IMediatorHandler mediatorHandler)
            : base(options)
        {
            // Desabilitamos pq nossa arquitetura nao depente desses ChangeTracker
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
            _mediatorHandler = mediatorHandler;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Event>();
            modelBuilder.Ignore<ValidationResult>();

            // Setar colunas string que nao foram mapeadas para varchar(100)
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            // Ira aplicar as configurações do PedidosContext
            // Entao qualquer coisa de Mapping que esteja implementando o IEntityTypeConfiguration para uma entidade que está sendo representada em nosso contexto PedidosContext
            // será mapeada e assim automaticamento ele vai aplicar esta e qualquer outra classe de configuração de mapeamento pro banco
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PedidosContext).Assembly);
        }

        public async Task<bool> Commit()
        {
            var sucesso = await base.SaveChangesAsync() > 0;
            if (sucesso) await _mediatorHandler.PublicarEventos(this);

            return sucesso;
        }

    }

    public static class MediatorExtension
    {
        // IMediatorHandler - interface nossa criada no projeto Core
        public static async Task PublicarEventos<T>(this IMediatorHandler mediator, T ctx) where T : DbContext
        {
            // ChangeTracker - memoria das entidades
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Notificacoes)
                .ToList();

            // Limpamos os eventos pois ja estao em memoria
            domainEntities.ToList()
                .ForEach(entity => entity.Entity.LimparEventos());

            // Para cada evento é criado uma task para publicar o mesmo
            var tasks = domainEvents
                .Select(async domainEvent =>
                {
                    await mediator.PublicarEvento(domainEvent);
                });

            // Quanto todas as tarefas estiverem completas finalizamos o metodo
            await Task.WhenAll(tasks);
        }
    }
}
