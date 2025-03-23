namespace Domain.Entities
{
    public class PermissionType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Permission> Permissions { get; set; }
    }
}
