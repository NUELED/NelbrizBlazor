﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Nelbriz_Business.Repository.IRepository;
using Nelbriz_Common;
using Nelbriz_DataAccess;
using Nelbriz_DataAccess.Data;
using Nelbriz_DataAccess.ViewModel;
using Nelbriz_Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nelbriz_Business.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public OrderRepository(IMapper mapper, ApplicationDbContext db)
        {
            _db = db;
            _mapper = mapper;   
        }

        public async Task<OrderHeaderDTO> CancelOrder(int id)
        {
            var orderHeader = await _db.OrderHeaders.FindAsync(id);
            if (orderHeader == null)
            {
                return new OrderHeaderDTO();
            }
            if(orderHeader.Status == SD.Status_Pending)
            {
                orderHeader.Status = SD.Status_Cancelled;
                 _db.SaveChanges();   
            }
            if (orderHeader.Status == SD.Status_Confirmed)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund =  service.Create(options);

                orderHeader.Status = SD.Status_Refunded;
                 _db.SaveChanges();
            }

            return _mapper.Map<OrderHeaderDTO>(orderHeader);   
        }

        public async Task<OrderDTO> Create(OrderDTO objDTO)
        {
            try
            {
                var obj = _mapper.Map<OrderDTO, OrderDto>(objDTO);


                var addedObj = _db.OrderHeaders.Add(obj.OrderHeader);
                await _db.SaveChangesAsync();

                foreach(var details in obj.OrderDetails) 
                {
                    details.OrderHeaderId = obj.OrderHeader.Id;
                    
                }
                _db.OrderDetails.AddRange(obj.OrderDetails);
                await _db.SaveChangesAsync();

                return new OrderDTO()
                {
                    OrderHeader = _mapper.Map<OrderHeaderDTO>(obj.OrderHeader),
                    OrderDetails = _mapper.Map<IEnumerable<OrderDetailDTO>>(obj.OrderDetails).ToList(),

                };
            }
            catch (Exception ex)
            {

                throw ex;
            }

           
        }

        public async Task<int> Delete(int id)
        {
            var objHeader = await _db.OrderHeaders.FirstOrDefaultAsync( u => u.Id == id);   
            if(objHeader != null)
            {
               IEnumerable<OrderDetail> objDetail = _db.OrderDetails.Where(u => u.OrderHeaderId == id);

                _db.OrderDetails.RemoveRange(objDetail);
                _db.OrderHeaders.Remove(objHeader);
               return  await _db.SaveChangesAsync();   
            }

            return 0;
        }

        public async Task<OrderDTO> Get(int id)
        {
            OrderDto order = new()
            {
                OrderHeader = _db.OrderHeaders.FirstOrDefault( u => u.Id == id),    
                OrderDetails =_db.OrderDetails.Where(u=> u.OrderHeaderId == id).ToList()
            };
            if(order != null)
            {
              return  _mapper.Map<OrderDTO>(order);
            }
            return new OrderDTO();
        }

        public async Task<IEnumerable<OrderDTO>> GetAll(string? userId = null, string? status = null)
        {

            List<OrderDto> OrderFromDb = new List<OrderDto>();
            IEnumerable<OrderHeader> orderHeaderList = _db.OrderHeaders;
            IEnumerable<OrderDetail> orderDetailList = _db.OrderDetails;
            foreach(OrderHeader header in orderHeaderList)
            {
                OrderDto order = new()
                {
                    OrderHeader = header,
                    OrderDetails = orderDetailList.Where(u=> u.OrderHeaderId == header.Id)
                };
                OrderFromDb.Add(order);
            }
            //do some filtering #TODO

            return _mapper.Map<IEnumerable<OrderDTO>>(OrderFromDb);

            
        }

        public async Task<OrderHeaderDTO> MarkPaymentSuccessfull(int id, string paymentIntentId )
        {
             var data = await _db.OrderHeaders.FindAsync(id);   
             if(data != null)
             {
                return new OrderHeaderDTO();
             }
           if(data.Status == SD.Status_Pending) 
            {
                data.PaymentIntentId = paymentIntentId; 
                data.Status = SD.Status_Confirmed;
                await _db.SaveChangesAsync();
                return _mapper.Map<OrderHeaderDTO>(data);
            }

           return new OrderHeaderDTO();

        }

        public async Task<OrderHeaderDTO> UpdateHeader(OrderHeaderDTO objDTO)
        {
            if (objDTO != null)
            {
                var orderHeaderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.Id == objDTO.Id);

                if (orderHeaderFromDb != null)
                {
                    orderHeaderFromDb.Name = objDTO.Name;
                    orderHeaderFromDb.PhoneNumber = objDTO.PhoneNumber;
                    orderHeaderFromDb.Carrier = objDTO.Carrier;
                    orderHeaderFromDb.Tracking = objDTO.Tracking;
                    orderHeaderFromDb.StreetAddress = objDTO.StreetAddress;
                    orderHeaderFromDb.City = objDTO.City;
                    orderHeaderFromDb.State = objDTO.State;
                    orderHeaderFromDb.PostalCode = objDTO.PostalCode;
                    orderHeaderFromDb.Status = objDTO.Status;

                    try
                    {
                         _db.SaveChanges();
                        return _mapper.Map<OrderHeaderDTO>(orderHeaderFromDb);
                    }
                    catch (Exception ex)
                    {                     
                        throw; // or return null/error DTO
                    }
                }
            }
            return new OrderHeaderDTO();
        }


        public async Task<bool> UpdateOrderStatus(int orderId, string status)
        {
            var data = await _db.OrderHeaders.FindAsync(orderId);
            if (data != null)
            {
                return false;
            }
            data.Status = status;
            if (status == SD.Status_Shipped)
            {
                data.ShippingDate = DateTime.Now;   
            }
            await _db.SaveChangesAsync();

            return true;

        }
    }
}
