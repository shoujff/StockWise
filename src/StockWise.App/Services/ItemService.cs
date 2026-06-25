using StockWise.App.Models;
using StockWise.App.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace StockWise.App.Services
{
    public class ItemService : IItemService
    {
        private readonly ItemRepository _repo;
        public ItemService(ItemRepository repo) { _repo = repo; }

        public async Task<IEnumerable<ItemDto>> GetAllAsync()
        {
            var items = await _repo.GetAllAsync();
            return items.Select(ToDto);
        }


        public async Task<IEnumerable<ItemDto>> SearchAsync(string? searchTerm, int? categoryId)
        {
            var items = await _repo.SearchAsync(searchTerm, categoryId);
            return items.Select(ToDto);
        }


        public async Task<ItemDto> CreateAsync(CreateItemDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) {
                throw new ArgumentException("Название товара не может быть пустым");

            }

            var article = dto.Article?.Trim();
            if (string.IsNullOrWhiteSpace(article))
            {
                var last = await _repo.GetLastArticleAsync();
                article = GenerateNextArticle(last);
            } else if(!await IsArticleUniqueAsync(article))
            {
                throw new ArgumentException("Такой артикул уже используется");
            }
            var barcode = dto.Barcode?.Trim();
            if (string.IsNullOrWhiteSpace(barcode) && !await IsBarcodeUniqueAsync(barcode))
            {
                throw new ArgumentException("Такой штрихкод уже используется");
            }

            var item = new Item
            {
                Name = dto.Name.Trim(),
                Article = article,
                Unit = (dto.Unit ?? "").Trim(),
                MinStock = dto.MinStock,
                MaxStock = dto.MaxStock,
                Barcode = barcode,
                CategoryId = dto.CategoryId,
            };
            var created = await _repo.AddAsync(item);
            return ToDto(created);
        }
        public async Task<ItemDto> UpdateAsync(int id, UpdateItemDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Название товара не может быть пустым");
            if (string.IsNullOrWhiteSpace(dto.Article))
                throw new ArgumentException("Артикул не может быть пустым");

            var item = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Товар с Id={id} не найден");

            var article = dto.Article.Trim();
            if (!string.Equals(item.Article, article, StringComparison.OrdinalIgnoreCase)
                && !await IsArticleUniqueAsync(article))
                throw new InvalidOperationException($"Артикул '{article}' уже используется");

            var barcode = dto.Barcode?.Trim();
            if (!string.IsNullOrWhiteSpace(barcode))
            {
                var existing = await _repo.GetByBarcodeAsync(barcode);
                if (existing is not null && existing.Id != id)
                    throw new InvalidOperationException($"Штрихкод '{barcode}' уже используется");
            }

            item.Name = dto.Name.Trim();
            item.Article =article;
            item.Unit = (dto.Unit ?? "").Trim();
            item.MinStock = dto.MinStock;
            item.MaxStock = dto.MaxStock;
            item.IsBatch = dto.IsBatch;
            item.Barcode = barcode;
            item.CategoryId = dto.CategoryId;

            await _repo.UpdateAsync(item);
            return ToDto(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item is null)
                return false;

            await _repo.DeleteAsync(item);
            return true;
        }

        public async Task<ItemDto?> GetByIdAsync(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            return ToDto(item);
        }
        public async Task<ItemDto?> GetByArticleAsync(string article)
        {
            var item = await _repo.GetByArticleAsync(article?.Trim() ?? "");
            return item is null ? null : ToDto(item);
        }

        public async Task<ItemDto?> GetByBarcodeAsync(string barcode)
        {
            var item = await _repo.GetByBarcodeAsync(barcode?.Trim() ?? "");
            return item is null ? null : ToDto(item);
        }


        public async Task<bool> IsArticleUniqueAsync(string article, int? excludeId = null)
        {
            var existing = await _repo.GetByArticleAsync(article?.Trim() ?? "");
            return existing is null || existing.Id == excludeId;
        }
        public async Task<bool> IsBarcodeUniqueAsync(string barcode, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return true;
            var existing = await _repo.GetByBarcodeAsync(barcode.Trim());
            return existing is null || existing.Id == excludeId;
        }
        public async Task<string> GetNextArticleAsync()
        {
            var last = await _repo.GetLastArticleAsync();
            return GenerateNextArticle(last);
        }

        private static string GenerateNextArticle(string? lastArticle)
        {
            var number = 1;
            if (!string.IsNullOrWhiteSpace(lastArticle) && lastArticle.StartsWith("ART-"))
            {
                _ = int.TryParse(lastArticle.AsSpan(4), out number);
                number++;
            }
            return $"ART-{number:D5}";
        }
        

   
      private static ItemDto ToDto(Item item) => new( item.Id,item.Name,  item.Article, item.Unit, item.MinStock,item.MaxStock,item.IsBatch, item.Barcode,item.ImagePath,item.CategoryId,item.Category?.Name ?? "");
  }
}
