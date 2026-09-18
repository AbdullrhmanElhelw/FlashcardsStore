using System.Text.Json;
using FlashcardsStore.Application.DTOs.Cart;
using Microsoft.AspNetCore.Http;

namespace FlashcardsStore.Web.Common;

public class CartSessionManager
{
    private const string CartSessionKey = "FlashcardsStore_CartItems";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartSessionManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession? Session => _httpContextAccessor.HttpContext?.Session;

    public List<CartItemDto> GetCartItems()
    {
        if (Session == null) return new List<CartItemDto>();

        var json = Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(json)) return new List<CartItemDto>();

        try
        {
            return JsonSerializer.Deserialize<List<CartItemDto>>(json) ?? new List<CartItemDto>();
        }
        catch
        {
            return new List<CartItemDto>();
        }
    }

    public void SaveCartItems(List<CartItemDto> items)
    {
        if (Session == null) return;
        var json = JsonSerializer.Serialize(items);
        Session.SetString(CartSessionKey, json);
    }

    public void AddItem(CartItemDto item)
    {
        var items = GetCartItems();
        var existing = items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existing != null)
        {
            existing.Quantity += item.Quantity;
        }
        else
        {
            items.Add(item);
        }
        SaveCartItems(items);
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var items = GetCartItems();
        var existing = items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            if (quantity <= 0)
            {
                items.Remove(existing);
            }
            else
            {
                existing.Quantity = quantity;
            }
            SaveCartItems(items);
        }
    }

    public void RemoveItem(int productId)
    {
        var items = GetCartItems();
        items.RemoveAll(i => i.ProductId == productId);
        SaveCartItems(items);
    }

    public void ClearCart()
    {
        Session?.Remove(CartSessionKey);
    }

    public CartViewModel GetCartViewModel()
    {
        var items = GetCartItems();
        return new CartViewModel
        {
            Items = items,
            ShippingFee = 50.00m
        };
    }
}
