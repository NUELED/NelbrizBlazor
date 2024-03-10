using Blazored.LocalStorage;
using Nelbriz_Common;
using NelbrizWeb_Client.Service.IService;
using NelbrizWeb_Client.ViewModels;

namespace NelbrizWeb_Client.Service
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        public event Action Onchange;
        public CartService( ILocalStorageService localStorageService)
        {
            _localStorage = localStorageService;
        }


        public async Task DecrementCart(ShoppingCart cartToDecrement)
        {
            var cart = await _localStorage.GetItemAsync<List<ShoppingCart>>(SD.ShoppingCart);

            for(int i=0; i<cart.Count; i++)
            {
                if (cart[i].ProductId == cartToDecrement.ProductId && cart[i].ProductPriceId == cartToDecrement.ProductPriceId)
                {

                    //if (cart[i].Count <= 1)
                    //{
                    //	cart.RemoveAt(i);
                    //	i--; // Decrement loop index to account for removed item
                    //}
                    if (cart[i].Count == 1 || cartToDecrement.Count == 0)
                    {
                        cart.Remove(cart[i]);
                    }
                    else
                    {
                        cart[i].Count -= cartToDecrement.Count;
                    }
                }
            }
            await _localStorage.SetItemAsync(SD.ShoppingCart, cart);
            Onchange.Invoke();
        }



        public async Task IncrementCart(ShoppingCart cartToAdd)
        {
            var cart = await _localStorage.GetItemAsync<List<ShoppingCart>>(SD.ShoppingCart);
            bool itemInCart = false;   
            
            if(cart == null) 
            {
                cart = new List<ShoppingCart>();
            }
            foreach (var item in cart)
            {
                if(item.ProductId == cartToAdd.ProductId && item.ProductPriceId == cartToAdd.ProductPriceId)
                {
                    itemInCart = true;
                    item.Count += cartToAdd.Count;
                }
            }
            if(!itemInCart)
            { 
                cart.Add(new ShoppingCart()
                {
                    ProductId = cartToAdd.ProductId,    
                    ProductPriceId = cartToAdd.ProductPriceId,  
                    Count = cartToAdd.Count 
                });
            }
            await _localStorage.SetItemAsync(SD.ShoppingCart, cart);
            Onchange.Invoke();
        }

    }
}
