using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Dto.Bid
{
    public class CreateBidDto
    {
        [Required(ErrorMessage = "Le compte est obligatoire.")]
        public string Account { get; set; } = null!;

        [Required(ErrorMessage = "Le type de l’offre est obligatoire.")]
        public string BidType { get; set; } = null!;

        [Required(ErrorMessage = "La quantité de l’offre est obligatoire.")]
        [Range(1, double.MaxValue, ErrorMessage = "La quantité doit être supérieure à 0.")]
        public double BidQuantity { get; set; }
    }
}
