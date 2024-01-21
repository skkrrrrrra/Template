namespace Domain.Constants
{
    public static class Columns
    {
        //base
        public const string Id = "id";
        public const string CreatedAt = "created_at";
        public const string UpdatedAt = "updated_at";
        public const string DeletedAt = "deleted_at";
        public const string IsDeleted = "is_deleted";


        //user
        public const string PhoneNumber = "phone_number";
        public const string Email = "email";
        public const string PasswordHash = "password_hash";
        public const string PasswordSalt = "password_salt";
        public const string Role = "role";


        //user_profile
        public const string UserId = "user_id";
        public const string FirstName = "first_name";
        public const string SecondName = "second_name";
        public const string Patronymic = "patronymic";
    }
}
