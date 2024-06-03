using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace MessagingPattern.Api.Controllers;

/*
 * Customers can post a list of items to add to their shopping cart.
 * 
 * Operation: CreateCart
 * Operands: 
 *  - Customer - Authorization -
 *  - The Request - what do they want to add? (CreateCartRequest)
 *      - We need to validate this stuff.
 *      - Validate the "form" of the message, but also the semantics of it.
 *          - You can't add products we don't carry to your cart..
 */
public class ShoppingCartController(

    IMessageBus bus,
    FakeShippingApi api, FakeDataContext context) : ControllerBase
{
    [HttpPost("/cart")]
    public async Task<ActionResult> CreateCartAsync(
        [FromBody] CreateCartRequest request,
        CancellationToken token)
    {

        // Blocking Call ("Synchronous")
        // foreach(var item in request) { go ask the product api if it has that stuff..., oh and give me the price)
        var total = await api.GetTotalForCartItemsAsync(request.Items, token);
        var response = new CartResponse { Total = total };

        // if the customer is a "Gold Customer", send an email to their sales rep for follow up.

        // send the customer a confirmation email 
        // Save this to the database (MY database). 
        await context.SaveChangesAsync(token);

        //await email.SendEmailTo("customer@email.com", "thanks for your order", token);
        // Publish is for "Events"

        // Send is for "Commands"
        await bus.SendAsync(new SendEmail("customer@email.com", "thanks for the order"));
        if (total > 40M)
        {
            await bus.ScheduleAsync(new SendEmail("salesrep@company.com", "customer ordered stuff"), TimeSpan.FromHours(3));
        }

        return Ok(response);
    }
}

public record CreateCartRequest
{
    public IList<CartItem> Items { get; set; } = [];
};

public record CartItem(string Sku, int Qty);


public record CartResponse
{
    public decimal Total { get; set; }
}



public class FakeShippingApi
{
    public async Task<decimal> GetTotalForCartItemsAsync(IList<CartItem> items, CancellationToken token)
    {
        await Task.Delay(items.Count * 500);
        return 42.23M;
    }
}

public class FakeDataContext
{
    public async Task SaveChangesAsync(CancellationToken token)
    {
        await Task.Delay(300);
    }
}

public class EmailHandler(ILogger<EmailHandler> logger)
{
    public async Task Handle(SendEmail command)
    {
        await Task.Delay(3000); // WAYY too long.
        logger.LogInformation("Sending an email to {email} with {message}", command.EmailAddres, command.Message);
    }
}

public record SendEmail(string EmailAddres, string Message);
