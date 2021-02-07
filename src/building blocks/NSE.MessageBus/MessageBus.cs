using EasyNetQ;
using NSE.Core.Messages.Integration;
using Polly;
using RabbitMQ.Client.Exceptions;
using System;
using System.Threading.Tasks;

namespace NSE.MessageBus
{
    // Nosso MessageBus que abstrai o EasyNetQ e o EasyNetQ abstrai a proprio drive do rabbitMQ para dotnet que abstrai a propria conexao que poderiamos fazer na 
    // mao com o rabbitMQ, essa classe facilita assim trabalharmos com outros frameworks independente do EasyNetQ
    public class MessageBus : IMessageBus
    {
        private IBus _bus;
        private IAdvancedBus _advancedBus; // Recursos que a interface IBus não oferece, utilizado para recuperação de falhas entre outros
        private readonly string _connectionString;

        public MessageBus(string connectionString)
        {
            _connectionString = connectionString;
            TryConnect(); // Conexao
        }

        public bool IsConnected => _bus?.IsConnected ?? false;

        public IAdvancedBus AdvancedBus => _bus?.Advanced;

        public void Publish<T>(T message) where T : IntegrationEvent
        {
            TryConnect();
            _bus.Publish(message); // Chamamos o proprio metodo publish do EasyNetQ, porém com essa camada a mais de cuidado
        }

        public async Task PublishAsync<T>(T message) where T : IntegrationEvent
        {
            TryConnect();
            await _bus.PublishAsync(message);
        }
        public void Subscribe<T>(string subscriptionId, Action<T> onMessage) where T : class
        {
            TryConnect();
            _bus.Subscribe(subscriptionId, onMessage);
        }

        public void SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage) where T : class
        {
            TryConnect();
            _bus.SubscribeAsync(subscriptionId, onMessage);
        }

        public TResponse Request<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return _bus.Request<TRequest, TResponse>(request);
        }

        public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return await _bus.RequestAsync<TRequest, TResponse>(request);
        }

        public IDisposable Respond<TRequest, TResponse>(Func<TRequest, TResponse> responder)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return _bus.Respond<TRequest, TResponse>(responder);
        }

        public IDisposable RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return _bus.RespondAsync(responder);
        }

        private void TryConnect()
        {
            if (IsConnected) return;

            // EasyNetQException - descrita na documentação do EasyNetQ
            // BrokerUnreachableException - descrita na documentação do rabbitMQ
            var policy = Policy.Handle<EasyNetQException>() // Poly que utilizamos no front-end
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))); // Pra cada tentativas fazemos o exponencial de 2 sobre tentativa

            policy.Execute(() =>
            { 
                _bus = RabbitHutch.CreateBus(_connectionString);
                _advancedBus = _bus.Advanced; // Precisamos da interface pois nao conseguimos trabalhar diretamente com o _bus.Advanced
                _advancedBus.Disconnected += OnDisconnect; // Assim que for desconectado a conexao com o Bus de imediato tentamos conectar novamente
            });
        }

        // Padrao de assinatura de um EventHandler
        private void OnDisconnect(object s, EventArgs e)
        {
            var policy = Policy.Handle<EasyNetQException>()
                .Or<BrokerUnreachableException>()
                .RetryForever(); // Sempre tentara reconectar a mensageria

            policy.Execute(TryConnect);
        }

        public void Dispose()
        {
            _bus.Dispose();
        }
    }
}
