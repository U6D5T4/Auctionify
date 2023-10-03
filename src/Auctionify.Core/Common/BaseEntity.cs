using System.ComponentModel.DataAnnotations.Schema;

namespace Auctionify.Core.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        private readonly List<BaseEvent> domainEvents = new();

        [NotMapped]
        public IReadOnlyCollection<BaseEvent> DomainEvents => domainEvents.AsReadOnly();

        public void AddDomainEvent(BaseEvent domainEvent)
        {
            domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(BaseEvent domainEvent)
        {
            domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            domainEvents.Clear();
        }
    }
}
