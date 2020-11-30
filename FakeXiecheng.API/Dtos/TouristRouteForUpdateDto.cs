using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FakeXiecheng.API.ValidationAttributes;

namespace FakeXiecheng.API.Dtos
{
    [TouristRouteTitleMustBeDifferentDescription]
    public class TouristRouteForUpdateDto : TouristRouteForManipulationDto
    {
        [Required(ErrorMessage = "更新必備")]
        [MaxLength(1500)]
        public override string Description { get; set; }
    }
}