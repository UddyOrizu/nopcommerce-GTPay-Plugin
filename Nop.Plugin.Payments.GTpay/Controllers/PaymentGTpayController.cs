using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.GTpay.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.GTpay.Controllers
{
    public class PaymentGTpayController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly PaymentSettings _paymentSettings;
        private readonly GTpayPaymentSettings _GTpayPaymentSettings;

        public PaymentGTpayController(IWorkContext workContext,
            IStoreService storeService, 
            ISettingService settingService, 
            IPaymentService paymentService, 
            IOrderService orderService, 
            IOrderProcessingService orderProcessingService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            ILogger logger, 
            IWebHelper webHelper,
            PaymentSettings paymentSettings,
            GTpayPaymentSettings GTpayPaymentSettings)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
            this._logger = logger;
            this._webHelper = webHelper;
            this._paymentSettings = paymentSettings;
            this._GTpayPaymentSettings = GTpayPaymentSettings;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var GTpayPaymentSettings = _settingService.LoadSetting<GTpayPaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            model.UseSandbox = GTpayPaymentSettings.UseSandbox;
            model.MerchantID = GTpayPaymentSettings.MerchantID;
            model.HashCode = GTpayPaymentSettings.HashCode;
            //model.PdtToken = GTpayPaymentSettings.PdtToken;
            model.Interswitch_Merchantid = GTpayPaymentSettings.Interswitch_Merchantid;
            model.GateWayName = GTpayPaymentSettings.GateWayName;
            model.CurrencyCode = GTpayPaymentSettings.CurrencyCode;
            model.GTpayProductID = GTpayPaymentSettings.GTpayProductID;
            model.PostUrl = GTpayPaymentSettings.PostUrl;
            model.ReturnUrl = GTpayPaymentSettings.ReturnUrl;
            model.ShowGateWayFirst = GTpayPaymentSettings.ShowGateWayFirst;

            model.PdtValidateOrderTotal = GTpayPaymentSettings.PdtValidateOrderTotal;
            model.AdditionalFee = GTpayPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = GTpayPaymentSettings.AdditionalFeePercentage;
            model.PassProductNamesAndTotals = GTpayPaymentSettings.PassProductNamesAndTotals;
            model.EnableIpn = GTpayPaymentSettings.EnableIpn;
            model.IpnUrl = GTpayPaymentSettings.IpnUrl;
            model.AddressOverride = GTpayPaymentSettings.AddressOverride;
            model.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage = GTpayPaymentSettings.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.UseSandbox, storeScope);
                model.MerchantID_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.MerchantID, storeScope);
                model.CurrencyCode_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.CurrencyCode, storeScope);

                model.GateWayName_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.GateWayName, storeScope);

                model.HashCode_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.HashCode, storeScope);

                model.GTpayProductID_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.GTpayProductID, storeScope);
                model.Interswitch_Merchantid_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.Interswitch_Merchantid, storeScope);

                model.PostUrl_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.PostUrl, storeScope);
                model.ReturnUrl_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.ReturnUrl, storeScope);
                model.ShowGateWayFirst_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.ShowGateWayFirst, storeScope);

                model.PdtValidateOrderTotal_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.PdtValidateOrderTotal, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
                model.PassProductNamesAndTotals_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.PassProductNamesAndTotals, storeScope);
                model.EnableIpn_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.EnableIpn, storeScope);
                model.IpnUrl_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.IpnUrl, storeScope);
                model.AddressOverride_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.AddressOverride, storeScope);
                model.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore = _settingService.SettingExists(GTpayPaymentSettings, x => x.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage, storeScope);
            }

            return View("~/Plugins/Payments.GTpay/Views/PaymentGTpay/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var GTpayPaymentSettings = _settingService.LoadSetting<GTpayPaymentSettings>(storeScope);

            //save settings
            GTpayPaymentSettings.UseSandbox = model.UseSandbox;
            GTpayPaymentSettings.MerchantID = model.MerchantID;
            GTpayPaymentSettings.CurrencyCode = model.CurrencyCode;
            GTpayPaymentSettings.GateWayName = model.GateWayName;
            GTpayPaymentSettings.GTpayProductID = model.GTpayProductID;
            GTpayPaymentSettings.Interswitch_Merchantid = model.Interswitch_Merchantid;
            GTpayPaymentSettings.PostUrl = model.PostUrl;
            GTpayPaymentSettings.ReturnUrl = model.ReturnUrl;
            GTpayPaymentSettings.ShowGateWayFirst = model.ShowGateWayFirst;

            GTpayPaymentSettings.HashCode = model.HashCode;

            GTpayPaymentSettings.PdtValidateOrderTotal = model.PdtValidateOrderTotal;
            GTpayPaymentSettings.AdditionalFee = model.AdditionalFee;
            GTpayPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            GTpayPaymentSettings.PassProductNamesAndTotals = model.PassProductNamesAndTotals;
            GTpayPaymentSettings.EnableIpn = model.EnableIpn;
            GTpayPaymentSettings.IpnUrl = model.IpnUrl;
            GTpayPaymentSettings.AddressOverride = model.AddressOverride;
            GTpayPaymentSettings.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage = model.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.UseSandbox_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.UseSandbox, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.UseSandbox, storeScope);

            if (model.HashCode_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.HashCode, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.HashCode, storeScope);

            if (model.MerchantID_OverrideForStore|| storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.MerchantID, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.MerchantID, storeScope);

            if (model.CurrencyCode_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.CurrencyCode, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.CurrencyCode, storeScope);

            if (model.GateWayName_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.GateWayName, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.GateWayName, storeScope);

            if (model.GTpayProductID_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.GTpayProductID, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.GTpayProductID, storeScope);


            if (model.Interswitch_Merchantid_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.Interswitch_Merchantid, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.Interswitch_Merchantid, storeScope);

            if (model.PostUrl_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.PostUrl, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.PostUrl, storeScope);

            if (model.ReturnUrl_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.ReturnUrl, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.ReturnUrl, storeScope);

            if (model.ShowGateWayFirst_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.ShowGateWayFirst, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.ShowGateWayFirst, storeScope);

            if (model.PdtValidateOrderTotal_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.PdtValidateOrderTotal, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.PdtValidateOrderTotal, storeScope);

            if (model.AdditionalFee_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.AdditionalFee, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.AdditionalFee, storeScope);

            if (model.AdditionalFeePercentage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.AdditionalFeePercentage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.AdditionalFeePercentage, storeScope);

            if (model.PassProductNamesAndTotals_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.PassProductNamesAndTotals, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.PassProductNamesAndTotals, storeScope);

            if (model.EnableIpn_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.EnableIpn, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.EnableIpn, storeScope);

            if (model.IpnUrl_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.IpnUrl, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.IpnUrl, storeScope);

            if (model.AddressOverride_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.AddressOverride, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.AddressOverride, storeScope);

            if (model.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(GTpayPaymentSettings, x => x.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(GTpayPaymentSettings, x => x.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            return View("~/Plugins/Payments.GTpay/Views/PaymentGTpay/PaymentInfo.cshtml");
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }

        [ValidateInput(false)]
        public ActionResult PDTHandler(FormCollection form)
        {
            var tx = _webHelper.QueryString<string>("txid");
            Dictionary<string, string> values;
            //string response;

            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.GTpay") as GTpayPaymentProcessor;
            if (processor == null ||
                !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("Gtpay module cannot be loaded");


                

                var orderNref = tx.Split('|');

                string orderGuid = orderNref[0].Trim();
                string orderid = orderNref[1];
                
                Guid orderNumberGuid = System.Guid.Empty;
                try
                {
                    orderNumberGuid = new Guid(orderGuid);
                }
                catch { }
                Order order = _orderService.GetOrderByGuid(orderNumberGuid);
                if (order != null)
                {
                            var actualTotal = order.OrderTotal;
                            //actualTotal = actualTotal + SiteConfiguration.BookingFee;
                            var amt = actualTotal.ToString("0.00");
                            var final = amt.Split('.');
                            var postedAmount = final[0] + final[1];

                            var hash = _GTpayPaymentSettings.MerchantID + tx + _GTpayPaymentSettings.HashCode;
                            hash = Hash.GetHashMini(hash, Hash.HashType.SHA512);// GetSHA512(hash);
                         
                           var response = processor.InterSwitchResponse(tx,postedAmount, hash);


                           if (response.ResponseCode == "00")
                           {


                               decimal mc_gross = decimal.Zero;
                               try
                               {

                                   mc_gross = decimal.Parse(response.Amount, new CultureInfo("en-US"));
                                   mc_gross = mc_gross / 100;
                               }
                               catch (Exception exc)
                               {
                                   _logger.Error("GTpay. Error getting Ammount paid", exc);
                               }

                               string payer_status = processor.ErrorCodeToString(response.ResponseCode);
                               //values.TryGetValue("payer_status", out payer_status);
                               string payment_status = response.ResponseCode;
                               //values.TryGetValue("payment_status", out payment_status);
                               string pending_reason = response.ResponseDescription;
                               //values.TryGetValue("pending_reason", out pending_reason);
                               //string mc_currency = string.Empty;
                               ///values.TryGetValue("mc_currency", out mc_currency);
                               string txn_id = tx;
                               //values.TryGetValue("txn_id", out txn_id);
                               string payment_type = response.MerchantReference;
                               //values.TryGetValue("payment_type", out payment_type);
                               string payer_id = response.MertID;
                               //values.TryGetValue("payer_id", out payer_id);
                               //string receiver_id = string.Empty;
                               //values.TryGetValue("receiver_id", out receiver_id);
                               //string invoice = string.Empty;
                               //values.TryGetValue("invoice", out invoice);
                               string payment_fee = response.Amount;
                               //values.TryGetValue("payment_fee", out payment_fee);

                               var sb = new StringBuilder();
                               sb.AppendLine("GTpay PDT:");
                               sb.AppendLine("amount: " + mc_gross);
                               sb.AppendLine("Payer status: " + payer_status);
                               sb.AppendLine("Payment status: " + payment_status);
                               sb.AppendLine("Pending reason: " + pending_reason);
                               //sb.AppendLine("mc_currency: " + mc_currency);
                               sb.AppendLine("txn_id: " + txn_id);
                               sb.AppendLine("MerchantReference: " + payment_type);
                               sb.AppendLine("payer_id: " + payer_id);
                               //sb.AppendLine("receiver_id: " + receiver_id);
                               //sb.AppendLine("invoice: " + invoice);
                               sb.AppendLine("returned_ammount: " + payment_fee);

                               var newPaymentStatus = GTpayHelper.GetPaymentStatus(payment_status, pending_reason);
                               sb.AppendLine("New payment status: " + newPaymentStatus);

                               //order note
                               order.OrderNotes.Add(new OrderNote
                               {
                                   Note = sb.ToString(),
                                   DisplayToCustomer = false,
                                   CreatedOnUtc = DateTime.UtcNow
                               });
                               _orderService.UpdateOrder(order);

                               //load settings for a chosen store scope
                               var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
                               var GTpayPaymentSettings = _settingService.LoadSetting<GTpayPaymentSettings>(storeScope);

                               //validate order total
                               if (GTpayPaymentSettings.PdtValidateOrderTotal && !Math.Round(mc_gross, 2).Equals(Math.Round(order.OrderTotal, 2)))
                               {
                                   string errorStr = string.Format("Returned order total {0} doesn't equal order total {1}", mc_gross, order.OrderTotal);
                                   _logger.Error(errorStr);

                                   return RedirectToAction("Index", "Home", new { area = "" });
                               }

                               //mark order as paid
                               if (newPaymentStatus == PaymentStatus.Paid)
                               {
                                   if (_orderProcessingService.CanMarkOrderAsPaid(order))
                                   {
                                       order.AuthorizationTransactionId = response.MerchantReference;
                                       _orderService.UpdateOrder(order);

                                       _orderProcessingService.MarkOrderAsPaid(order);
                                   }
                               }

                               return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                           }
                           else
                           {
                               if (order != null)
                               {
                                   //order note
                                   order.OrderNotes.Add(new OrderNote
                                   {
                                       Note = "Gtpay PDT failed.",
                                       DisplayToCustomer = false,
                                       CreatedOnUtc = DateTime.UtcNow
                                   });
                                   _orderService.UpdateOrder(order);
                               }
                               return RedirectToAction("Index", "Home", new { area = "" });
                           }

                
            }
            else
            {
                
                
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        [ValidateInput(false)]
        public ActionResult IPNHandler()
        {
            byte[] param = Request.BinaryRead(Request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);
            Dictionary<string, string> values;

            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.GTpay") as GTpayPaymentProcessor;
            if (processor == null ||
                !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("GTPAY Standard module cannot be loaded");

            if (processor.VerifyIpn(strRequest, out values))
            {
                #region values
                decimal mc_gross = decimal.Zero;
                try
                {
                    mc_gross = decimal.Parse(values["mc_gross"], new CultureInfo("en-US"));
                }
                catch { }

                string payer_status = string.Empty;
                values.TryGetValue("payer_status", out payer_status);
                string payment_status = string.Empty;
                values.TryGetValue("payment_status", out payment_status);
                string pending_reason = string.Empty;
                values.TryGetValue("pending_reason", out pending_reason);
                string mc_currency = string.Empty;
                values.TryGetValue("mc_currency", out mc_currency);
                string txn_id = string.Empty;
                values.TryGetValue("txn_id", out txn_id);
                string txn_type = string.Empty;
                values.TryGetValue("txn_type", out txn_type);
                string rp_invoice_id = string.Empty;
                values.TryGetValue("rp_invoice_id", out rp_invoice_id);
                string payment_type = string.Empty;
                values.TryGetValue("payment_type", out payment_type);
                string payer_id = string.Empty;
                values.TryGetValue("payer_id", out payer_id);
                string receiver_id = string.Empty;
                values.TryGetValue("receiver_id", out receiver_id);
                string invoice = string.Empty;
                values.TryGetValue("invoice", out invoice);
                string payment_fee = string.Empty;
                values.TryGetValue("payment_fee", out payment_fee);

                #endregion

                var sb = new StringBuilder();
                sb.AppendLine("GTPay IPN:");
                foreach (KeyValuePair<string, string> kvp in values)
                {
                    sb.AppendLine(kvp.Key + ": " + kvp.Value);
                }

                var newPaymentStatus = GTpayHelper.GetPaymentStatus(payment_status, pending_reason);
                sb.AppendLine("New payment status: " + newPaymentStatus);

                switch (txn_type)
                {
                    case "recurring_payment_profile_created":
                        //do nothing here
                        break;
                    case "recurring_payment":
                        #region Recurring payment
                        {
                            Guid orderNumberGuid = Guid.Empty;
                            try
                            {
                                orderNumberGuid = new Guid(rp_invoice_id);
                            }
                            catch
                            {
                            }

                            var initialOrder = _orderService.GetOrderByGuid(orderNumberGuid);
                            if (initialOrder != null)
                            {
                                var recurringPayments = _orderService.SearchRecurringPayments(initialOrderId: initialOrder.Id);
                                foreach (var rp in recurringPayments)
                                {
                                    switch (newPaymentStatus)
                                    {
                                        case PaymentStatus.Authorized:
                                        case PaymentStatus.Paid:
                                            {
                                                var recurringPaymentHistory = rp.RecurringPaymentHistory;
                                                if (recurringPaymentHistory.Count == 0)
                                                {
                                                    //first payment
                                                    var rph = new RecurringPaymentHistory
                                                    {
                                                        RecurringPaymentId = rp.Id,
                                                        OrderId = initialOrder.Id,
                                                        CreatedOnUtc = DateTime.UtcNow
                                                    };
                                                    rp.RecurringPaymentHistory.Add(rph);
                                                    _orderService.UpdateRecurringPayment(rp);
                                                }
                                                else
                                                {
                                                    //next payments
                                                    _orderProcessingService.ProcessNextRecurringPayment(rp);
                                                }
                                            }
                                            break;
                                    }
                                }

                                //this.OrderService.InsertOrderNote(newOrder.OrderId, sb.ToString(), DateTime.UtcNow);
                                _logger.Information("GTPAY IPN. Recurring info", new NopException(sb.ToString()));
                            }
                            else
                            {
                                _logger.Error("GTPAY IPN. Order is not found", new NopException(sb.ToString()));
                            }
                        }
                        #endregion
                        break;
                    default:
                        #region Standard payment
                        {
                            string orderNumber = string.Empty;
                            values.TryGetValue("custom", out orderNumber);
                            Guid orderNumberGuid = Guid.Empty;
                            try
                            {
                                orderNumberGuid = new Guid(orderNumber);
                            }
                            catch
                            {
                            }

                            var order = _orderService.GetOrderByGuid(orderNumberGuid);
                            if (order != null)
                            {

                                //order note
                                order.OrderNotes.Add(new OrderNote
                                {
                                    Note = sb.ToString(),
                                    DisplayToCustomer = false,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                                _orderService.UpdateOrder(order);

                                switch (newPaymentStatus)
                                {
                                    case PaymentStatus.Pending:
                                        {
                                        }
                                        break;
                                    case PaymentStatus.Authorized:
                                        {
                                            if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
                                            {
                                                _orderProcessingService.MarkAsAuthorized(order);
                                            }
                                        }
                                        break;
                                    case PaymentStatus.Paid:
                                        {
                                            if (_orderProcessingService.CanMarkOrderAsPaid(order))
                                            {

                                                order.AuthorizationTransactionId = txn_id;
                                                _orderService.UpdateOrder(order);

                                                _orderProcessingService.MarkOrderAsPaid(order);
                                            }
                                        }
                                        break;
                                    case PaymentStatus.Refunded:
                                        {
                                            var totalToRefund = Math.Abs(mc_gross);
                                            if (totalToRefund > 0 && Math.Round(totalToRefund, 2).Equals(Math.Round(order.OrderTotal, 2)))
                                            {
                                                //refund
                                                if (_orderProcessingService.CanRefundOffline(order))
                                                {
                                                    _orderProcessingService.RefundOffline(order);
                                                }
                                            }
                                            else
                                            {
                                                //partial refund
                                                if (_orderProcessingService.CanPartiallyRefundOffline(order, totalToRefund))
                                                {
                                                    _orderProcessingService.PartiallyRefundOffline(order, totalToRefund);
                                                }
                                            }
                                        }
                                        break;
                                    case PaymentStatus.Voided:
                                        {
                                            if (_orderProcessingService.CanVoidOffline(order))
                                            {
                                                _orderProcessingService.VoidOffline(order);
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                _logger.Error("GTPAY IPN. Order is not found", new NopException(sb.ToString()));
                            }
                        }
                        #endregion
                        break;
                }
            }
            else
            {
                _logger.Error("GTPAY IPN failed.", new NopException(strRequest));
            }

            //nothing should be rendered to visitor
            return Content("");
        }

        public ActionResult CancelOrder(FormCollection form)
        {
            if (_GTpayPaymentSettings.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage)
            {
                var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                    customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                    .FirstOrDefault();
                if (order != null)
                {
                    return RedirectToRoute("OrderDetails", new { orderId = order.Id });
                }
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}