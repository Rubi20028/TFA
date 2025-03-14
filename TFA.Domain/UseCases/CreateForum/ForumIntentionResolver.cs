using TFA.Domain.Authentication;
using TFA.Domain.Authorization;

namespace TFA.Domain.UseCases.CreateForum;

public class ForumIntentionResolver : IIntentionResolver<ForumIntention>
{
    public bool IsAllowed(IIdentity subject, ForumIntention intention)
    {
        return intention switch
        {
            ForumIntention.Create => subject.IsAuthenticated(),
            _ => false
        };
    }
}