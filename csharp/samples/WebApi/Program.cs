using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using KnifeUI.Swagger.Net;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    { 
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // 保留属性名称的原始大小写
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // 忽略空值属性
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        //当使用IFormFile参数接收文件上传时，通常会添加一个[Consumes]属性来限制接受的媒体类型。将此选项设置为true会禁用对IFormFile参数的[Consumes]约束，使其可以接受任何媒体类型。
        options.SuppressConsumesConstraintForFormFileParameters = true;
        options.SuppressInferBindingSourcesForParameters = true;
        options.SuppressModelStateInvalidFilter = true;
        options.SuppressMapClientErrors = true;
        options.ClientErrorMapping[404].Link = "https://*/404";
    });
builder.Services.AddHttpContextAccessor();
//配置全局获取HttpContext
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI", Version = "v1" });
    options.CustomOperationIds(apiDesc =>
    {
        var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
        return controllerAction.ControllerName + "-" + controllerAction.ActionName;
    });
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    //记得设置工程属性:生成xml文档
    var xmlPath = Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".xml");
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath, true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseKnife4UI(c =>
    {
        c.DocumentTitle = "KnifeUI.Swagger.Net 实例文档";
        c.RoutePrefix = "";
        c.SwaggerEndpoint($"v1/api-docs", $"MyAPI");
    });
    app.MapSwagger("{documentName}/api-docs");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
