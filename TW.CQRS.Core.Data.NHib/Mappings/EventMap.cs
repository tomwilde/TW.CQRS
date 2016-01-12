using FluentNHibernate.Mapping;

namespace TW.CQRS.Core.Data.NHib.Mappings
{
    public class EventMap : ClassMap<Event>
    {
        public EventMap()
        {
            Id(x => x.Id);

            Map(x => x.AggregateId);
            Map(x => x.EventData).Length(10000);
            Map(x => x.EventType);
            Map(x => x.Timestamp);

            Table("tblEvent");
        }
    }
}