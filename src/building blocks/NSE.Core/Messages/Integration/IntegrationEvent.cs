using System;

namespace NSE.Core.Messages.Integration
{
    // Classe base para os eventos de integração
    public abstract class IntegrationEvent : Event
    {

    }

    // Estamos colocando essa classe no Core pq ela pode tbm ser utilizada pelo Cliente
    public class UsuarioRegistradoIntegrationEvent : IntegrationEvent
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Cpf { get; private set; }

        // Esse usuario vai se tornar um cliente
        public UsuarioRegistradoIntegrationEvent(Guid id, string nome, string email, string cpf)
        {
            Id = id;
            Nome = nome;
            Email = email;
            Cpf = cpf;
        }
    }
}
