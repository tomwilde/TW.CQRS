using TW.CQRS.Core.Data.EF;

namespace EstateAgent.DB.Reporting
{
    public class GenericEstateAgentRepository<T> : AbstractRepository<ReportingContainer, T> 
        where T : class
    {
    }
}
