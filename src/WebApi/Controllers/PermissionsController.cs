using ApplicationCore.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(
            IPermissionService permissionService)
        {
            _permissionService = permissionService;

        }

        // Endpoint para crear un permiso (Request Permission)
        [HttpPost]
        public async Task<ActionResult> CreatePermission([FromBody] CreatePermission command)
        {
            try
            {
                await _permissionService.CreatePermission(command);
                return Ok(new { Message = "Permission created successfully" });
            }
            catch (Exception ex)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred", Error = ex.Message });
            }

        }

        // Endpoint para modificar un permiso (Modify Permission)
        [HttpPatch("{id}")]
        public async Task<ActionResult> ModifyPermission([FromBody] ModifyPermission command)
        {
            try
            {
                await _permissionService.ModifyPermission(command);
                return Ok(new { Message = "Permission modified successfully" });
            }
            catch (Exception ex)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred", Error = ex.Message });
            }

        }

        // Endpoint para obtener todos los permisos (Get Permissions)
        [HttpGet]
        public async Task<ActionResult<List<Permission>>> GetPermissions()
        {
            try
            {
                var permissions = await _permissionService.GetPermissions();
                return Ok(permissions);
            }
            catch (Exception ex)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred", Error = ex.Message });
            }

        }
    }
}
