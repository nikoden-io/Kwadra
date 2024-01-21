using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Domend;

public class VersionRouteConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            var controllerVersion = controller.Attributes
                .OfType<ApiVersionAttribute>()
                .SelectMany(attr => attr.Versions)
                .FirstOrDefault();

            if (controllerVersion != null)
            {
                var versionedRoute = $"v{controllerVersion.ToString()}/[controller]";
                controller.Selectors[0].AttributeRouteModel =
                    new AttributeRouteModel(new RouteAttribute(versionedRoute));
            }
        }
    }
}