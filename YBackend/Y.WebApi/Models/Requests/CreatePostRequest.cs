namespace Y.WebApi.Models.Requests;

public sealed class CreatePostRequest
{
    public string Headline { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
