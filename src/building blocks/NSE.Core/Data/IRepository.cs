using NSE.Core.DomainObjects;
using System;

namespace NSE.Core.Data
{
    // Todo repositorio tem que saber fazer o IDisposable
    // Apenas um repositorio por raiz de agregação
    public interface IRepository<T> : IDisposable where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

    }
}
