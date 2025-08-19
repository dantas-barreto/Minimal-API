namespace MinimalApi.Domain.ModelViews;

public struct Home
{
    public string Message { get => "Bem vindo ao Minimal API"; }
    public string Documentation { get => "/swagger"; }
}