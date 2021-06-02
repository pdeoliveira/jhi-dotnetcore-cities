using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JHipsterNet.Core.Pagination;
using JHipsterNet.Core.Pagination.Extensions;
using company.world.Domain;
using company.world.Domain.Repositories.Interfaces;
using company.world.Infrastructure.Data.Extensions;

namespace company.world.Infrastructure.Data.Repositories
{
    public class CityRepository : GenericRepository<City>, ICityRepository
    {
        public CityRepository(IUnitOfWork context) : base(context)
        {
        }

        public override async Task<City> CreateOrUpdateAsync(City city)
        {
            bool exists = await Exists(x => x.Id == city.Id);

            if (city.Id != 0 && exists)
            {
                Update(city);
            }
            else
            {
                _context.AddOrUpdateGraph(city);
            }
            return city;
        }
    }
}
