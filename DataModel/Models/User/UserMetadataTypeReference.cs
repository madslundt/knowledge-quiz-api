using System.Collections.Generic;

namespace DataModel.Models.User
{
    public class UserMetadataTypeReference
    {
        public UserMetadataType Id { get; set; }
        public string Name { get; set; }

        public ICollection<UserMetadata> UserMetadata { get; set; }
    }

    public enum UserMetadataType
    {
        Brand = 1,
        BuildNumber = 2,
        DeviceCountry = 3,
        DeviceId = 4,
        DeviceLocale = 5,
        DeviceName = 6,
        MACAddress = 7,
        Manufacturer = 8,
        SystemName = 9,
        SystemVersion = 10,
        Timezone = 11,
        UniqueId = 12,
        Version = 13
    }
}
