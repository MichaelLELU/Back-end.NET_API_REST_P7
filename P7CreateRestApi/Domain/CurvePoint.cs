using P7CreateRestApi.Domain.Common;

namespace P7CreateRestApi.Domain
{
    public class CurvePoint : BaseEntity
    {
        public byte CurveId { get; set; }
        public DateTime? AsOfDate { get; set; }
        public double Term { get; set; }
        public double CurvePointValue { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
