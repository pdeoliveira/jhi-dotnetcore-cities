using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using company.world.Domain;

namespace company.world.Domain.Services.Interfaces
{
    public interface ICityService
    {
        Task<City> Save(City city);

        Task<IPage<City>> FindAll(IPageable pageable);

        Task<City> FindOne(long id);

        Task Delete(long id);
    }
}
