using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.UseCases.SaveOrderInCosmosDB;
public interface ISaveOrderInCosmosDBUseCase
{
    Task Apply(Order order);
}
