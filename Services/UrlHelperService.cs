using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace App.Services;

public class UrlHelperService
{
    private readonly IUrlHelper urlHelper;

    public UrlHelperService(IUrlHelperFactory factory, IActionContextAccessor action)
    {   
        urlHelper = factory.GetUrlHelper(action.ActionContext);
    }

    public string GetLink(string Action, string Controller, string Area)
    {
        return urlHelper.Action(Action, Controller, new {area = Area});
    }
}