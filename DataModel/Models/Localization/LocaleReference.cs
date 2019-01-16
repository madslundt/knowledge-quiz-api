using System.ComponentModel.DataAnnotations;

namespace DataModel.Models.Localization
{
    public class LocaleReference
    {
        public Locale Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public enum Locale
    {
        [Display(Name = "English")]
        en_US = 1,

        [Display(Name = "Dansk")]
        da_DK = 2
    }
}
