using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Models.User
{
    public class UserMetadata
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Brand { get; set; }
        public string BuildNumber { get; set; }
        public string DeviceCountry { get; set; }
        public string DeviceId { get; set; }
        public string DeviceLocale { get; set; }
        public string DeviceName { get; set; }
        public string Manufacturer { get; set; }
        public string SystemName { get; set; }
        public string SystemVersion { get; set; }
        public string Timezone { get; set; }
        public string UniqueId { get; set; }
        public string Version { get; set; }

        public DateTime Created { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
