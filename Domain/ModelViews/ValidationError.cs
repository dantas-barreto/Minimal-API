namespace MinimalApi.Domain.ModelViews;

public struct ValidationError
{
    public List<string> ErrorMessages { get; set; }
}