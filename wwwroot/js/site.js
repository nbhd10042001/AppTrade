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
        // layoutAdmin controll panel
        $("#accordionSidebar").removeClass("sidebar-light");
        $("#accordionSidebar").addClass("sidebar-dark");
        $("#topbar-admin").removeClass("topbar-light");
        $("#topbar-admin").addClass("topbar-dark");
        $("#footer-admin").removeClass("topbar-light");
        $("#footer-admin").addClass("topbar-dark");

        
        //layout main
        $("#body-layout").removeClass("body-light");
        $("#body-layout").addClass("body-dark");
        $("#header-layout").removeClass("header-light");
        $("#header-layout").addClass("header-dark");
        $("#footer-layout").removeClass("header-light");
        $("#footer-layout").addClass("header-dark");
        $("#container-layout").removeClass("container-light");
        $("#container-layout").addClass("container-dark");

        $("#card-product").removeClass("card-product-light");
        $("#card-product").addClass("card-product-dark");
        $("#cart-table").addClass("text-white");
        $("#cart-table").removeClass("text-dark");

        $("#icon-dark-mode").addClass("fa-sun");
        $("#icon-dark-mode").removeClass("fa-moon");

    }

    function toggleLightMode(){
        // layoutAdmin controll panel
        $("#accordionSidebar").addClass("sidebar-light");
        $("#accordionSidebar").removeClass("sidebar-dark");
        $("#topbar-admin").removeClass("topbar-dark");
        $("#topbar-admin").addClass("topbar-light");
        $("#footer-admin").removeClass("topbar-dark");
        $("#footer-admin").addClass("topbar-light");

        //layout main
        $("#body-layout").removeClass("body-dark");
        $("#body-layout").addClass("body-light");
        $("#header-layout").removeClass("header-dark");
        $("#header-layout").addClass("header-light");
        $("#footer-layout").removeClass("header-dark");
        $("#footer-layout").addClass("header-light");
        $("#container-layout").removeClass("container-dark");
        $("#container-layout").addClass("container-light");

        $("#card-product").removeClass("card-product-dark");
        $("#card-product").addClass("card-product-light");
        $("#cart-table").addClass("text-dark");
        $("#cart-table").removeClass("text-white");

        $("#icon-dark-mode").removeClass("fa-sun");
        $("#icon-dark-mode").addClass("fa-moon");

    }
 });



