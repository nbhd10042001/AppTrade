## Package cài đặt MSSQL và MySQL
dotnet add package MySql.Data --version 8.3.0
dotnet add package MySql.Data.EntityFramework
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 8.0.0
dotnet add package MySqlConnector.DependencyInjection
dotnet add package System.Data.SqlClient
dotnet add package Newtonsoft.Json

## Package/Tool Enity Framework
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet tool install --global dotnet-aspnet-codegenerator

dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools.DotNet
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Proxies

dotnet add package Microsoft.AspNetCore.Session
dotnet add package Microsoft.Extensions.Caching.Memory
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Logging
dotnet add package Microsoft.Extensions.Logging.Console
dotnet add package Bogus --version 35.5.0

## Package hỗ trợ tạo bảng bởi dotnet (dotnet sql cache search on nuget):
dotnet new tool-manifest 
dotnet tool install --local dotnet-sql-cache --version 8.0.0 (yêu cầu phiên bản dotnet 8.0)
dotnet add package Microsoft.Extensions.Caching.SqlServer

- Lệnh tạo bảng nhanh
dotnet sql-cache create "StringConnect" dbo NameTable
string connect : "Data Source=localhost,1433;Initial Catalog=webdb;User ID=SA;Password=Password123; TrustServerCertificate=True;"


## Package về dịch vụ gửi mail
dotnet add package MailKit
dotnet add package MimeKit
dotnet add package RestSharp

## Package Identity
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Microsoft.AspNetCore.Identity.UI
dotnet add package Microsoft.AspNetCore.Authentication
dotnet add package Microsoft.AspNetCore.Http.Abstractions
dotnet add package Microsoft.AspNetCore.Authentication.Cookies
dotnet add package Microsoft.AspNetCore.Authentication.Facebook
dotnet add package Microsoft.AspNetCore.Authentication.Google
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.Authentication.MicrosoftAccount
dotnet add package Microsoft.AspNetCore.Authentication.oAuth
dotnet add package Microsoft.AspNetCore.Authentication.OpenIDConnect
dotnet add package Microsoft.AspNetCore.Authentication.Twitter

## SignalR
dotnet add package Microsoft.AspNetCore.SignalR.Common --version 8.0.0
dotnet add package Microsoft.AspNetCore.SignalR.Client --version 8.0.0

## Package elFinder
dotnet add package elFinder.NetCore
 
 - Để làm việc với Migration và Scaffold. Sử dụng lệnh dotnet ef trên Terminal để kiểm tra ef đã cài đặt. Nếu chưa thì cài thêm tool runtime [Download .NET 8.0 Runtime (microsoft.com) ](https://dotnet.microsoft.com/en-us/download/dotnet/8.0/runtime?cid=getdotnetcore&os=windows&arch=x64)

## Cài đặt Package Webpack để đóng gói JS, CSS, SCSS
npm init -y   
npm i -D webpack webpack-cli
npm i node-sass postcss-loader postcss-preset-env 
npm i sass-loader css-loader cssnano 
npm i mini-css-extract-plugin cross-env file-loader
npm install copy-webpack-plugin
npm install npm-watch
npm install bootstrap 
npm install jquery 
npm install popper.js 



