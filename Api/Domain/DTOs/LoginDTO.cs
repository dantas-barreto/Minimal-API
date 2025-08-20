namespace MinimalApi.DTO;
public class LoginDTO
{
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
    public string? Password { get; set; }
};