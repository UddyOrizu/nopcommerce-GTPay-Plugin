using Nop.Core.Domain.Payments;

namespace Nop.Plugin.Payments.GTpay
{
    /// <summary>
    /// Represents paypal helper
    /// </summary>
    public class GTpayHelper
    {
        /// <summary>
        /// Gets a payment status
        /// </summary>
        /// <param name="paymentStatus">GTpay payment status</param>
        /// <param name="pendingReason">GTpay pending reason</param>
        /// <returns>Payment status</returns>
        public static PaymentStatus GetPaymentStatus(string paymentStatus, string pendingReason)
        {
            var result = PaymentStatus.Pending;

            if (paymentStatus == null)
                paymentStatus = string.Empty;

            if (pendingReason == null)
                pendingReason = string.Empty;

            switch (paymentStatus.ToLowerInvariant())
            {
                case "00":
                    result = PaymentStatus.Paid;
                    break;
                case "denied":
                case "expired":
                case "failed":
                case "voided":
                    result = PaymentStatus.Voided;
                    break;
                case "refunded":
                case "reversed":
                    result = PaymentStatus.Refunded;
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}

