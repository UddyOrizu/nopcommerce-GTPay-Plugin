using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.GTpay
{
    public class GTpayPaymentSettings : ISettings
    {
        public bool UseSandbox { get; set; }
        public string MerchantID { get; set; }
        public string Interswitch_Merchantid { get; set; }
        public string PostUrl { get; set; }
        public string ReturnUrl { get; set; }
        public string CurrencyCode { get; set; }
        public string ShowGateWayFirst { get; set; }
        public string GateWayName { get; set; }        
        public string GTpayProductID { get; set; }

        public string HashCode { get; set; }

       //public string PdtToken { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }
        /// <summary>
        /// Additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }
        public bool PassProductNamesAndTotals { get; set; }
        public bool PdtValidateOrderTotal { get; set; }
        public bool EnableIpn { get; set; }

        public string IpnUrl { get; set; }
        /// <summary>
        /// Enable if a customer should be redirected to the order details page
        /// when he clicks "return to store" link on PayPal site
        /// WITHOUT completing a payment
        /// </summary>
        public bool ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage { get; set; }
        /// <summary>
        /// Enable PayPal address override
        /// </summary>
        public bool AddressOverride { get; set; }
    }

    public class InterswitchResponse
    {

        public string Amount { get; set; }
        public string ResponseDescription { get; set; }
        public string MerchantReference { get; set; }
        public string ResponseCode { get; set; }
        public string MertID { get; set; }
        //public string CardNumber { get; set; }

    }
}
