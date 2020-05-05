﻿using System;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Sociomedia.ProjectionSynchronizer.Application;

namespace Sociomedia.ProjectionSynchronizer.Infrastructure
{
    public class EventStoreOrg : IEventStore
    {
        private IEventStoreConnection _connection;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ILogger _logger;
        private readonly ITypeLocator _typeLocator;
        private EventStoreAllCatchUpSubscription _subscription;
        private DomainEventReceived _onEventReceived;

        public EventStoreOrg(ILogger logger, ITypeLocator typeLocator)
        {
            _logger = logger;
            _typeLocator = typeLocator;

            var jsonResolver = new PropertyCleanerSerializerContractResolver();
            jsonResolver.IgnoreProperty(typeof(IDomainEvent), "Version", "TimeStamp");
            jsonResolver.RenameProperty(typeof(IDomainEvent), "Id", "AggregateId");

            _serializerSettings = new JsonSerializerSettings {
                ContractResolver = jsonResolver,
                Formatting = Formatting.Indented
            };
        }

        // ----- Public methods

        public async Task Connect(string server, int port = 1113, string login = "admin", string password = "changeit")
        {
            _connection = EventStoreConnection.Create(new Uri($"tcp://{login}:{password}@{server}:{port}"), "Aggregator");
            await _connection.ConnectAsync();
        }

        public void StartListeningEvents(Position? position, DomainEventReceived onEventReceived)
        {
            _onEventReceived = onEventReceived;
            _subscription = _connection.SubscribeToAllFrom(position, CatchUpSubscriptionSettings.Default, EventAppeared, LiveProcessingStarted, SubscriptionDropped);
            _logger.Debug($"Subscribed from position {position}. Replaying missing events.");
        }

        public void StopListeningEvents()
        {
            if (_subscription == null) {
                throw new InvalidOperationException("Subscription not started");
            }
            _subscription.Stop();
        }

        // ----- Internal logic

        private void SubscriptionDropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason, Exception ex)
        {
            _logger.Debug($"Subscription dropped : {reason}.");
            _logger.Error(ex.ToString());
        }

        private void LiveProcessingStarted(EventStoreCatchUpSubscription obj)
        {
            _logger.Debug("Switched to live mode");
        }

        private async Task EventAppeared(EventStoreCatchUpSubscription arg1, ResolvedEvent evt)
        {
            try {
                if (!evt.OriginalStreamId.StartsWith("$")) {
                    _logger.Debug($"New event received. Stream : {evt.OriginalStreamId}, position: {evt.OriginalPosition}");
                    if (TryConvertToDomainEvent(evt, out var @event)) {
                        if (!evt.OriginalPosition.HasValue) {
                            throw new InvalidOperationException("No position in the stream ??");
                        }
                        await _onEventReceived(evt.OriginalPosition.Value, @event);
                    }
                }
            }
            catch (Exception ex) {
                _logger.Error(ex.Message);
                throw;
            }
        }

        private bool TryConvertToDomainEvent(ResolvedEvent @event, out IDomainEvent result)
        {
            try {
                result = ConvertToDomainEvent(@event);
                return true;
            }
            catch (UnknownEvent ex) {
                _logger.Error(ex.Message);
                result = null;
                return false;
            }
        }

        private IDomainEvent ConvertToDomainEvent(ResolvedEvent @event)
        {
            var json = Encoding.UTF8.GetString(@event.Event.Data);
            var type = _typeLocator.FindEventType(@event.Event.EventType);
            if (type == null) {
                throw new UnknownEvent(@event.Event.EventType);
            }
            return (IDomainEvent) JsonConvert.DeserializeObject(json, type, _serializerSettings);
        }
    }
}