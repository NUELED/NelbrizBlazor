using Nelbriz_Models;
using NelbrizWeb_Client.Service.IService;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace NelbrizWeb_Client.Service
{
    public class PaymentService : IPaymentService
    {
       
        private readonly HttpClient _httpClient;
     
        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;        
        }


        public async Task<SuccessModelDTO> Checkout(StripePaymentDTO model)
        {

            try
            {
                var content = JsonConvert.SerializeObject(model);
                //You can decide to encrypt the content if you so wish, if it is not HTTPS!
                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/stripepayment/create", bodyContent);

                var responseResult = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<SuccessModelDTO>(responseResult);
                    return result;
                }
                else
                {
                    var errorModel = JsonConvert.DeserializeObject<ErrorModelDTO>(responseResult);
                    throw new Exception(errorModel.ErrorMessage);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
         
        }

  




    }
}
