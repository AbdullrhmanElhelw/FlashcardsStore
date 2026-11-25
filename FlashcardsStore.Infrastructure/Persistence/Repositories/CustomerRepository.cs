using Microsoft.EntityFrameworkCore;
using FlashcardsStore.Application.DTOs.Admin;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Domain.Enums;
using FlashcardsStore.Infrastructure.Persistence;

namespace FlashcardsStore.Infrastructure.Persistence.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(FlashcardsStoreDbContext context) : base(context)
    {
    }

    public Task<Customer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
        => _dbSet.Where(customer => customer.PhoneNumber == phone)
            .OrderByDescending(customer => customer.PasswordHash != null)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Customer?> GetAccountByPhoneAsync(string phone, CancellationToken cancellationToken = default)
        => _dbSet.FirstOrDefaultAsync(
            customer => customer.PhoneNumber == phone && customer.PasswordHash != null,
            cancellationToken);

    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLower();
        return _dbSet.FirstOrDefaultAsync(
            customer => customer.Email != null && customer.Email.ToLower() == normalizedEmail,
            cancellationToken);
    }

    public async Task<AdminCustomersSummaryDto> GetAdminCustomersSummaryAsync(
        string? search,
        string? filterType,
        CancellationToken cancellationToken = default)
    {
        var normalizedFilter = string.IsNullOrWhiteSpace(filterType) ? "all" : filterType.Trim().ToLowerInvariant();
        var totalCustomers = await _dbSet.CountAsync(cancellationToken);
        var mobileAccounts = await _dbSet.CountAsync(c => c.PasswordHash != null, cancellationToken);

        IQueryable<Customer> query = _dbSet.AsNoTracking();

        if (normalizedFilter == "accounts")
        {
            query = query.Where(c => c.PasswordHash != null);
        }
        else if (normalizedFilter == "guests")
        {
            query = query.Where(c => c.PasswordHash == null);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(c =>
                c.FullName.ToLower().Contains(term) ||
                c.PhoneNumber.ToLower().Contains(term) ||
                (c.Email != null && c.Email.ToLower().Contains(term)) ||
                c.Governorate.ToLower().Contains(term) ||
                c.City.ToLower().Contains(term));
        }

        var customers = await query
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new AdminCustomerListItemDto
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                WhatsAppNumber = c.WhatsAppNumber,
                Governorate = c.Governorate,
                City = c.City,
                CustomerType = c.CustomerType,
                HasMobileAccount = c.PasswordHash != null,
                OrdersCount = c.Orders.Count(o => !o.IsDeleted),
                TotalSpent = c.Orders
                    .Where(o => !o.IsDeleted && o.Status != OrderStatus.Rejected && o.Status != OrderStatus.Cancelled)
                    .Sum(o => (decimal?)o.TotalAmount) ?? 0m,
                LastOrderDate = c.Orders
                    .Where(o => !o.IsDeleted)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(o => (DateTime?)o.CreatedAt)
                    .FirstOrDefault(),
                CreatedAt = c.CreatedAt
            })
            .Take(200)
            .ToListAsync(cancellationToken);

        return new AdminCustomersSummaryDto
        {
            Customers = customers,
            TotalCustomers = totalCustomers,
            MobileAccountCustomers = mobileAccounts,
            GuestCustomers = totalCustomers - mobileAccounts,
            SearchTerm = search,
            FilterType = normalizedFilter
        };
    }
}
