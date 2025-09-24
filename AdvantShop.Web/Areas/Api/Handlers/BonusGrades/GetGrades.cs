using AdvantShop.Areas.Api.Models.Bonuses;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Bonuses
{
    public class GetGrades : AbstractCommandHandler<GetGradesResponse>
    {
        public GetGrades()
        {
        }

        protected override GetGradesResponse Handle()
        {
            var grades = GradeService.GetAll();

            return new GetGradesResponse(grades);
        }
    }
}