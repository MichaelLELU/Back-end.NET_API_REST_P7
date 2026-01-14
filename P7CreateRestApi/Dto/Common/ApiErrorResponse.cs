using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace P7CreateRestApi.Dto.Common
{
    public class ApiErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public IDictionary<string, string[]>? Errors { get; set; }

        public static ApiErrorResponse FromModelState(ModelStateDictionary modelState)
        {
            return new ApiErrorResponse
            {
                Message = "Erreur de validation",
                Errors = modelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
                    )
            };
        }
    }
}
