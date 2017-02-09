using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.GTpay
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //PDT
            routes.MapRoute("Plugin.Payments.GTpay.PDTHandler",
                 "Plugins/PaymentGTpay/PDTHandler",
                 new { controller = "PaymentGTpay", action = "PDTHandler" },
                 new[] { "Nop.Plugin.Payments.GTpay.Controllers" }
            );
            //IPN
            routes.MapRoute("Plugin.Payments.GTpay.IPNHandler",
                 "Plugins/PaymentGTpay/IPNHandler",
                 new { controller = "PaymentGTpay", action = "IPNHandler" },
                 new[] { "Nop.Plugin.Payments.GTpay.Controllers" }
            );
            //Cancel
            routes.MapRoute("Plugin.Payments.GTpay.CancelOrder",
                 "Plugins/PaymentGTpay/CancelOrder",
                 new { controller = "PaymentGTpay", action = "CancelOrder" },
                 new[] { "Nop.Plugin.Payments.GTpay.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
