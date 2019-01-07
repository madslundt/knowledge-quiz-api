using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace API.Infrastructure.Filter
{
    public class LocaleFilterAttribute : Attribute, IResourceFilter
    {
        private const DataModel.Models.Localization.Locale DEFAULT_LOCALE = DataModel.Models.Localization.Locale.en_US;

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var locale = context.HttpContext.Request.Headers[MagicString.MagicString.HEADER_LOCALE];

            if (string.IsNullOrWhiteSpace(locale) || !Enum.TryParse<DataModel.Models.Localization.Locale>(locale, true, out _))
            {
                context.HttpContext.Request.Headers[MagicString.MagicString.HEADER_LOCALE] = DEFAULT_LOCALE.ToString();
            }
        }
    }
}
