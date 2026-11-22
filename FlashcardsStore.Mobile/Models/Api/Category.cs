namespace FlashcardsStore.Mobile.Models.Api;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public int ProductCount { get; set; }
}
