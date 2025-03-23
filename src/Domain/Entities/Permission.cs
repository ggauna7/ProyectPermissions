namespace Domain.Entities
{
    public class Permission
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid PermissionTypeId { get; set; }

        public PermissionType PermissionType { get; set; }
    }
    public class ModifyPermission
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class CreatePermission
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid PermissionTypeId { get; set; }
    }
}
