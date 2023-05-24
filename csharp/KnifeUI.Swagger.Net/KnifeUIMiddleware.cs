using System.Reflection;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;
namespace KnifeUI.Swagger.Net
{
    public class KnifeUIMiddleware
    {
        private const string EmbeddedFileNamespace = "KnifeUI.Swagger.Net";
        private readonly KnifeUIOptions _options;
        private readonly StaticFileMiddleware _staticFileMiddleware;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public KnifeUIMiddleware(
            RequestDelegate next,
            IHostingEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            KnifeUIOptions options
            )
        {
            _options = options ?? new KnifeUIOptions();
            _staticFileMiddleware = CreateStaticFileMiddleware(next, hostingEnv, loggerFactory, options);
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            _jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var httpMethod = httpContext.Request.Method;
            var path = httpContext.Request.Path.Value;
            // If the RoutePrefix is requested (with or without trailing slash), redirect to index URL
            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/?{Regex.Escape(_options.RoutePrefix)}/?$"))
            {
                // Use relative redirect to support proxy environments
                var relativeRedirectPath = path.EndsWith("/")? "doc.html" : $"{path.Split('/').Last()}/doc.html";
                RespondWithRedirect(httpContext.Response, relativeRedirectPath);
                return;
            }
            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/{Regex.Escape(_options.RoutePrefix)}/?doc.html$"))
            {
                await RespondWithIndexHtml(httpContext.Response);
                return;
            }
            if (httpMethod == "GET" && Regex.IsMatch(path, $"^/v3/api-docs/swagger-config$"))
            {
                await RespondWithConfig(httpContext.Response);
                return;
            }
            await _staticFileMiddleware.Invoke(httpContext);
        }

        private async Task RespondWithConfig(HttpResponse response)
        {
            await response.WriteAsync(JsonSerializer.Serialize(_options.ConfigObject, _jsonSerializerOptions));
        }

        private StaticFileMiddleware CreateStaticFileMiddleware(RequestDelegate next, IHostingEnvironment hostingEnv, ILoggerFactory loggerFactory,KnifeUIOptions options)
        {
            var staticFileOptions = new StaticFileOptions
            {
                RequestPath = string.IsNullOrEmpty(options.RoutePrefix) ? string.Empty : $"/{options.RoutePrefix}",
                FileProvider = new EmbeddedFileProvider(typeof(KnifeUIMiddleware).GetTypeInfo().Assembly, EmbeddedFileNamespace),
            };
            return new StaticFileMiddleware(next, hostingEnv, Options.Create(staticFileOptions), loggerFactory);
        }

        private void RespondWithRedirect(HttpResponse response, string location)
        {
            response.StatusCode = 301;
            response.Headers["Location"] = location;
        }

        private async Task RespondWithIndexHtml(HttpResponse response)
        {
            response.StatusCode = 200;
            response.ContentType = "text/html;charset=utf-8";
            if (_options.IndexStream()!=null)
            {
                using (var stream = _options.IndexStream())
                {
                    // Inject arguments before writing to response
                    var htmlBuilder = new StringBuilder(new StreamReader(stream).ReadToEnd());
                    foreach (var entry in GetIndexArguments())
                        htmlBuilder.Replace(entry.Key, entry.Value);
                    await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
                }
            }
        }

        private IDictionary<string, string> GetIndexArguments()
        {
            return new Dictionary<string, string>()
            {
                { "%(DocumentTitle)", _options.DocumentTitle },
                { "%(HeadContent)", _options.HeadContent },
                //{ "%(OAuthConfigObject)", JsonSerializer.Serialize(_options.OAuthConfigObject, _jsonSerializerOptions) }
            };
        }
    }
}
