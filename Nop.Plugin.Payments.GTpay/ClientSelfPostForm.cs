using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Web;

namespace Nop.Plugin.Payments.GTpay
{
    public class ClientSelfPostForm
    {
        private NameValueCollection nvc = new NameValueCollection();

        public bool SafetyOn { get; set; }

        public string this[string name]
        {
            get { return nvc[name]; }
            set
            {
                nvc.Remove(name);
                nvc[name] = value;
            }
        }

        public string ActionURL { get; set; }
        public string Method { get; set; }
        public string RedirectMsg { get; set; }

        public string Build()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            sb.Append("<head />");
            sb.Append("<body>");

            sb.AppendFormat("<form action =\"{0}\" method=\"{1}\">", ActionURL, Method);

            foreach (string name in nvc.Keys)
            {
                sb.AppendFormat("<input type=\"hidden\" name=\"{0}\" value=\"{1}\">", name, SafetyOn ? HttpUtility.UrlEncode(nvc[name]) : nvc[name]);
            }

            sb.Append("<input type=\"submit\" value=\"Banki\">");
            sb.Append("<br />");
            sb.Append(RedirectMsg);
            sb.Append("</form>");
            sb.Append("<script language=\"javascript\" type=\"text/javascript\">");
            sb.Append("document.forms[0].submit();");
            sb.Append("</script>");
            sb.Append("</body>");
            sb.Append("</html>");

            return sb.ToString();
        }
    }
}
