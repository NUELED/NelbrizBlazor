﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Nelbriz_Business.Repository.IRepository;
using Nelbriz_DataAccess;
using Nelbriz_DataAccess.Data;
using Nelbriz_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nelbriz_Business.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly  ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public ProductRepository(ApplicationDbContext db, IMapper mapper)
        {
           _db = db;
           _mapper = mapper;    
        }

        public async Task<ProductDTO> Create(ProductDTO objDTO)
        {
            var obj = _mapper.Map<ProductDTO, Product>(objDTO);
   

            var addedObj = _db.Products.Add(obj);
                    await    _db.SaveChangesAsync();
            var objReturned = _mapper.Map<Product, ProductDTO>(addedObj.Entity);

            return objReturned;
        }


        public async Task<int> Delete(int id)
        {
           var obj = await _db.Products.FirstOrDefaultAsync(c => c.Id == id);
            if (obj != null)
            {
                _db.Products.Remove(obj); 
                return await _db.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<ProductDTO> Get(int id)
        {
            var obj = await _db.Products.Include(u=>u.Category).Include(u => u.ProductPrices).FirstOrDefaultAsync(c => c.Id == id);
            //if (obj != null)
            //{
            //   return _mapper.Map<Product, ProductDTO>(obj);
            //}
            if (obj != null)
            {
                return _mapper.Map<ProductDTO>(obj);
            }
            return new ProductDTO();
        }

        public async Task<IEnumerable<ProductDTO>> GetAll()
        {      
            var obj = await _db.Products.Include(u => u.Category).Include(u=> u.ProductPrices).ToListAsync();    
            //  return _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(_db.Products.Include(u => u.Category));             
            return _mapper.Map<IEnumerable<ProductDTO>>(obj);          
        }

        public async Task<ProductDTO> Update(ProductDTO objDTO)
        {
            var objFromDb = await _db.Products.FirstOrDefaultAsync(c => c.Id == objDTO.Id);
            if (objFromDb != null) 
            {
                 objFromDb.Name = objDTO.Name;
                 objFromDb.Description = objDTO.Description;
                 objFromDb.ImageUrl = objDTO.ImageUrl;
                 objFromDb.CategoryId = objDTO.CategoryId;
                 objFromDb.Color = objDTO.Color;
                 objFromDb.ShopFavourites = objDTO.ShopFavourites;
                 objFromDb.CustomerFavourites = objDTO.CustomerFavourites;
                _db.Products.Update(objFromDb);
                await  _db.SaveChangesAsync();
                return _mapper.Map<Product, ProductDTO>(objFromDb);

            }

            return objDTO;           
        }
    }
}
