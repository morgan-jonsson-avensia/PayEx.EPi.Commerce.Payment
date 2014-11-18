﻿using Mediachase.Commerce.Orders.Dto;

namespace Epinova.PayExProvider.Contracts
{
    public interface IParameterReader
    {
        int GetVat(PaymentMethodDto paymentMethodDto);
        string GetDefaultView(PaymentMethodDto paymentMethodDto);
        string GetPriceArgsList(PaymentMethodDto paymentMethodDto);
    }
}
