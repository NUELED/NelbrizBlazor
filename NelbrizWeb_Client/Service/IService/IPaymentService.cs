using Nelbriz_Models;
using NelbrizWeb_Client.ViewModels;

namespace NelbrizWeb_Client.Service.IService
{
    public interface IPaymentService
    {
      
        public Task<SuccessModelDTO>  Checkout(StripePaymentDTO model);

    }
}
