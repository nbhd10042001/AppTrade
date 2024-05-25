using System.Text;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Model.Structures;

namespace App.Services;

public class DarkModeService
{
    private readonly UserManager<AppUser> _userManager;

    public DarkModeService (UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public string ChangeViewMode (bool darkMode = true)
    {
        var html = new StringBuilder();

        if (darkMode)
        {
            html.Append(@"
                <style>
                    .view-mode-sidebar{
                        background-color: rgb(26, 26, 26);
                        color: rgb(255, 255, 255) !important;
                    }

                    .view-mode-topbar{
                        background-color: rgb(55, 55, 55);
                        color: rgb(255, 255, 255) !important;
                    }
                </style>
            ");
        }
        else
        {
            html.Append(@"
                <style>
                    .view-mode-sidebar{
                        background-color: rgb(100, 100, 100);
                        color: rgb(0, 0, 0) !important;
                    }

                    .view-mode-topbar{
                        background-color: rgb(162, 231, 239);
                        color: rgb(0, 0, 0) !important;
                    }
                </style>
            ");
        }
        
        return html.ToString();
    }

    
}