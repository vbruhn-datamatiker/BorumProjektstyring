using ClassLib.Interface;
using ClassLib.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLib.Repo
{
    public class CustomerRepo : IRepo<Customer>
    {
        private BorumBygDBContext _context;

        public CustomerRepo ()
        {
            _context = new BorumBygDBContext ();
        }

        public Customer Add(Customer entity)
        {
            // Calls validation before customer is created
            entity.Validate();
            _context.Customers.Add(entity);
            _context.SaveChanges();
            return entity;

        }

        public Customer Delete(int id)
        {
            var customer = _context.Customers.Find(id);
            _context.Customers.Remove(customer);
            _context.SaveChanges();
            return customer;
        }

        public List<Customer> GetAll()
        {
            return _context.Customers.ToList();
        }

        public Customer? GetById(int id)
        {
            return _context.Customers.Find(id);
        }

        public Customer Update(Customer entity, int id)
        {
            var customer = GetById(id);
            if (customer != null)
            {
                _context.Entry(customer).CurrentValues.SetValues(entity);
                _context.SaveChanges();
            }
            return entity;
        }



    }
}
