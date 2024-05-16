using System.Text;
using App.Models.MenuAdmin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace App.Services;

public class SidebarAdminService
{
    private readonly IUrlHelper urlHelper;
    public List<SidebarItem> Items {set; get;} = new List<SidebarItem>();

    public SidebarAdminService(IUrlHelperFactory factory, IActionContextAccessor action)
    {   
        urlHelper = factory.GetUrlHelper(action.ActionContext);

        // Khoi tao cac muc sidebar
        Items.Add(new SidebarItem() {Type = SidebarItemType.Divider});
        Items.Add(new SidebarItem() {Type = SidebarItemType.Heading, Title="Quan li chung"});

        Items.Add(new SidebarItem() 
            {
                Type = SidebarItemType.NavItem,
                Title = "Quan li Database",
                AwesomeIcon = "fas fa-database",
                Items = null,
                Area = "Database",
                Controller = "DbManage",
                Action = "Index"
            });

        Items.Add(new SidebarItem() 
            {
                Type = SidebarItemType.NavItem,
                Title = "Quan li Lien he",
                AwesomeIcon = "far fa-address-card",
                Items = null,
                Area = "Contact",
                Controller = "Contact",
                Action = "Index"
            });
        
        Items.Add(new SidebarItem() {Type = SidebarItemType.Divider});

        Items.Add(new SidebarItem() 
            {
                Type = SidebarItemType.NavItem,
                Title = "Phan quyen va Thanh vien",
                AwesomeIcon = "far fa-folder",
                CollapseId = "RolesAndUsers",

                Items = new List<SidebarItem>()
                {
                    new SidebarItem()
                    {
                        Type = SidebarItemType.NavItem,
                        Items = null,
                        Title = "Quan li Role",
                        Area = "Identity",
                        Controller = "Role",
                        Action = "Index"
                    },
                    new SidebarItem()
                    {
                        Type = SidebarItemType.NavItem,
                        Items = null,
                        Title = "Quan li User",
                        Area = "Identity",
                        Controller = "User",
                        Action = "Index"
                    }

                }
            });

        Items.Add(new SidebarItem() {Type = SidebarItemType.Divider});

        Items.Add(new SidebarItem() 
            {
                Type = SidebarItemType.NavItem,
                Title = "Quan li Blog",
                AwesomeIcon = "far fa-folder",
                CollapseId = "BlogManage",

                Items = new List<SidebarItem>()
                {
                    new SidebarItem()
                    {
                        Type = SidebarItemType.NavItem,
                        Items = null,
                        Title = "Quan li Chuyen muc",
                        Area = "Blog",
                        Controller = "Category",
                        Action = "Index"
                    },
                    new SidebarItem()
                    {
                        Type = SidebarItemType.NavItem,
                        Items = null,
                        Title = "Quan li Bai viet",
                        Area = "Blog",
                        Controller = "Post",
                        Action = "Index"
                    }

                }
            });
        Items.Add(new SidebarItem() {Type = SidebarItemType.Divider});

        Items.Add(new SidebarItem() 
            {
                Type = SidebarItemType.NavItem,
                Title = "Quan li San pham",
                AwesomeIcon = "far fa-folder",
                CollapseId = "ProductManage",

                Items = new List<SidebarItem>()
                {
                    new SidebarItem()
                    {
                        Type = SidebarItemType.NavItem,
                        Items = null,
                        Title = "Quan li CMSP",
                        Area = "Product",
                        Controller = "CategoryProduct",
                        Action = "Index"
                    },
                    new SidebarItem()
                    {
                        Type = SidebarItemType.NavItem,
                        Items = null,
                        Title = "Quan li San pham",
                        Area = "Product",
                        Controller = "ProductManage",
                        Action = "Index"
                    }

                }
            });


    }

    public string RenderHtml()
    {
        var html = new StringBuilder();
        foreach(var item in Items)
        {
            html.Append(item.RenderHtml(urlHelper));
        }
        return html.ToString();
    }

    public void SetActive(string Controller, string Action, string Area)
    {
        foreach(var item in Items)
        {
            if(item.Controller == Controller && item.Action == Action && item.Area == Area)
            {
                item.IsActive = true;
                return;
            }
            else
            {
                if (item.Items != null)
                {
                    foreach(var itemChild in item.Items)
                    {
                        if(itemChild.Controller == Controller && itemChild.Action == Action && itemChild.Area == Area)
                        {
                            itemChild.IsActive = true;
                            item.IsActive = true;
                            return;
                        }
                    }
                }
            }
        }
    }
}