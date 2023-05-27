using FluentValidation;
using Ifasanat.MeterMonitoringManagement.Dto;

namespace Ifasanat.MeterMonitoringManagement.Validators
{
    public class CityValidator : AbstractValidator<CityDto>
    {
        public CityValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(30);

        }
    }
}
