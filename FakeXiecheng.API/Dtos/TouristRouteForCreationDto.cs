using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FakeXiecheng.API.Dtos
{
    public class TouristRouteForCreationDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1500)]
        public string Description { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999999999.99")]
        public decimal OriginalPrice { get; set; }

        [Range(0.0, 1.0)]
        public double? DiscountPrice { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public DateTime? DepartureTime { get; set; }

        public string Features { get; set; }

        public string Fees { get; set; }

        public string Notes { get; set; }

        public double? Rating { get; set; }

        public string TravelDays { get; set; }

        public string TripType { get; set; }

        public string DepartureCity { get; set; }

        public ICollection<TouristRoutePictureCreationDto> TouristRoutePictures { get; set; }
            = new List<TouristRoutePictureCreationDto>();
    }
}