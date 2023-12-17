using Y.Domain.Models;

namespace Y.WebApi.Models.Requests;

public sealed class CreateReactionPostRequest
{
    public PostReactions PostReaction { get; set; }
}
