using System.Collections.Generic;

namespace DataModel.Models.User
{
    public class MetadataTypeReference
    {
        public UserMetadataType Id { get; set; }
        public string Name { get; set; }
    }

    public enum UserMetadataType
    {
        Brand = 1,
        BuildNumber = 2,
        DeviceCountry = 3,
        DeviceLocale = 4,
        DeviceName = 5,
        MacAddress = 6,
        Manufacturer = 7,
        SystemName = 8,
        SystemVersion = 9,
        Timezone = 10,
        UniqueId = 11,
        Version = 12
    }
}
