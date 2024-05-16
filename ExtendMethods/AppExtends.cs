using System.Net;
using Microsoft.AspNetCore.Http;

namespace App.ExtendMethods;

public static class AppExtends
{
    public static void AddStatusCodePage(this IApplicationBuilder app)
    {
        // tuy bien trang loi~ code 400 -> 599
        app.UseStatusCodePages(appError => {
            appError.Run(async httpContext => {
                var response = httpContext.Response;
                var code = response.StatusCode;

                var content = @$"
                    <html>
                        <head>
                            <meta charset='UTF-8'/>
                            <title>Error {code}</title>
                        </head>

                        <body>
                            <p style='color:red; font-size:30px'>
                                Error {code} - {(HttpStatusCode)code}
                            </p>
                        </body>
                    </html>";
                
                await response.WriteAsync(content);
            });
        });
    }
}