using NPD.Workflow.Common;
using System.Web;
using System.Web.Mvc;

namespace NPD.Workflow
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (filters != null)
            {
                filters.Add(new HandleErrorAttribute());
                filters.Add(new SessionExpiration());
            }
        }
    }
}
