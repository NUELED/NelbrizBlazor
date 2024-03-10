using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Nelbriz_Business.Repository.IRepository;
using Nelbriz_Common;
using Nelbriz_DataAccess;
using Nelbriz_DataAccess.Data;
using Nelbriz_DataAccess.ViewModel;
using Nelbriz_Models;
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


        public async Task<OrderDTO> Create(OrderDTO objDTO)
        {
            try
            {
                var obj = _mapper.Map<OrderDTO, Order>(objDTO);


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
            Order order = new()
            {
                OrderHeader = _db.OrderHeaders.FirstOrDefault( u => u.Id == id),    
                OrderDetails =_db.OrderDetails.Where(u=> u.OrderHeaderId == id)
            };
            if(order != null)
            {
                _mapper.Map<OrderDTO>(order);
            }
            return new OrderDTO();
        }

        public async Task<IEnumerable<OrderDTO>> GetAll(string? userId = null, string? status = null)
        {

            List<Order> OrderFromDb = new List<Order>();
            IEnumerable<OrderHeader> orderHeaderList = _db.OrderHeaders;
            IEnumerable<OrderDetail> orderDetailList = _db.OrderDetails;
            foreach(OrderHeader header in orderHeaderList)
            {
                Order order = new()
                {
                    OrderHeader = header,
                    OrderDetails = orderDetailList.Where(u=> u.OrderHeaderId == header.Id)
                };
                OrderFromDb.Add(order);
            }
            //do some filtering #TODO

            return _mapper.Map<IEnumerable<OrderDTO>>(OrderFromDb);

            
        }

        public async Task<OrderHeaderDTO> MarkPaymentSuccessfull(int id)
        {
             var data = await _db.OrderHeaders.FindAsync(id);   
             if(data != null)
             {
                return new OrderHeaderDTO();
             }
           if(data.Status == SD.Status_Pending) 
            {
                data.Status = SD.Status_Confirmed;
                await _db.SaveChangesAsync();
                return _mapper.Map<OrderHeaderDTO>(data);
            }

           return new OrderHeaderDTO();

        }

        public async Task<OrderHeaderDTO> UpdateHeader(OrderHeaderDTO objDTO)
        {
            if(objDTO != null)
            {
                var OrderHeader = _mapper.Map<OrderHeader>(objDTO);
                 _db.OrderHeaders.Update(OrderHeader);    
                await _db.SaveChangesAsync();
                return _mapper.Map<OrderHeaderDTO>(OrderHeader);
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
