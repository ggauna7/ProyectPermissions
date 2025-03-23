using Domain.Entities;
using Domain.Interfaces;
using Moq;
using WebApi.ApplicationCore.Services;
using Xunit;

namespace UnitTest
{
    public class PermissionCommandHandlerTests
    {
        readonly Mock<IPermissionRepository> _mockPermissionRepository;
        readonly Mock<IElasticSearchService> _mockElasticsearchService;
        readonly Mock<IKafkaService> _mockKafkaService;
        readonly PermissionService _mockService;

        // ID del tipo de permiso "User"
        private readonly Guid _userPermissionTypeId = Guid.Parse("46A515A1-D258-40F1-B8DA-565DFDF505EA");

        public PermissionCommandHandlerTests()
        {
            // Inicializamos los mocks
            _mockPermissionRepository = new Mock<IPermissionRepository>();
            _mockElasticsearchService = new Mock<IElasticSearchService>();
            _mockKafkaService = new Mock<IKafkaService>();

            // Instanciamos PermissionService con las dependencias mockeadas
            _mockService = new PermissionService(
                _mockPermissionRepository.Object,
                _mockElasticsearchService.Object,
                _mockKafkaService.Object
            );

            // Configurar la simulación de los métodos de IPermissionRepository
            _mockPermissionRepository.Setup(repo => repo.AddPermissionAsync(It.IsAny<Permission>()))
                                     .Returns(Task.CompletedTask);

            _mockPermissionRepository.Setup(repo => repo.UpdatePermissionAsync(It.IsAny<Permission>()))
                                     .Returns(Task.CompletedTask);

            _mockPermissionRepository.Setup(repo => repo.GetPermissionsAsync())
                                     .ReturnsAsync(new List<Permission>
                                     {
                                     new Permission { Id = Guid.NewGuid(), Name = "Permission 1", Description = "Description 1", PermissionTypeId = _userPermissionTypeId },
                                     new Permission { Id = Guid.NewGuid(), Name = "Permission 2", Description = "Description 2", PermissionTypeId = _userPermissionTypeId }
                                     });
        }

        // Test para "Request Permission" (Crear Permiso)
        [Fact]
        public async Task Handle_CreatePermission_ShouldCreatePermissionAndCallDependencies()
        {
            // Arrange: Crear un comando de prueba con el PermissionTypeId "User"
            var command = new CreatePermission
            {
                Name = "Test Permission",
                Description = "Test Description",
                PermissionTypeId = _userPermissionTypeId
            };

            // Act: Llamamos al servicio (simulando el manejo de la solicitud)
            await _mockService.CreatePermission(command);

            // Assert: Verificamos que las dependencias fueron llamadas correctamente
            _mockPermissionRepository.Verify(repo => repo.AddPermissionAsync(It.IsAny<Permission>()), Times.Once);
            _mockElasticsearchService.Verify(es => es.IndexPermissionAsync(It.IsAny<Permission>()), Times.Once);
            _mockKafkaService.Verify(kafka => kafka.ProduceMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        // Test para "Modify Permission" (Modificar Permiso)
        [Fact]
        public async Task Handle_ModifyPermission_ShouldModifyPermissionAndCallDependencies()
        {
            // Arrange: Crear un comando de prueba con el PermissionTypeId "User"
            var command = new ModifyPermission
            {
                Id = Guid.NewGuid(),
                Name = "Updated Permission",
                Description = "Updated Description",
            };

            // Simular que el permiso existe en la base de datos
            var permission = new Permission
            {
                Id = command.Id,
                Name = "Old Permission",
                Description = "Old Description",
                PermissionTypeId = _userPermissionTypeId
            };
            _mockPermissionRepository.Setup(repo => repo.GetPermissionByIdAsync(command.Id)).ReturnsAsync(permission);

            // Act: Llamamos al servicio (simulando el manejo de la solicitud)
            await _mockService.ModifyPermission(command);

            // Assert: Verificamos que las dependencias fueron llamadas correctamente
            _mockPermissionRepository.Verify(repo => repo.UpdatePermissionAsync(It.IsAny<Permission>()), Times.Once);
            _mockElasticsearchService.Verify(es => es.IndexPermissionAsync(It.IsAny<Permission>()), Times.Once);
            _mockKafkaService.Verify(kafka => kafka.ProduceMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        // Test para "Get Permissions" (Obtener Permisos)
        [Fact]
        public async Task Handle_GetPermissions_ShouldReturnPermissions()
        {
            var permissions = new List<Permission>
        {
            new Permission { Id = Guid.NewGuid(), Name = "Permission 1", Description = "Description 1", PermissionTypeId = _userPermissionTypeId },
            new Permission { Id = Guid.NewGuid(), Name = "Permission 2", Description = "Description 2", PermissionTypeId = _userPermissionTypeId }
        };
            _mockPermissionRepository.Setup(repo => repo.GetPermissionsAsync()).ReturnsAsync(permissions);

            var result = await _mockService.GetPermissions();

            _mockPermissionRepository.Verify(repo => repo.GetPermissionsAsync(), Times.Once);
            Assert.Equal(2, result.Count);  // Asegúrate de que se devuelven 2 permisos
        }
    }

}
