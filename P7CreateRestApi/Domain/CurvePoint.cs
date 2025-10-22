using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Domain
{
        public class CurvePoint
        {
        [Key]
        public int Id { get; set; }
            public byte? CurveId { get; set; }
            public DateTime? AsOfDate { get; set; }
            public double? Term { get; set; }
            public double? CurvePointValue { get; set; }
            public DateTime? CreationDate { get; set; }
        }
    }
