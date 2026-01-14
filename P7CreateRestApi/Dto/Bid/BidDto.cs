namespace P7CreateRestApi.Dto.Bid
{
    public class BidDto
    {
        public int Id { get; set; }
        public string Account { get; set; } = string.Empty;
        public string BidType { get; set; } = string.Empty;
        public double BidQuantity { get; set; }
    }
}
