using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Web.Templates
{
    public class RazorEmailRenderer
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorEmailRenderer(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderTemplateAsync(string templateName, object model)
        {
            // Find the Razor view template
            var viewResult = _razorViewEngine.GetView("~/Views/Emails/" + templateName + ".cshtml", null, isMainPage: false);

            if (!viewResult.Success)
            {
                throw new InvalidOperationException($"Template '{templateName}' not found.");
            }

            using var writer = new StringWriter();

            // Create a ViewContext
            var viewContext = new ViewContext
            {
                HttpContext = new DefaultHttpContext { RequestServices = _serviceProvider }, // Mock HttpContext for rendering
                View = viewResult.View,
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                },
                TempData = new TempDataDictionary(new DefaultHttpContext(), _tempDataProvider),
                Writer = writer
            };

            // Render the view to a string
            await viewResult.View.RenderAsync(viewContext);
            return writer.ToString();
        }
    }
}
