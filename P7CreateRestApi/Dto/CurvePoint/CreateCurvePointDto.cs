using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Dto.CurvePoint
{
    public class CreateCurvePointDto
    {
        [Required(ErrorMessage = "L’identifiant de la courbe est obligatoire.")]
        public byte CurveId { get; set; }

        [Required(ErrorMessage = "Le délai est obligatoire.")]
        public double Term { get; set; }

        [Required(ErrorMessage = "La valeur est obligatoire.")]
        public double CurvePointValue { get; set; }
    }
}
