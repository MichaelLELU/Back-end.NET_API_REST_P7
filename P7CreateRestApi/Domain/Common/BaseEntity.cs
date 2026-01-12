using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Domain.Common
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
