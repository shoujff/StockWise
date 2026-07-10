using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockWise.App.Models;
using StockWise.App.Repositories;

namespace StockWise.App.Services;

public class CustomerService : ICustomerService
{
    private readonly CustomerRepository _repo;

    public CustomerService(CustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var customers = await _repo.GetAllAsync();
        return customers.Select(ToDto);
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _repo.GetByIdAsync(id);
        return customer is null ? null : ToDto(customer);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Название не может быть пустым");

        var customer = new Customer
        {
            Name = dto.Name.Trim(),
            INN = dto.INN?.Trim(),
            ContactPerson = dto.ContactPerson?.Trim(),
            Phone = dto.Phone?.Trim(),
            Email = dto.Email?.Trim()
        };

        var created = await _repo.AddAsync(customer);
        return ToDto(created);
    }

    public async Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Название не может быть пустым");

        var customer = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Контрагент с Id={id} не найден");

        customer.Name = dto.Name.Trim();
        customer.INN = dto.INN?.Trim();
        customer.ContactPerson = dto.ContactPerson?.Trim();
        customer.Phone = dto.Phone?.Trim();
        customer.Email = dto.Email?.Trim();

        await _repo.UpdateAsync(customer);
        return ToDto(customer);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _repo.GetByIdAsync(id);
        if (customer is null)
            return false;

        await _repo.DeleteAsync(customer);
        return true;
    }

    public async Task<IEnumerable<CustomerDto>> SearchAsync(string? searchTerm)
    {
        var all = await _repo.GetAllAsync();
        if (string.IsNullOrWhiteSpace(searchTerm))
            return all.Select(ToDto);

        var term = searchTerm.Trim().ToLower();
        return all.Where(c =>
            c.Name.ToLower().Contains(term) ||
            (c.INN?.ToLower().Contains(term) ?? false) ||
            (c.ContactPerson?.ToLower().Contains(term) ?? false) ||
            (c.Phone?.Contains(term) ?? false) ||
            (c.Email?.ToLower().Contains(term) ?? false))
            .Select(ToDto);
    }

    private static CustomerDto ToDto(Customer c) => new(
        c.Id, c.Name, c.INN, c.ContactPerson, c.Phone, c.Email);
}
