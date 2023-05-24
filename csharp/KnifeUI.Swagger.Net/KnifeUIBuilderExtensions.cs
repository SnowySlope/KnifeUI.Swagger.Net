using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace KnifeUI.Swagger.Net
{
    public static class KnifeUIBuilderExtensions
    {
        public static IApplicationBuilder UseKnife4UI(this IApplicationBuilder app, Action<KnifeUIOptions>? setupAction = null)
        {
            var options = new KnifeUIOptions();
            if (setupAction != null)
                setupAction(options);
            else
                options = app.ApplicationServices.GetRequiredService<IOptions<KnifeUIOptions>>().Value;
            app.UseMiddleware<KnifeUIMiddleware>(options);
            return app;
        }
    }
}
