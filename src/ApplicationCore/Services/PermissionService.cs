using ApplicationCore.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace WebApi.ApplicationCore.Services
{
    public class PermissionService : IPermissionService
    {
        #region Members
        private readonly IPermissionRepository _permissionRepository;
        private readonly IElasticSearchService _elasticSearchService;
        private readonly IKafkaService _kafkaService;


        #endregion

        #region Constructors
        public PermissionService(IPermissionRepository permissionRepository, IElasticSearchService elasticsearchService, IKafkaService kafkaService)
        {
            _permissionRepository = permissionRepository;
            _elasticSearchService = elasticsearchService;
            _kafkaService = kafkaService;
        }
        #endregion

        #region Methods

        public async Task CreatePermission(CreatePermission command)
        {
            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Description = command.Description,
                PermissionTypeId = command.PermissionTypeId
            };

            await _permissionRepository.AddPermissionAsync(permission);
            await _elasticSearchService.IndexPermissionAsync(permission);  // Indexar en Elasticsearch
            await _kafkaService.ProduceMessageAsync("permissions-topic", $"Created Permission with ID: {permission.Id}");
        }
        public async Task ModifyPermission(ModifyPermission command)
        {
            var permission = await _permissionRepository.GetPermissionByIdAsync(command.Id);

            if (permission != null)
            {
                // Actualizamos los valores
                permission.Name = command.Name;
                permission.Description = command.Description;

                // Actualizamos el permiso en la base de datos
                await _permissionRepository.UpdatePermissionAsync(permission);

                // Enviar el permiso a Elasticsearch para indexarlo
                await _elasticSearchService.IndexPermissionAsync(permission);

                // Enviar un mensaje a Kafka sobre la modificación del permiso
                await _kafkaService.ProduceMessageAsync("permission-topic", $"Permission {command.Name} modified");
            }
            else
            {
                throw new Exception("Permission not found");
            }
        }

        // Manejador del comando para obtener todos los permisos
        public async Task<List<Permission>> GetPermissions()
        {
            return await _permissionRepository.GetPermissionsAsync();
        }
        #endregion
    }
}
