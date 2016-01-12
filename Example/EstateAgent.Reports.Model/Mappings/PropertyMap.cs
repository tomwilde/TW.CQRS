using FluentNHibernate.Mapping;

namespace EstateAgent.Reports.Model.Mappings
{
    public class PropertyMap : ClassMap<Property>
    {
        public PropertyMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
            Map(x => x.Price);
        }
    }
}