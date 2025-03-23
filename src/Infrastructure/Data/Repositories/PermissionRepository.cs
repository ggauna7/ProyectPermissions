using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Infrastructure.Data.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDbContext _context;

        public PermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Permission> GetPermissionByIdAsync(Guid id)
        {
            return await _context.Permissions
                .Include(p => p.PermissionType)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Permission>> GetPermissionsAsync()
        {
            return await _context.Permissions.Include(p => p.PermissionType).ToListAsync();
        }

        public async Task AddPermissionAsync(Permission permission)
        {
            try
            {
                await _context.Permissions.AddAsync(permission);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public async Task UpdatePermissionAsync(Permission permission)
        {
            // Obtener el permiso existente desde la base de datos
            var existingPermission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Id == permission.Id);

            if (existingPermission == null)
            {
                throw new ArgumentException("Permission not found", nameof(permission.Id));
            }

            // Actualizar las propiedades del permiso existente
            existingPermission.Name = permission.Name;
            existingPermission.Description = permission.Description;
            existingPermission.PermissionTypeId = permission.PermissionTypeId;

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();
        }
    }
}
