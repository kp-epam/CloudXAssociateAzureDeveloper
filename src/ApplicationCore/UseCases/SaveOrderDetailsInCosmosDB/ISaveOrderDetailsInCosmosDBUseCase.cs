using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.UseCases.SaveOrderDetailsInCosmosDB;
public interface ISaveOrderDetailsInCosmosDBUseCase
{
    /// <summary>
    /// Send data to Azure Cosmos DB.
    /// </summary>
    /// <param name="azureFunctionUrl"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    Task Apply(string? azureFunctionUrl, Order order);
}
