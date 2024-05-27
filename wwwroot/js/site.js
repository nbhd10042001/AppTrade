// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// save dark mode in local store
$(document).ready(function () {

    // toggle dark mode when click button
    $("#dark-mode-button").on("click", function () {
       if (localStorage.hasOwnProperty('isDarkMode')) 
        {
            if (localStorage.getItem('isDarkMode') == 'true') 
            {
                localStorage.setItem('isDarkMode', 'false');
                toggleLightMode();
            }
            else
            {
                localStorage.setItem('isDarkMode', 'true');
                toggleDarkMode();
            }
       }
       else
       {
            localStorage.setItem('isDarkMode', 'true');
       }
    });

    // kiem tra dark mode khi reload page
    if (localStorage.hasOwnProperty('isDarkMode')) 
    {
        if(localStorage.getItem('isDarkMode') == 'true')
        {
            toggleDarkMode();
        }
        else
        {
            toggleLightMode();
        }
    }
    else
    {
        localStorage.setItem('isDarkMode', 'true');
        toggleDarkMode();
    }
       

    function toggleDarkMode() {
        $("#accordionSidebar").removeClass("light-mode-sidebar");
        $("#accordionSidebar").addClass("dark-mode-sidebar");
        $("#topbar").removeClass("light-mode-topbar");
        $("#topbar").addClass("dark-mode-topbar");
        $("#footer").removeClass("light-mode-topbar");
        $("#footer").addClass("dark-mode-topbar");
        $("#icon-dark-mode").addClass("fa-sun");
        $("#icon-dark-mode").removeClass("fa-moon");

    }

    function toggleLightMode(){
        $("#accordionSidebar").removeClass("dark-mode-sidebar");
        $("#accordionSidebar").addClass("light-mode-sidebar");
        $("#topbar").removeClass("dark-mode-topbar");
        $("#topbar").addClass("light-mode-topbar");
        $("#footer").removeClass("dark-mode-topbar");
        $("#footer").addClass("light-mode-topbar");
        $("#icon-dark-mode").removeClass("fa-sun");
        $("#icon-dark-mode").addClass("fa-moon");

    }
 });



