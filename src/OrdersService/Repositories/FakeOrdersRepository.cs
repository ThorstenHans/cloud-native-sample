using OrdersService.Entities;

namespace OrdersService.Repositories;

public class FakeOrdersRepository : IOrdersRepository
{
    private readonly List<Order> _orders = new()
    {
        new Order
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Empty.ToString(),
            UserName = "John Doe",
            SubmittedAt = DateTime.Now.AddDays(-1),
            Positions = new List<OrderPosition>
            {
                new()
                {
                    ProductId = Guid.Parse("b3b749d1-fd02-4b47-8e3c-540555439db6"),
                    Quantity = 4
                },
                new(){
                    ProductId = Guid.Parse("bbbbbbbb-fd02-4b47-8e3c-540555439db6"),
                    Quantity = 3
                },
            }
        },
        new Order
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Empty.ToString(),
            UserName = "Jane Doe",
            SubmittedAt = DateTime.Now.AddDays(-2),
            Positions = new List<OrderPosition>
            {
                new(){
                    ProductId = Guid.Parse("aaaaaaaa-fd02-4b47-8e3c-540555439db6"),
                    Quantity = 10
                },
                new(){
                    ProductId = Guid.Parse("bbbbbbbb-fd02-4b47-8e3c-540555439db6"),
                    Quantity = 5
                },
                new(){
                    ProductId = Guid.Parse("b3b749d1-fd02-4b47-8e3c-540555439db6"),
                    Quantity = 2
                }
            }
        }
    };

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await Task.Run(() => _orders);
    }
}