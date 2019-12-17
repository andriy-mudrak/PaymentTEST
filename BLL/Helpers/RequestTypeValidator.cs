﻿using BLL.Models;
using Microsoft.EntityFrameworkCore.Internal;

namespace BLL.Helpers
{
    public static class RequestTypeValidator
    {
        public static bool TypeValidation(PaymentModel model)
        {
            switch (model.Type)
            {
                case PaymentServiceConstants.CHARGE: return CreationPayment(model);
                case PaymentServiceConstants.AUTH: return CreationPayment(model);
                case PaymentServiceConstants.CAPTURE: return UpdationPayment(model);
                case PaymentServiceConstants.REFUND: return UpdationPayment(model);
                default: return false;
            }
        }

        public static PaymentServiceConstants.PaymentType PaymentChecker(string type)
        {
            switch (type)
            {
                case PaymentServiceConstants.AUTH: return PaymentServiceConstants.PaymentType.Auth;
                case PaymentServiceConstants.CHARGE: return PaymentServiceConstants.PaymentType.Charge;
                case PaymentServiceConstants.CAPTURE: return PaymentServiceConstants.PaymentType.Capture;
                case PaymentServiceConstants.REFUND: return PaymentServiceConstants.PaymentType.Refund;
                default: return PaymentServiceConstants.PaymentType.Default;
            }
        }

        private static bool CreationPayment(PaymentModel charge)
        {
            bool isChargeModel = true;
            isChargeModel = charge.Email.Any();
            isChargeModel = !charge.Amount.IsZero();
            isChargeModel = !(charge.UserId == null);
            isChargeModel = !charge.OrderId.IsZero();

            return isChargeModel;
        }

        private static bool UpdationPayment(PaymentModel charge)
        {
            bool isChargeModel = true;
            isChargeModel = !charge.OrderId.IsZero();

            return isChargeModel;
        }
    }
}