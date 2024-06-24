using Microsoft.EntityFrameworkCore;
using OrderManagement.Models;
using OrderManagement.Repositories;

namespace OrderManagement.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DeleteCardholderDataAsync(int id)
        {
            var data = await _context.CardholderData.FindAsync(id);
            if (data != null)
            {
                _context.CardholderData.Remove(data);
                await _context.SaveChangesAsync();
            }
        }

        //create a method to add card holder
        public async Task AddCardholderDataAsync(Cardholder cardholder)
        {
            await _context.CardholderData.AddAsync(cardholder);
            await _context.SaveChangesAsync();
        }
    }
}
