using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Dto.CurvePoint
{
    public class UpdateCurvePointDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "L’identifiant de la courbe est obligatoire.")]
        public byte CurveId { get; set; }

        [Required(ErrorMessage = "Le délai est obligatoire.")]
        public double Term { get; set; }

        [Required(ErrorMessage = "La valeur est obligatoire.")]
        public double CurvePointValue { get; set; }
    }
}
