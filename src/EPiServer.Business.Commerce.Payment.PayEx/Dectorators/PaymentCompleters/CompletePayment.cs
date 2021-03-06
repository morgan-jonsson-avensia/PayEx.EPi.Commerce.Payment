﻿using EPiServer.Business.Commerce.Payment.PayEx.Contracts;
using EPiServer.Business.Commerce.Payment.PayEx.Contracts.Commerce;
using EPiServer.Business.Commerce.Payment.PayEx.Models;
using EPiServer.Business.Commerce.Payment.PayEx.Models.Result;
using log4net;
using PaymentMethod = EPiServer.Business.Commerce.Payment.PayEx.Models.PaymentMethods.PaymentMethod;

namespace EPiServer.Business.Commerce.Payment.PayEx.Dectorators.PaymentCompleters
{
    internal class CompletePayment : IPaymentCompleter
    {
        private IPaymentCompleter _paymentCompleter;
        private readonly IPaymentManager _paymentManager;
        private readonly IPaymentActions _paymentActions;
        protected readonly ILog Log = LogManager.GetLogger(Constants.Logging.DefaultLoggerName);

        public CompletePayment(IPaymentCompleter paymentCompleter, IPaymentManager paymentManager, IPaymentActions paymentActions)
        {
            _paymentCompleter = paymentCompleter;
            _paymentManager = paymentManager;
            _paymentActions = paymentActions;
        }

        public PaymentCompleteResult Complete(PaymentMethod currentPayment, string orderRef)
        {
            Log.InfoFormat("Completing payment with ID:{0} belonging to order with ID: {1}", currentPayment.Payment.Id, currentPayment.OrderGroupId);
            CompleteResult completeResult = _paymentManager.Complete(orderRef);
            if (!completeResult.Success || string.IsNullOrWhiteSpace(completeResult.TransactionNumber))
                return new PaymentCompleteResult { TransactionErrorCode = completeResult.ErrorDetails != null ? completeResult.ErrorDetails.TransactionErrorCode : string.Empty };

            if (completeResult.GetTransactionDetails)
            {
                Log.InfoFormat("Retrieving transaction details for payment with ID:{0} belonging to order with ID: {1}", currentPayment.Payment.Id, currentPayment.OrderGroupId);
                if (_paymentCompleter == null)
                    _paymentCompleter = new UpdateTransactionDetails(null, _paymentManager);
                _paymentCompleter = new UpdateTransactionDetails(_paymentCompleter, _paymentManager);
            }

            _paymentActions.UpdatePaymentInformation(currentPayment, completeResult.TransactionNumber, completeResult.PaymentMethod);

            PaymentCompleteResult result = new PaymentCompleteResult { Success = true };
            if (_paymentCompleter != null)
                result = _paymentCompleter.Complete(currentPayment, orderRef);

            Log.InfoFormat("Successfully completed payment with ID:{0} belonging to order with ID: {1}", currentPayment.Payment.Id, currentPayment.OrderGroupId);
            return result;
        }
    }
}
