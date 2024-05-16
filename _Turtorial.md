## (3) - Entity Framework, SQL server
  - Open file appsettings.json
      add ("ConnectionStrings": {
        "AppMVCConnectionString" : "Data Source=localhost,1433;Initial Catalog=AppMVC ;User ID=SA;Password=Password123; TrustServerCertificate=True;"
      })
  - Create file AppDbContext.cs in folder Models
  - Open file Program and add builder.Services.AddDbContext
  - Tao 1 migration moi va update len CSDL
    > dotnet ef migrations add Init
    > dotnet ef database update
  - Kiem tra database tren Azure Data da co AppMVC chua?
  - Tao Area Database va Controller de quan li cac Database
    > dotnet aspnet-codegenerator area Database
    > dotnet aspnet-codegenerator controller -name DbManageController -outDir Areas\Database\Controllers  -namespace App.Areas.Database.Controllers   
  - Tao folder theo ten controller la DbManage trong folder Views -> tao file Index.cshtml trong folder DbManage
  - Copy 2 file _ViewImports.cshtml va _ViewStart.cshtml vao trong folder Views
  - Tao file _MenuManagePartial.cshtml trong folder App\Views\Shared
  - Chen partial vua tao vao file _Layout.cshtml trong folder App\Views\Shared
  - Kich hoat hien thi log EntityFramework trong file appsettings.json

## (4) - Model Binding\Validation
  - Create Contact.cs in Models\Contact folder
  - Them property DbSet co kieu la Contact
  - Tao 1 migration moi va update CSDL
    > dotnet ef migrations add Contact
    > dotnet ef database update
  - Tao 1 Controller de quan li cac Contact
    > dotnet aspnet-codegenerator area Contact
    > dotnet aspnet-codegenerator controller -name ContactController -namespace App.Areas.Contact.Controllers -m App.Models.Contacts.ContactModel -udl -dc App.Models.AppDbContext -outDir Areas\Contact\Controllers
  - Xoa di action Edit trong ContactController va file Edit.cshtml trong foler Area\Contact\Views\Contact
  - Doi ten action va file Create thanh SendContact
  - Su dung Gulp bien dich SCSS/SASS thanh CSS

## (5) - Identity
  - Install Packages Indentity
  - Tao file .cs ten AppUser, chinh sua file AppDbContext.cs
  - Tao 1 migration moi va update CSDL
    > dotnet ef migrations add AddIdentity
    > dotnet ef database update
  - Dang ky dich vu Identity, Options Identity, Authentication Google,...
  - Them Authentication trong file appsettings.json
  - Tai file libman.json tren github va chay lenh libman restore tren terminal
  - Dang ky dich vu SendEmail va IdentityErrorDescriber trong Program.cs
  - Them partial _LogginPartial trong file _Layout.cshtml 
  - Tao ra cac roles (su dung migration) va User Admin... (su dung Action SeedData trong DbManageController)
  - Them policy hien thi Menu Manage voi tung Role tuong ung. (config trong file Program.cs -- services.AddAuthorization)
  - Inject dich vu Authorization trong _MenuManagePartial.cshtml

## (6) - Build model Category (Tag)
  - Tao file Category.cs trong folder Models\Blog
  - Khai bao 1 DbSet Category trong AppDbContext.cs
  - Su dung ky thuat Fluent API cho truong du lieu Slug
  - Tao migrations moi
    > dotnet ef migrations add AddCategory
    > dotnet ef database update
  - Tao 1 Area Blog va tao 1 controller voi model Category
   > dotnet aspnet-codegenerator area Blog
   > dotnet aspnet-codegenerator controller -name CategoryController -namespace App.Areas.Blog.Controllers -m App.Models.Blog.CategoryModel -udl -dc App.Models.AppDbContext -outDir Areas\Blog\Controllers
  - Copy 2 file _ViewImports.cshtml va _ViewStart.cshtml
  
## (7) - Xay dung model cac bai viet post cua blog, xay dung website
  - Tao 2 file PostBase.cs, PostCategory.cs va Post.cs trong folder Models\Blog
  - Tạo 1 bảng PostCategory để hình thành mối quan hệ nhiều-nhiều giữ các bài Post và các Category 
  - Cap nhat cac model vua tao trong AppDbContext.cs
  - Su dung ky thuat Fluent API de tao Primary Key duoc tao boi 2 field PostID va CategoryID trong PostCategory.cs
  - Tao mirgations moi
    > dotnet ef migrations add AddPost
    > dotnet ef database update
  - Tao Controller lam viec voi model Post
    > dotnet aspnet-codegenerator controller -name PostController -namespace App.Areas.Blog.Controllers -m App.Models.Blog.Post -udl -dc App.Models.AppDbContext -outDir Areas\Blog\Controllers
  - Su dung thu vien Bogus de phat sinh cac fake Categories va Posts trong file AppDbContext.cs
  - Su dung pagingModel de lam Page hien thi cac Post
  - Chinh sua lai Edit/Create/... cua Post
  - Xay dung tinh nang phat sinh URL theo Title neu ko nhap URL field (code trong Utilities/AppUtilities.cs)

## (8) - Tích hợp HTML Editor (WYSIWYG HTML)
  - Tich hop them thu vien sumernote trong file libman.json.
  - Thuc hien lenh de restore cac thu vien trong libman.json:
    > libman restore
  - Xay dung model Summernote.cs va partial _Summernote.cshtml.
    (Phải đảm bảo thư viện jquery được nạp trước thư viện summernote để tránh xảy ra lỗi )
  - Ap dung model va partial Summernote vao trong file Views\Home\Index.cshtml.
  - Ap dung tuong tu voi cac file Create.cshtml - Edit.cshtml cua App\Blog\Views\Category.
  - Ap dung tuong tu voi cac file Create.cshtml - Edit.cshtml cua App\Blog\Views\Post.

## (9) - Tích hợp thư viện quản lý file elFinder
  - Tich hop them thu vien jqueryui va elfinder trong libman.json:
    > libman restore
    > libman update jqueryui
    > libman update elfinder
  - Tao area Files va controller FileManagerController:
    > dotnet aspnet-codegenerator area Files
    > dotnet aspnet-codegenerator controller -name FileManagerController -outDir Areas\Files\Controllers
  - Copy 2 file _ViewImports.cshtml va _ViewStart.cshtml trong folder Areas\Files\Views.
  - Copy file _Layout.cshtml va paste vao folder Areas\Files\Views\Shared\.
  - Doi ten _Layout.cshtml -> _LayoutFileManager.cshtml va tuy chinh. Tuy chinh lai file _ViewStart.cshtml.
  - Tao file Index.cshtml trong folder Views\FileManager.
  - Nap them package.
    > dotnet add package elFinder.NetCore
  - Tao them 1 folder files trong path App\wwwroot\.
  - Thiet lap connector trong FileManagerController.cs va tuy chinh height trong file Index.cshtml & _LayoutFileManager.cshtml.
  - Tạo thư mục Uploads để lưu trữ hoặc truy cập các file ảnh.
  - Tùy biến đường dẫn Static Files trong file Program.cs.
  - Xoa thu muc files trong App\wwwroot vi khong can dung den.
  - Tuy chinh code trong Action Connector trong FileManagerController.cs.
  - Tich hop elFinder vao HTML Editor Summernote de tich hop hinh anh vao bai viet.
  - Tao pluggin trong _Summernote.cshtml de khoi chay elFinder, them toolbar elfinder trong Summernote.cs.

## (10) - Build page News, Blogs
  - Tao controller trong area Blog.
    > dotnet aspnet-codegenerator controller -name ViewPostController -namespace App.Areas.Blog.Controllers -outDir Areas\Blog\Controllers
  - Tao cac file View tuong tu cac Action trong Controller luu trong folder Areas\Blog\View\ViewPost\.
  - Trong _LayoutBlog.cshtml ta sẽ đổ vào layout chính là _Layout.cshtml. Như vây _LayoutBlog.cshtml sẽ là layout con của layout chính.
  - Tạo thêm _ViewStart.cshtml với layout là _LayoutBlog để view Index.cshtml của View\ViewPost tự động nạp  layout này.
  - Chinh sua giao dien. Tuy chinh them trong ~\css\site.css.
  - Xay dung Components hien thi chuyen muc Blog.
  - Tao Breadcrumb cho chuyen muc.

## (11) - Xây dựng model và các trang quản lý sản phẩm - website asp.net
  - Tạo các model ProductModel.cs, CategoryPdModel.cs, ProductCategoryPd.cs.
  - Cập nhật code trong AppDbContext.cs, khai bao cac DbSet va fluent API.
    > dotnet ef migrations add addProduct
    > dotnet ef database update
  - Tao ra seeddate fake cac Product va CategoryProduct.
  - Tao area Product.
    > dotnet aspnet-codegenerator area Product
  - Khoi tao cac controller va view cho CategoryProduct.
  - Tao model CreateProductModel ke thua ProductModel.
  - Khoi tao cac controller va view cho ProductManage.
  - Xây dựng 1 model ten ProductPhotoModel.cs, cap nhat code trong AppDbContext.cs va ProductModel.cs
  - Thuc hien them migration va update len database
    > dotnet ef migrations add AddProductPhoto
    > dotnet ef database update
  - Tạo thêm thư mục Products trong Uploads để lưu trữ các ảnh của Product.
  - Tạo chức năng upload ảnh cho Product, dùng Ajax để lấy các ảnh của sản phẩm và render.

## (12) - Trang xem sản phẩm và chức năng giỏ hàng cart - website asp.net
  - Copy ViewBlogController.cs rename ViewProductController.cs va chinh sua lai code.
  - Copy cac view render theo action trong ViewProductController.cs va chinh sua code.
  - Tao them CategoryProductSidebar.cs trong thu muc Views\Shared\Components.
  - Xay dung chu nang gio hang, dat hang. 
  - Dang ky cac dich vu memorycache, session trong Program.cs
  - Xay dung model CartItemModel.cs trong thu muc Areas\Product\Models
  - Xay dung service CartService.cs trong thu muc Areas\Product\Services
  - Dang ky dich vu CartService trong Program.cs
  - Tao ra _CartPartial.cshtml, Tao ra view Cart.cshtml de render ra chuc nang Gio hang.
  - Chinh sua code trong ViewProductController.cs voi cac Action lien quan den chuc nang Gio hang.
  
## (13) - Sử dụng FontAwesome và HTM Template SB Admin cho website Asp.net
  - Tải templet SbAdmin font-awesome của bootstrap: https://startbootstrap.com/theme/sb-admin-2#google_vignette
  - Nạp thêm các thư viện của libman. (hoặc tự code nạp thêm trong file libman.json, search gg cdnjs và nhập tên library để check version)
    > libman install jquery-easing
    > libman install font-awesome
  - Lưu ý, ver của font-awesome phải khớp với ver sử dụng trong SbAdmin. (download\startbootstrap-sb-admin-2-gh-pages\vendor\fontawesome-free\attribution.js)
    > libman restore
  - Nạp thêm thư viện font-awesome vào code của _Layout.cshtml
  - Tao file moi _LoginLayout.cshtml trong thu muc App/Areas/Identity/Views/Account. Copy code trong file login.html o thu muc download/startbootstrap-sb-admin-2-gh-pages/login.html vao file _LoginLayout.cshtml
  - Copy file sb-admin-2.min.css trong thu muc download/startbootstrap-sb-admin-2-gh-pages/css/sb-admin-2.min.css va paste vao thu muc App/wwwroot/css.
  - Copy file sb-admin-2.min.js trong thu muc download/startbootstrap-sb-admin-2-gh-pages/js/sb-admin-2.min.js va paste vao thu muc App/wwwroot/js.
  - Sua lai cac duong dan file trong _LoginLayout.cshtml
  - Chinh sua lai giao dien cua trang Register/ForgotPassword tuong tu nhu trang Login.
  - Tao areas AdminControlPanel va controller/view AdminCP.cs
  - Tao 1 layout rieng danh cho trang Admin (_LayoutAdmin.cshtml) trong thu muc Views\Shared
  - Copy code trong file blank.html (download\sd-admin-2\blank.html) vao file _LayoutAdmin.cshtml
  - Xây dựng model SidebarItem.cs để render ra các MenuAdmin dưới dạng mã html trong _layoutAdmin.cshtml.
  - Xây dựng Service SidebarAdminService.cs và đăng ký service trong Program.cs
  - Lưu ý: Cần phải đăng ký thêm dịch vụ IActionContextAccessor để có thể inject vào trong SidebarAdminService.cs
  để tránh xảy ra lỗi.
  - Khởi tạo các mục menu admin trong SidebarAdminService.cs.
  - Inject dịch vụ SidebarAdminService vào Index.cshtml của AdminCPController
  - Chạy hàm RenderHtml trong section Sidebar để tự động render các mã html theo từng mục menu, sau đó đổ sang view _LayoutAdmin.cshtml (@await RenderSectionAsync("Sidebar", false))
  - Cập nhật các View layout của các Controller: Category/Post/CategoryProduct/ProductManage thành _LayoutAdmin.cshtml
  - Thêm các section Sidebar và SetActive cho từng View của mỗi Controller trên.


