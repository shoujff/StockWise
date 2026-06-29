using StockWise.App.Models;
using StockWise.App.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public class WarehouseService : IWarehouseService
{
    private readonly WarehouseRepository _repo;

    public WarehouseService(WarehouseRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<WarehouseDto>> GetAllAsync()
    {
        var warehouses = await _repo.GetAllAsync();
        return warehouses.Select(ToDto);
    }

    public async Task<WarehouseDto?> GetByIdAsync(int id)
    {
        var warehouse = await _repo.GetByIdAsync(id);
        return warehouse is null ? null : ToDto(warehouse);
    }

    public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Название склада не может быть пустым");

        var warehouse = new Warehouse
        {
            Name = dto.Name.Trim(),
            Address = dto.Address?.Trim(),
            IsActive = true
        };

        var created = await _repo.AddAsync(warehouse);
        return ToDto(created);
    }

    public async Task<WarehouseDto> UpdateAsync(int id, UpdateWarehouseDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Название склада не может быть пустым");

        var warehouse = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Склад с Id={id} не найден");

        warehouse.Name = dto.Name.Trim();
        warehouse.Address = dto.Address?.Trim();
        warehouse.IsActive = dto.IsActive;

        await _repo.UpdateAsync(warehouse);
        return ToDto(warehouse);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var warehouse = await _repo.GetByIdAsync(id);
        if (warehouse is null)
            return false;

        await _repo.DeleteAsync(warehouse);
        return true;
    }

    public async Task<IEnumerable<WarehouseDto>> SearchAsync(string? searchTerm)
    {
        var all = await _repo.GetAllAsync();
        if (string.IsNullOrWhiteSpace(searchTerm))
            return all.Select(ToDto);

        var term = searchTerm.Trim().ToLower();
        return all.Where(w =>
            w.Name.ToLower().Contains(term) ||
            (w.Address?.ToLower().Contains(term) ?? false))
            .Select(ToDto);
    }

    private static WarehouseDto ToDto(Warehouse w) => new(
        w.Id, w.Name, w.Address, w.IsActive,
        w.StockBalances?.Count ?? 0);
}
