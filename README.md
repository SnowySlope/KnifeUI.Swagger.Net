# KnifeUI.Swagger.Net
>来自[IGeekFan.AspNetCore.Knife4jUI](https://gitee.com/igeekfan/IGeekFan.AspNetCore.Knife4jUI)

#### 介绍
集成Knife4j、swagger、asp.net core搭建的后台api文档


一个swagger ui 库：**[knife4j UI](https://gitee.com/xiaoym/knife4j)**，支持 .NET Core3.0+或.NET Standard2.0。


[![nuget](https://img.shields.io/nuget/v/KnifeUI.Swagger.Net.svg?style=flat-square)](https://www.nuget.org/packages/KnifeUI.Swagger.Net) [![stats](https://img.shields.io/nuget/dt/KnifeUI.Swagger.Net.svg?style=flat-square)](https://www.nuget.org/stats/packages/KnifeUI.Swagger.Net?groupby=Version) [![GitHub license](https://img.shields.io/badge/license-MIT-green)](https://raw.githubusercontent.com/luoyunchong/IGeekFan.AspNetCore.Knife4jUI/master/LICENSE.txt)

## 相关依赖项
### [knife4j](https://gitee.com/xiaoym/knife4j)
- knife4j-vue
### [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

## 📚 快速开始

### 1.安装包

```
Install-Package KnifeUI.Swagger.Net -Version 1.0.3
```

### 2.注册服务（ConfigureServices）

```
public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
{
    Configuration = configuration;
    this.webHostEnvironment = webHostEnvironment;
}

public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyAPI", Version = "v1" });
        options.CustomOperationIds(apiDesc =>
        {
            var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
            return controllerAction.ControllerName + "-" + controllerAction.ActionName;
        });
        //记得设置工程属性:生成xml文档
        var xmlPath = Path.Combine(webHostEnvironment.ContentRootPath, Assembly.GetExecutingAssembly().GetName().Name + ".xml");
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath, true);
        };
    });
}
```

### 3. 中间件配置
```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
    	app.UseDeveloperExceptionPage();
    }
    app.UseKnife4UI(c =>
    {
        c.DocumentTitle = "KnifeUI.Swagger.Net 实例文档";
        c.RoutePrefix = "";
        c.SwaggerEndpoint($"v1/api-docs", $"MyAPI");
    });
    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapSwagger("{documentName}/api-docs");
        endpoints.MapControllers();
    });
}
```
### 4. 设置工程属性
![输入图片说明](https://images.gitee.com/uploads/images/2020/1115/204923_0dee5c84_871523.png "屏幕截图.png")

### 🔎 效果图
运行项目，打开 https://localhost:5001/doc.html#/home

![https://pic.downk.cc/item/5f2fa77b14195aa594ccbedc.jpg](https://pic.downk.cc/item/5f2fa77b14195aa594ccbedc.jpg)


### 更多配置请参考

- [https://github.com/domaindrivendev/Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

## 工程示例
- [普通asp.net core应用](https://gitee.com/jackletter/knife-ui_-swagger_-net/tree/master/csharp/samples/webapi)
- [集成网关多api文档](https://gitee.com/jackletter/knife-ui_-swagger_-net/tree/master/csharp/samples/gateway-demo)


## 下载调试
下载后，先打包前端项目：
```js
npm i
npm run build
```
然后，再运行示例工程
