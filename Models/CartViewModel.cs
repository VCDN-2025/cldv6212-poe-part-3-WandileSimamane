public class CartViewModel
{
    public string CartId { get; set; }
    public List<CartItemViewModel> Items { get; set; } = new();
}

public class CartItemViewModel
{
    public string CartItemId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public double? UnitPrice { get; set; }
}
