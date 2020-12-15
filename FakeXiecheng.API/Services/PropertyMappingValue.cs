using System.Collections.Generic;

namespace FakeXiecheng.API.Services
{
    public class PropertyMappingValue
    {
        public PropertyMappingValue(IEnumerable<string> destinationProperties)
        {
            DestinationProperties = destinationProperties;
        }

        public IEnumerable<string> DestinationProperties { get; private set; }
    }
}