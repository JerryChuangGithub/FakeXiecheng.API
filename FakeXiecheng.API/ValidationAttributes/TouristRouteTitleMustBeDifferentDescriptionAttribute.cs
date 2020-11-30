using System.ComponentModel.DataAnnotations;
using FakeXiecheng.API.Dtos;

namespace FakeXiecheng.API.ValidationAttributes
{
    public class TouristRouteTitleMustBeDifferentDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var touristRouteDto = (TouristRouteForCreationDto)validationContext.ObjectInstance;
            if (touristRouteDto.Title == touristRouteDto.Description)
            {
                return new ValidationResult(
                    "旅遊路線 Title 和 Description 不可相同",
                    new []{ "TouristRouteForCreationDto" });
            }

            return ValidationResult.Success;
        }
    }
}