using Domain.Constants;
using Domain.Entities.Base;
using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table(Tables.Users)]
    public class User : BaseEntity<long>
    {
        [Column(Columns.Username)]
        public string Username { get; set; }

        [Column(Columns.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Column(Columns.Email)]
        public string Email { get; set; }

        [Column(Columns.PasswordHash)]
        public byte[] PasswordHash { get; set; }

        [Column(Columns.PasswordSalt)]
        public byte[] PasswordSalt { get; set; }

        [Column(Columns.Role)]
        public ClientRole Role { get; set; }

        public virtual UserProfile Profile { get; set; }
    }   
}
