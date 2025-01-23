using System.Globalization;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace HUBT_Social_Core.ASP_Extensions;

public static class LocalizationConfig
{
    public static IServiceCollection ConfigureLocalization(this IServiceCollection services)
    {
        // Đăng ký localization và các ngôn ngữ được hỗ trợ
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            // Các ngôn ngữ được hỗ trợ
            var supportedCultures = new[]
            {
                new CultureInfo("vi"), // Tiếng Việt
                new CultureInfo("en")  // Tiếng Anh
                // Có thể thêm các ngôn ngữ khác ở đây nếu cần
            };

            const string defaultCulture = "en"; // Ngôn ngữ mặc định là tiếng Anh

            // Cấu hình ngôn ngữ mặc định và ngôn ngữ hỗ trợ
            options.DefaultRequestCulture = new RequestCulture(defaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures; // Các UI cultures hỗ trợ

            // Xử lý lựa chọn ngôn ngữ từ query string, cookie hoặc header 'Accept-Language'
            options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider()); // Đọc từ query string
        });

        return services;
    }

    public static IApplicationBuilder UseLocalization(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(options);
        LocalValue.Initialize(app.ApplicationServices.GetRequiredService<IStringLocalizer<SharedResource>>());
        return app;
    }
}