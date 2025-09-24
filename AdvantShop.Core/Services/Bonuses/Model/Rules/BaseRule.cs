using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Core.Services.Bonuses.Model.Enums;
using AdvantShop.Core.Services.Localization;
using Newtonsoft.Json;
using Quartz;

namespace AdvantShop.Core.Services.Bonuses.Model.Rules
{
    public class BaseRule:IJob
    {
        public static BaseRule Get(CustomRule model)
        {
            switch (model.RuleType)
            {
                case ERule.BirthDay: return Creator<BirthDayRule>(model.Params);
                case ERule.CancellationsBonus: return Creator<CancellationsBonusRule>(model.Params);
                case ERule.NewCard: return Creator<NewCardRule>(model.Params);
                case ERule.ChangeGrade: return Creator<ChangeGradeRule>(model.Params);
                case ERule.CleanExpiredBonus: return Creator<CleanExpiredBonusRule>(model.Params);
                case ERule.PostingReview: return Creator<PostingReviewBonusRule>(model.Params);
                default:
                    throw new BlException(LocalizationService.GetResource("AdvantShop.Core.Services.Bonuses.Model.Rules.WrongType") + model.RuleType);
            }
        }

        private static BaseRule Creator<T>(string p) where T : BaseRule, new()
        {
            return string.IsNullOrWhiteSpace(p) || p == "null" ? new T() : JsonConvert.DeserializeObject<T>(p);
        }

        public static string Set(BaseRule model)
        {
            return JsonConvert.SerializeObject(model);
        }
       
        public void Execute(IJobExecutionContext context) => context.TryRun(() => Process(context));

        public virtual void Process(IJobExecutionContext contex)
        {
            
        }
    }
}
