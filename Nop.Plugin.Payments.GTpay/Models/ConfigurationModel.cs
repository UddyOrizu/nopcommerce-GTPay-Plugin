using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.GTpay.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.MerchantID")]
        public string MerchantID { get; set; }
        public bool MerchantID_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.HashCode")]
        public string HashCode { get; set; }
        public bool HashCode_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.Interswitch_Merchantid")]
        public string Interswitch_Merchantid { get; set; }
        public bool Interswitch_Merchantid_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.PostUrl")]
        public string PostUrl { get; set; }
        public bool PostUrl_OverrideForStore { get; set; }



        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.ReturnUrl")]
        public string ReturnUrl { get; set; }
        public bool ReturnUrl_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.CurrencyCode")]
        public string CurrencyCode { get; set; }
        public bool CurrencyCode_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.ShowGateWayFirst")]
        public string ShowGateWayFirst { get; set; }
        public bool ShowGateWayFirst_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.GateWayName")]
        public string GateWayName { get; set; }
        public bool GateWayName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.GTpayProductID")]
        public string GTpayProductID { get; set; }
        public bool GTpayProductID_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.PDTValidateOrderTotal")]
        public bool PdtValidateOrderTotal { get; set; }
        public bool PdtValidateOrderTotal_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.PassProductNamesAndTotals")]
        public bool PassProductNamesAndTotals { get; set; }
        public bool PassProductNamesAndTotals_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.EnableIpn")]
        public bool EnableIpn { get; set; }
        public bool EnableIpn_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.IpnUrl")]
        public string IpnUrl { get; set; }
        public bool IpnUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.AddressOverride")]
        public bool AddressOverride { get; set; }
        public bool AddressOverride_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GTpay.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage")]
        public bool ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage { get; set; }
        public bool ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore { get; set; }
    }
}