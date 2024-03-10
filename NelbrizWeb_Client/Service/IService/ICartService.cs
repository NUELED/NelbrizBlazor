using NelbrizWeb_Client.ViewModels;

namespace NelbrizWeb_Client.Service.IService
{
    public interface ICartService
    {
        public event Action Onchange;
        Task DecrementCart(ShoppingCart shoppingCart);
        Task IncrementCart(ShoppingCart shoppingCart);
    }
}
