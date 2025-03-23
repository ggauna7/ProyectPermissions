using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPermissionRepository
    {
        Task<Permission> GetPermissionByIdAsync(Guid id);
        Task<List<Permission>> GetPermissionsAsync();
        Task AddPermissionAsync(Permission permission);
        Task UpdatePermissionAsync(Permission permission);
    }
}
