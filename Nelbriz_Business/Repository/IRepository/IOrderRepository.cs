using Nelbriz_DataAccess;
using Nelbriz_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nelbriz_Business.Repository.IRepository
{
    public interface IOrderRepository
    {
        public Task<OrderDTO> Get(int id);  
        public Task<IEnumerable<OrderDTO>> GetAll(string? userId = null, string? status = null);
        public Task<OrderDTO> Create(OrderDTO objDTO);
        public Task<int> Delete(int id);


        public Task<OrderHeaderDTO> UpdateHeader(OrderHeaderDTO objDTO);
        public Task<OrderHeaderDTO> MarkPaymentSuccessfull(int id, string paymentIntentId);
        public Task<bool> UpdateOrderStatus(int orderId, string status);
        public Task<OrderHeaderDTO> CancelOrder(int id);
    }
}
