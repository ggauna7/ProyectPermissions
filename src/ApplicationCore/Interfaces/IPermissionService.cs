using Domain.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IPermissionService
    {
        Task CreatePermission(CreatePermission command);
        Task<List<Permission>> GetPermissions();
        Task ModifyPermission(ModifyPermission command);
    }
}
