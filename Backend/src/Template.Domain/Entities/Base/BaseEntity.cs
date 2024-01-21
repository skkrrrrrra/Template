using Domain.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Base
{
    public abstract class BaseEntity<TId>
    {
        [Key]
        [Column(Columns.Id)]
        public new TId Id { get; set; }

        [Column(Columns.CreatedAt)]
        public DateTime CreatedAt { get; set; }

        [Column(Columns.UpdatedAt)]
        public DateTime UpdatedAt { get; set; }

        [Column(Columns.DeletedAt)]
        public DateTime? DeletedAt { get; set; } = null;

        [Column(Columns.IsDeleted)]
        public bool IsDeleted { get; set; } = false;
    }
}
