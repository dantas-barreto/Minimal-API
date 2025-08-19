using MinimalApi.Domain.Enums;

namespace MinimalApi.Domain.ModelViews;

public record LogedAdministrator
{
    public string Email { get; set; } = default!;
    public string Profile { get; set; } = default!;
    public string token { get; set; } = default!;
}