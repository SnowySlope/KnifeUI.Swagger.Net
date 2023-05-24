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
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // �����������Ƶ�ԭʼ��Сд
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // ���Կ�ֵ����
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        //��ʹ��IFormFile���������ļ��ϴ�ʱ��ͨ�������һ��[Consumes]���������ƽ��ܵ�ý�����͡�����ѡ������Ϊtrue����ö�IFormFile������[Consumes]Լ����ʹ����Խ����κ�ý�����͡�
        options.SuppressConsumesConstraintForFormFileParameters = true;
        options.SuppressInferBindingSourcesForParameters = true;
        options.SuppressModelStateInvalidFilter = true;
        options.SuppressMapClientErrors = true;
        options.ClientErrorMapping[404].Link = "https://*/404";
    });
builder.Services.AddHttpContextAccessor();
//����ȫ�ֻ�ȡHttpContext
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
    //�ǵ����ù�������:����xml�ĵ�
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
        c.DocumentTitle = "KnifeUI.Swagger.Net ʵ���ĵ�";
        c.RoutePrefix = "";
        c.SwaggerEndpoint($"v1/api-docs", $"MyAPI");
    });
    app.MapSwagger("{documentName}/api-docs");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
