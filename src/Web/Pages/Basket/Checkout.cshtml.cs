﻿using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.UseCases.PushOrderItemsToServiceBus;
using Microsoft.eShopWeb.ApplicationCore.UseCases.SaveOrderDetailsInCosmosDB;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.eShopWeb.Web.Interfaces;

namespace Microsoft.eShopWeb.Web.Pages.Basket;

[Authorize]
public class CheckoutModel : PageModel
{
    private readonly IBasketService _basketService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOrderService _orderService;
    private readonly IBasketViewModelService _basketViewModelService;
    private readonly IAppLogger<CheckoutModel> _logger;
    private readonly ISaveOrderDetailsInCosmosDBUseCase _saveOrderDetailsInCosmosDB;
    private readonly IPushOrderItemsToServiceBusUseCase _pushOrderItemsToServiceBus;
    private readonly IConfiguration _configuration;
    private string? _username = null;

    public CheckoutModel(IBasketService basketService,
        IBasketViewModelService basketViewModelService,
        SignInManager<ApplicationUser> signInManager,
        IOrderService orderService,
        IAppLogger<CheckoutModel> logger,
        ISaveOrderDetailsInCosmosDBUseCase saveOrderDetailsInCosmosDB,
        IPushOrderItemsToServiceBusUseCase pushOrderItemsToServiceBus,
        IConfiguration configuration)
    {
        _basketService = basketService;
        _signInManager = signInManager;
        _orderService = orderService;
        _basketViewModelService = basketViewModelService;
        _logger = logger;
        _saveOrderDetailsInCosmosDB = saveOrderDetailsInCosmosDB;
        _pushOrderItemsToServiceBus = pushOrderItemsToServiceBus;
        _configuration = configuration;
    }

    public BasketViewModel BasketModel { get; set; } = new BasketViewModel();

    public async Task OnGet()
    {
        await SetBasketModelAsync();
    }

    public async Task<IActionResult> OnPost(IEnumerable<BasketItemViewModel> items)
    {
        try
        {
            await SetBasketModelAsync();

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var quantities = items.ToDictionary(b => b.Id, b => b.Quantity);
            await _basketService.SetQuantities(BasketModel.Id, quantities);
            var createdOrder = await _orderService
                .CreateAndGetOrderAsync(BasketModel.Id, new Address("123 Main St.", "Kent", "OH", "United States", "44240"));
            await _basketService.DeleteBasketAsync(BasketModel.Id);

            var deliveryOrderProcessorFuncUrl = _configuration.GetValue("DeliveryOrderProcessorFuncUrl", string.Empty);
            await _saveOrderDetailsInCosmosDB.Apply(deliveryOrderProcessorFuncUrl, createdOrder);

            var serviceBusConnString = _configuration.GetValue("ServiceBusConnectionString", string.Empty);
            var serviceBusQueueName = _configuration.GetValue("ServiceBusQueueName", string.Empty);
            await _pushOrderItemsToServiceBus.Apply(serviceBusConnString, serviceBusQueueName, quantities);
        }
        catch (EmptyBasketOnCheckoutException emptyBasketOnCheckoutException)
        {
            //Redirect to Empty Basket page
            _logger.LogWarning(emptyBasketOnCheckoutException.Message);
            return RedirectToPage("/Basket/Index");
        }

        return RedirectToPage("Success");
    }

    private async Task SetBasketModelAsync()
    {
        Guard.Against.Null(User?.Identity?.Name, nameof(User.Identity.Name));
        if (_signInManager.IsSignedIn(HttpContext.User))
        {
            BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(User.Identity.Name);
        }
        else
        {
            GetOrSetBasketCookieAndUserName();
            BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(_username!);
        }
    }

    private void GetOrSetBasketCookieAndUserName()
    {
        if (Request.Cookies.ContainsKey(Constants.BASKET_COOKIENAME))
        {
            _username = Request.Cookies[Constants.BASKET_COOKIENAME];
        }
        if (_username != null) return;

        _username = Guid.NewGuid().ToString();
        var cookieOptions = new CookieOptions();
        cookieOptions.Expires = DateTime.Today.AddYears(10);
        Response.Cookies.Append(Constants.BASKET_COOKIENAME, _username, cookieOptions);
    }
}
