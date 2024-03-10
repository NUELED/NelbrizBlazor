﻿using Nelbriz_Models;

namespace NelbrizWeb_Client.Service.IService
{
    public interface IOrderService
    {
        public Task<IEnumerable<OrderDTO>> GetAll(string? userId);
        public Task<OrderDTO> Get(string? orderId);
    }
}
