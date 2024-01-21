namespace Template.Persistence
{
    public interface IAuditUserProvider
    {
        long? GetUserId();
        string GetUserRole();
    }
}
