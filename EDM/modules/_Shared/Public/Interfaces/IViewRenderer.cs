using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Public.Interfaces
{
    public interface IViewRenderer
    {
        string RenderViewToString(string controllerName, string viewName, object model);
    }
}