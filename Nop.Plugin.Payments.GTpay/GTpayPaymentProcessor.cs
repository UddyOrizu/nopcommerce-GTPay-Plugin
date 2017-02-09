using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.GTpay.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;
using Newtonsoft;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.GTpay
{


    /// <summary>
    /// GTpay payment processor
    /// </summary>
    /// 

    public class GTpayPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly GTpayPaymentSettings _GTpayPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IWebHelper _webHelper;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ITaxService _taxService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly HttpContextBase _httpContext;
        #endregion

        #region Ctor

        public GTpayPaymentProcessor(GTpayPaymentSettings GTpayPaymentSettings,
            ISettingService settingService, ICurrencyService currencyService,
            CurrencySettings currencySettings, IWebHelper webHelper,
            ICheckoutAttributeParser checkoutAttributeParser, ITaxService taxService, 
            IOrderTotalCalculationService orderTotalCalculationService, HttpContextBase httpContext)
        {
            this._GTpayPaymentSettings = GTpayPaymentSettings;
            this._settingService = settingService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._webHelper = webHelper;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._taxService = taxService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._httpContext = httpContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets Paypal URL
        /// </summary>
        /// <returns></returns>
        private string GetGTpayUrl()
        {
            return _GTpayPaymentSettings.UseSandbox ? _GTpayPaymentSettings.PostUrl : _GTpayPaymentSettings.PostUrl;
        }

   
        /// <summary>
        /// Verifies IPN
        /// </summary>
        /// <param name="formString">Form string</param>
        /// <param name="values">Values</param>
        /// <returns>Result</returns>
        public bool VerifyIpn(string formString, out Dictionary<string, string> values)
        {
            var req = (HttpWebRequest)WebRequest.Create(GetGTpayUrl());
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            //now PayPal requires user-agent. otherwise, we can get 403 error
            req.UserAgent = HttpContext.Current.Request.UserAgent;

            string formContent = string.Format("{0}&cmd=_notify-validate", formString);
            req.ContentLength = formContent.Length;

            using (var sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
            {
                sw.Write(formContent);
            }

            string response;
            using (var sr = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                response = HttpUtility.UrlDecode(sr.ReadToEnd());
            }
            bool success = response.Trim().Equals("VERIFIED", StringComparison.OrdinalIgnoreCase);

            values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string l in formString.Split('&'))
            {
                string line = l.Trim();
                int equalPox = line.IndexOf('=');
                if (equalPox >= 0)
                    values.Add(line.Substring(0, equalPox), line.Substring(equalPox + 1));
            }

            return success;
        }



        public InterswitchResponse InterSwitchResponse(string tranasactionid, string amount, string hash)
        {
            InterswitchResponse result = new InterswitchResponse();

            
            string liveurl = string.Format(_GTpayPaymentSettings.IpnUrl + "?mertid={0}&tranxid={1}&amount={2}&hash={3}", _GTpayPaymentSettings.MerchantID, tranasactionid, amount, hash);


            

            string paymentUrl = liveurl;

            string text;
            using (WebClient client = new WebClient())
            {
                
                text = client.DownloadString(paymentUrl);

                result = JsonConvert.DeserializeObject<InterswitchResponse>(text.Trim());

            }

            return result;
        }

        #region Status Codes
        public string ErrorCodeToString(string createStatus)
        {
            // a full list of status codes.
            switch (createStatus)
            {
                case "00":
                    return "Approved by Financial Institution";
                case "01":
                    return "Refer to Financial Institution";
                case "02":
                    return "Refer to Financial Institution, Special Condition ";
                case "03":
                    return "Invalid Merchant";
                case "04":
                    return "Pick-up card ";
                case "05":
                    return "Do Not Honor ";
                case "06":
                    return "Error";
                case "07":
                    return "Pick-Up Card, Special Condition";
                case "08":
                    return "Honor with Identification";
                case "09":
                    return "Request in Progress";
                case "10":
                    return "Approved by Financial Institution, Partial";
                case "11":
                    return "Honor with Identification";
                case "12":
                    return "Invalid Transaction";
                case "13":
                    return "Invalid Amount";
                case "15":
                    return "No Such Financial Institution";
                case "16":
                    return "Approved by Financial Institution, Update Track 3 ";
                case "17":
                    return "Customer Cancellation";
                case "18":
                    return "Customer Dispute";
                case "19":
                    return "Re-enter Transaction";
                case "20":
                    return "Invalid Response from Financial Institution";
                case "21":
                    return "No Action Taken by Financial Institution";
                case "22":
                    return "Suspected Malfunction ";
                case "23":
                    return "Unacceptable Transaction Fee";
                case "24":
                    return "File Update not Supported";
                case "26":
                    return "Duplicate Record ";
                case "27":
                    return "File Update Field Edit Error ";
                case "28":
                    return "File Update File Locked";

                case "29":
                    return "File Update Failed";
                case "30":
                    return "Format Error";
                case "31":
                    return "Bank Not Supported";
                case "32":
                    return "Completed Partially by Financial Institution";
                case "33":
                    return "Expired Card, Pick-Up";
                case "34":
                    return "Suspected Fraud, Pick-Up";
                case "35":
                    return "Contact Acquirer, Pick-Up";
                case "36":
                    return "Restricted Card, Pick-Up";
                case "37":
                    return "Call Acquirer Security, Pick-Up";
                case "38":
                    return "PIN Tries Exceeded, Pick-Up ";
                case "39":
                    return "No Credit Account ";
                case "40":
                    return "Function not supported";
                case "41":
                    return "Lost Card, Pick-Up";
                case "42":
                    return "No Universal Account";
                case "44":
                    return "No Investment Account ";

                case "51":
                    return "Insufficient Funds";
                case "52":
                    return "No Check Account";
                case "53":
                    return "No Savings Account";
                case "55":
                    return "Incorrect PIN";
                case "56":
                    return "No Card Record ";
                case "59":
                    return "Suspected Fraud ";

                case "60":
                    return "Contact Acquirer";
                case "62":
                    return "Restricted Card  ";
                case "63":
                    return "Security Violation ";
                case "64":
                    return "Original Amount Incorrect ";
                case "65":
                    return "Exceeds withdrawal frequency ";
                case "66":
                    return "Call Acquirer Security";
                case "67":
                    return "Hard Capture";
                case "68":
                    return "Response Received Too Late ";
                case "77":
                    return "Intervene, Bank Approval Required";
                case "78":
                    return "Intervene, Bank Approval Required for Partial Amount";
                case "54":
                    return "Expired Card ";
                case "A0":
                    return "Unexpected error ";
                case "A4":
                    return "Transaction not permitted to card holder, via channels ";
                case "91":
                    return "Issuer or Switch Inoperative ";
                case "98":
                    return "Exceeds Cash Limit ";
                case "Z0":
                    return "Transaction Status Unconfirmed";
                case "Z1":
                    return "Transaction Error";
                case "Z2":
                    return "Bank account error ";
                case "Z3":
                    return "Bank collections account error ";
                case "Z4":
                    return "Interface Integration Error";
                case "Z5":
                    return "Duplicate Reference Error Z5";
                case "Z6":
                    return "Incomplete Transaction";
                case "Z7":
                    return "Transaction Split Pre-processing Error ";
                case "Z8":
                    return "Invalid Card Number, via channels ";
                case "Z9":
                    return "Transaction not permitted to card holder, via channels ";
                case "75":
                    return "Incorrect security details provided. Pin tries exceeded.";
                case "61":
                    return "Your bank has prevented your card from carrying out this transaction, please contact your bank";
                case "X00":
                    return "Account error, please contact your bank";
                case "X03":
                    return "The amount requested is above the limit permitted by your bank, please contact your bank";
                case "X04":
                    return "The amount requested is too low.";
                case "X05":
                    return "The amount requested is above the limit permitted by your bank, please contact your bank";
                case "14":
                    return "The card number inputted is invalid, please re-try with a valid card number";
                case "57":
                    return "Your bank has prevented your card from carrying out this transaction, please contact your bank";


                default:
                    return "Transaction Error";
            }
        }
        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;
            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {

            ClientSelfPostForm GTpaySubmitForm = new ClientSelfPostForm();
            GTpaySubmitForm.ActionURL = _GTpayPaymentSettings.PostUrl;
            GTpaySubmitForm.Method = "POST";
            GTpaySubmitForm.RedirectMsg = "Redirecting you to the payment Gateway (this will take a few seconds)";

            GTpaySubmitForm["gtpay_mert_id"] = _GTpayPaymentSettings.MerchantID;
            //sendToSecure.Add("gtpay_return_url", SiteConfiguration.gtpay_return_url);
            GTpaySubmitForm["gtpay_tranx_curr"] =_GTpayPaymentSettings.CurrencyCode;
            GTpaySubmitForm["gtpay_gway_first"] =_GTpayPaymentSettings.ShowGateWayFirst;

            if (_GTpayPaymentSettings.ShowGateWayFirst ==  "yes")
            {
                 GTpaySubmitForm["gtpay_gway_name"] = _GTpayPaymentSettings.GateWayName;
            }
             GTpaySubmitForm["gtpay_cust_id"] = postProcessPaymentRequest.Order.CustomerId.ToString();
             GTpaySubmitForm["gtpay_cust_name"] = postProcessPaymentRequest.Order.BillingAddress.FirstName+" "+postProcessPaymentRequest.Order.BillingAddress.LastName;

            int i = 1;
            var actualTotal = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);





            var amt = actualTotal.ToString("0.00");
            var final = amt.Split('.');
            var postedAmount = final[0] + final[1];
            GTpaySubmitForm["gtpay_tranx_amt"] = postedAmount;
            GTpaySubmitForm["gtpay_tranx_memo"] = "Online Shopping @ "+_webHelper.GetStoreHost(false);//pass store name here

                       
            var orderid =  postProcessPaymentRequest.Order.Id;

            var hash = string.Empty;
            // [gtpay_tranx_id + gtpay_tranx_amt + gtpay_tranx_noti_url + hashkey]
            string returnUrl = _webHelper.GetStoreLocation(false) + "Plugins/PaymentGTpay/PDTHandler";
            var hashdata = postProcessPaymentRequest.Order.OrderGuid + "|" + orderid + postedAmount + returnUrl + "?txid=" + postProcessPaymentRequest.Order.OrderGuid + "|" + orderid + _GTpayPaymentSettings.HashCode;
            hash = Hash.GetHashMini(hashdata, Hash.HashType.SHA512); //GetSHA512(hashdata);

            GTpaySubmitForm["gtpay_tranx_hash"] = hash;

            GTpaySubmitForm["gtpay_tranx_id"] = postProcessPaymentRequest.Order.OrderGuid + "|" + orderid;
            GTpaySubmitForm["gtpay_tranx_noti_url"] = returnUrl + "?txid=" + postProcessPaymentRequest.Order.OrderGuid + "|" + orderid;
            GTpaySubmitForm["gtpay_echo_data"] = postProcessPaymentRequest.Order.OrderGuid + "|" + orderid;

            _httpContext.Response.Clear();
            _httpContext.Response.Write(GTpaySubmitForm.Build());
            _httpContext.Response.Flush();
            _httpContext.Response.End();
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
                _GTpayPaymentSettings.AdditionalFee, _GTpayPaymentSettings.AdditionalFeePercentage);
            return result;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            
            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return false;

            return true;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentGTpay";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.GTpay.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentGTpay";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.GTpay.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentGTpayController);
        }

        public override void Install()
        {
            //settings
            var settings = new GTpayPaymentSettings
            {
                UseSandbox = false,
                MerchantID = "1234",
                Interswitch_Merchantid ="1234",
                PostUrl ="https://ibank.gtbank.com/GTPay/Tranx.aspx",
                IpnUrl ="https://ibank.gtbank.com/GTPayService/gettransactionstatus.json",
                GTpayProductID = "1234",
                GateWayName="webpay",
                ShowGateWayFirst = "no",
                CurrencyCode= "566",
                PdtValidateOrderTotal = true,
                HashCode ="Your Hash Here...",
                EnableIpn = true,
                AddressOverride = true,
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.RedirectionTip", "You will be redirected to payment site to complete the order.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.UseSandbox", "Use Sandbox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.BusinessEmail", "Business Email");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.BusinessEmail.Hint", "Specify your PayPal business email.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.PDTToken", "PDT Identity Token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.PDTToken.Hint", "Specify PDT identity token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.PDTValidateOrderTotal", "PDT. Validate order total");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.PDTValidateOrderTotal.Hint", "Check if PDT handler should validate order totals.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.PassProductNamesAndTotals", "Pass product names and order totals to PayPal");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.PassProductNamesAndTotals.Hint", "Check if product names and order totals should be passed to PayPal.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.EnableIpn", "Enable IPN (Instant Payment Notification)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.EnableIpn.Hint", "Check if IPN is enabled.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.EnableIpn.Hint2", "Leave blank to use the default IPN handler URL.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.IpnUrl", "IPN Handler");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.IpnUrl.Hint", "Specify IPN Handler.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.AddressOverride", "Address override");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.AddressOverride.Hint", "For people who already have PayPal accounts and whom you already prompted for a shipping address before they choose to pay with PayPal, you can use the entered address instead of the address the person has stored with PayPal.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage", "Return to order details page");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.GTpay.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage.Hint", "Enable if a customer should be redirected to the order details page when he clicks \"return to store\" link on PayPal site WITHOUT completing a payment");

            base.Install();
        }
        
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<GTpayPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.RedirectionTip");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.UseSandbox");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.UseSandbox.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.BusinessEmail");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.BusinessEmail.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.PDTToken");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.PDTToken.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.PDTValidateOrderTotal");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.PDTValidateOrderTotal.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.PassProductNamesAndTotals");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.PassProductNamesAndTotals.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.EnableIpn");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.EnableIpn.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.EnableIpn.Hint2");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.IpnUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.IpnUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.AddressOverride");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.AddressOverride.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage");
            this.DeletePluginLocaleResource("Plugins.Payments.GTpay.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage.Hint");
            
            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Redirection;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}
