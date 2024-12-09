using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.UseCases.SaveOrderDetailsInCosmosDB;
public interface ISaveOrderDetailsInCosmosDBUseCase
{
    Task Apply(Order order);
}
