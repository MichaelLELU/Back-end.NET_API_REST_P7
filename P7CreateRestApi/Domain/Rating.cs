using P7CreateRestApi.Domain.Common;

namespace P7CreateRestApi.Domain
{
    public class Rating : BaseEntity
    {
        public string MoodysRating { get; set; }
        public string SandPRating { get; set; }
        public string FitchRating { get; set; }
        public byte? OrderNumber { get; set; }
    }
}
