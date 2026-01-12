using P7CreateRestApi.Domain.Common;

namespace P7CreateRestApi.Domain
{
    public class RuleName : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Json { get; set; }
        public string Template { get; set; }
        public string SqlStr { get; set; }
        public string SqlPart { get; set; }
    }
}
