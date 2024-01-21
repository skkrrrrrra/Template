using Domain.Constants;
using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UserProfile : IBaseEntity<long>
    {
        [Column(Columns.FirstName)]
        public string FirstName { get; set; }

        [Column(Columns.SecondName)]
        public string SecondName { get; set; }

        [Column(Columns.Patronymic)]
        public string Patronymic { get; set; }

        [ForeignKey(Columns.UserId)]
        [Column(Columns.UserId)]
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
