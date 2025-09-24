using System;
using InstaSharper.Classes;

namespace AdvantShop.Core.Services.Crm.Instagram
{
    public class InstagramChallengeRequiredException : Exception
    {
        public ChallengeRequiredResponse ChallengeRequired { get; private set; }

        public InstagramChallengeRequiredException(string message, ChallengeRequiredResponse challengeRequired) : base(message)
        {
            ChallengeRequired = challengeRequired;
        }
    }
}
