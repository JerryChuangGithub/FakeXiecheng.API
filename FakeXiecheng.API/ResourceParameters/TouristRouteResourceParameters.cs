using System.Text.RegularExpressions;

namespace FakeXiecheng.API.ResourceParameters
{
    public class TouristRouteResourceParameters
    {
        private string _rating;

        public string Keyword { get; set; }

        public string Rating
        {
            get => _rating;
            set
            {
                if (string.IsNullOrWhiteSpace(value) == false)
                {
                    var regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
                    var match = regex.Match(value);
                    if (match.Success)
                    {
                        RatingOperator = match.Groups[1].Value;
                        RatingValue = int.Parse(match.Groups[2].Value);
                    }
                }

                _rating = value;
            }
        }

        public string RatingOperator { get; set; }

        public int? RatingValue { get; set; }

        private int _pageNumber = 1;

        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                if (value > 0)
                {
                    _pageNumber = value;
                }
            }
        }

        private int _pageSize = 10;

        private const int MaxPageSize = 50;

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value > 0)
                {
                    _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
                }
            }
        }
    }
}