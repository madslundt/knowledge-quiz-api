using System.Collections.Generic;

namespace DataModel.Models.Localization
{
    public class LocaleReference
    {
        public Locale Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public ICollection<Localization> Localizations { get; set; }
    }

    public enum Locale
    {
        en_US = 1,
        da_DK = 2
    }
}
