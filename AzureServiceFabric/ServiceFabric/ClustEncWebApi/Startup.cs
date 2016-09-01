using System.Web.Http;
using Owin;
using System.Web.Http.Cors;

namespace ClustEncWebApi
{
    public static class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Removing xml formatter in order to leave JSON as a single output option.
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Enabling cross site scripting
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            appBuilder.UseWebApi(config);

            // Simple file server is enough for AngularJS based site we use.
            var physicalFileSystem = new Microsoft.Owin.FileSystems.PhysicalFileSystem(@".\wwwroot");
            var fileServerOptions = new Microsoft.Owin.StaticFiles.FileServerOptions
            {
                EnableDefaultFiles = true,
                RequestPath = Microsoft.Owin.PathString.Empty,
                FileSystem = physicalFileSystem
            };
            fileServerOptions.DefaultFilesOptions.DefaultFileNames = new[] { "index.html" };
            fileServerOptions.StaticFileOptions.FileSystem = fileServerOptions.FileSystem = physicalFileSystem;
            fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = true;

            appBuilder.UseFileServer(fileServerOptions);
        }
    }
}
