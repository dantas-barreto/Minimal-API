namespace MinimalApi.DTO;
public record VehicleDTO
{
    public string Model { get; set; } = default!;
    public string Brand { get; set; } = default!;
    public int Year { get; set; } = default!;
};