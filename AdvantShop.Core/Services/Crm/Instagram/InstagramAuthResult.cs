namespace AdvantShop.Core.Services.Crm.Instagram
{
    public interface IInstagramAuthResult
    {

    }

    public class InstagramAuthResult : IInstagramAuthResult
    {
        public bool Result { get; set; }
        public string Error { get; set; }

        public InstagramAuthResult(bool result)
        {
            Result = result;
        }

        public InstagramAuthResult(bool result, string error)
        {
            Result = result;
            Error = error;
        }
    }

    public class InstagramAuthChallengeRequiredResult : InstagramAuthResult
    {
        public string ApiPath { get; set; }

        public InstagramAuthChallengeRequiredResult(bool result) : base(result)
        {
        }

        public InstagramAuthChallengeRequiredResult(bool result, string error) : base(result, error)
        {
        }
    }
}
