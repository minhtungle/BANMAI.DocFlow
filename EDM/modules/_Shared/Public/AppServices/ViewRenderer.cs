using Public.Interfaces;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Public.AppServices
{
    public class ViewRenderer : IViewRenderer
    {
        public string RenderViewToString(string controllerName, string viewName, object model)
        {
            // Lấy context hiện tại
            HttpContextBase httpContext = new HttpContextWrapper(System.Web.HttpContext.Current);

            // Tạo route giả định
            var routeData = new RouteData();
            routeData.Values["controller"] = controllerName;

            // Tạo controller giả
            var controller = CreateControllerInstance(controllerName);
            var controllerContext = new System.Web.Mvc.ControllerContext(new RequestContext(httpContext, routeData), controller);

            // Tìm View
            var viewEngineResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
            if (viewEngineResult.View == null)
            {
                throw new FileNotFoundException($"View '{viewName}' không tìm thấy trong controller '{controllerName}'.");
            }

            // Khởi tạo ViewData & TempData
            var viewData = new System.Web.Mvc.ViewDataDictionary(model);
            var tempData = new System.Web.Mvc.TempDataDictionary();

            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(
                    controllerContext,
                    viewEngineResult.View,
                    viewData,
                    tempData,
                    sw
                );

                viewEngineResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        private System.Web.Mvc.Controller CreateControllerInstance(string controllerName)
        {
            // Có thể dùng reflection để tạo controller thực tế nếu cần
            // Ở đây dùng DefaultController kế thừa Controller cơ bản
            return new DefaultController();
        }

        private class DefaultController : System.Web.Mvc.Controller { }
    }
}