using Domain.Constants;
using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table(Tables.UserProfiles)]
    public class UserProfile : BaseEntity<long>
    {
        [Column(Columns.FirstName)]
        public string FirstName { get; set; }

        [Column(Columns.SecondName)]
        public string SecondName { get; set; }

        [Column(Columns.Patronymic)]
        public string Patronymic { get; set; }

        [Column(Columns.UserId)]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }
        public virtual User User { get; set; }
    }
}
