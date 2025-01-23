using Microsoft.Extensions.Localization;

namespace HUBT_Social_Core.Settings
{
    public static class LocalValue
    {
        private static IStringLocalizer<SharedResource>? _localizer;

        // Phương thức này sẽ được gọi một lần trong suốt vòng đời ứng dụng để inject IStringLocalizer
        public static void Initialize(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        // Phương thức lấy giá trị localization
        public static string Get(string key)
        {
            if (_localizer == null)
            {
                return "Localization is null.";
            }

            return _localizer[key].Value;
        }
    }
}
