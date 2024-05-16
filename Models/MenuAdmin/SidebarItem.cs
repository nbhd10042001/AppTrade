using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace App.Models.MenuAdmin;


public enum SidebarItemType
{
    Divider,
    Heading,
    NavItem
}

public class SidebarItem
{
    public SidebarItemType Type {set; get;}
    public string Title {set; get;}
    public bool IsActive {set; get;}
    public string Controller {set; get;}
    public string Action {set; get;}
    public string Area {set; get;}
    public string AwesomeIcon {set; get;}
    public string CollapseId {set; get;}

    public List<SidebarItem> Items {set; get;}

    public string GetLink(IUrlHelper urlHelper)
    {
        return urlHelper.Action(Action, Controller, new {area = Area});
    }

    public string RenderHtml(IUrlHelper urlHelper)
    {   
        var html = new StringBuilder();

        if(Type == SidebarItemType.Divider)
        {
            html.Append("<hr class=\"sidebar-divider my-1\">");
        }
        else if(Type == SidebarItemType.Heading)
        {
            html.Append(@$"<div class=""sidebar-heading"">
                                {Title}
                            </div>");
        }
        else if(Type == SidebarItemType.NavItem)
        {
            if (Items == null)
            {
                var url = GetLink(urlHelper);
                var icon = (AwesomeIcon != null) ?  @$"<i class=""{AwesomeIcon}""></i>" : "";
                var cssClass = "nav-item";
                if (IsActive) cssClass += " active";

                html.Append(@$"<li class=""{cssClass}"">
                                    <a class=""nav-link"" href=""{url}"">
                                        {icon}
                                        <span>{Title}</span></a>
                                </li>");
            }
            else // Items != null
            {
                var url = GetLink(urlHelper);
                var icon = (AwesomeIcon != null) ?  @$"<i class=""{AwesomeIcon}""></i>" : "";
                var cssClass = "nav-item";
                if (IsActive) cssClass += " active";

                var cssCollapse = "collapse";
                // if (IsActive) cssCollapse += " show";

                var itemMenu = "";
                foreach(var item in Items)
                {
                    var urlItem = item.GetLink(urlHelper);
                    var cssClassItem = "collapse-item";
                    if (item.IsActive) cssClassItem += " active";

                    itemMenu += @$"<a class=""{cssClassItem}"" href=""{urlItem}"">{item.Title}</a>";
                }

                html.Append(@$"
                    <li class=""{cssClass}"">
                        <a class=""nav-link collapsed"" href=""#"" data-bs-toggle=""collapse"" data-bs-target=""#{CollapseId}""
                            aria-expanded=""true"" aria-controls=""{CollapseId}"">
                            {icon} <span>{Title}</span> 
                        </a>
                        
                        <div id=""{CollapseId}"" class=""{cssCollapse}"" aria-labelledby=""headingTwo"" data-bs-parent=""#accordionSidebar"">
                            <div class=""bg-white py-2 collapse-inner rounded"">
                                {itemMenu}
                            </div>
                        </div>
                    </li>
                ");
            }
        }

        return html.ToString();
    }
}