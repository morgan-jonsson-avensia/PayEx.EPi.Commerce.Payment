﻿
namespace EPiServer.Business.Commerce.Payment.PayEx.Contracts
{
    public interface IPayExSettings
    {
        long AccountNumber { get; }
        string EncryptionKey { get; }
        string DescriptionFormat { get; }
        string PayExCallbackIpAddress { get; }
        bool DisablePaymentMethodCreation { get; }
    }
}
