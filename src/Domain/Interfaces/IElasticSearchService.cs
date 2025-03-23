using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IElasticSearchService
    {
        Task IndexPermissionAsync(Permission permission);
    }
}
