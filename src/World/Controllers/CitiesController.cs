
using System.Collections.Generic;
using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using company.world.Domain;
using company.world.Crosscutting.Exceptions;
using company.world.Domain.Repositories.Interfaces;
using company.world.Web.Extensions;
using company.world.Web.Filters;
using company.world.Web.Rest.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using company.world.Web.Rest.Utilities.PrimeNG.LazyLoading;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;

namespace company.world.Controllers
{
    [Authorize]
    [Route("api/cities")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private const string EntityName = "city";
        private readonly ICityRepository _cityRepository;
        private readonly ILogger<CitiesController> _log;

        public CitiesController(ILogger<CitiesController> log,
            ICityRepository cityRepository)
        {
            _log = log;
            _cityRepository = cityRepository;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<ActionResult<City>> CreateCity([FromBody] City city)
        {
            _log.LogDebug($"REST request to save City : {city}");
            if (city.Id != 0)
                throw new BadRequestAlertException("A new city cannot already have an ID", EntityName, "idexists");

            await _cityRepository.CreateOrUpdateAsync(city);
            await _cityRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCity), new { id = city.Id }, city)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, city.Id.ToString()));
        }

        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateCity(long id, [FromBody] City city)
        {
            _log.LogDebug($"REST request to update City : {city}");
            if (city.Id == 0) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            if (id != city.Id) throw new BadRequestAlertException("Invalid Id", EntityName, "idinvalid");
            await _cityRepository.CreateOrUpdateAsync(city);
            await _cityRepository.SaveChangesAsync();
            return Ok(city)
                .WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, city.Id.ToString()));
        }

        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<City>>> GetAllCities(IPageable pageable)
        // {
        //     _log.LogDebug("REST request to get a page of Cities");
        //     var result = await _cityRepository.QueryHelper()
        //         .GetPageAsync(pageable);
        //     return Ok(result.Content).WithHeaders(result.GeneratePaginationHttpHeaders());
        // }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetAllCities([FromQuery] string loadEvent, IPageable pageable)
        {
            _log.LogDebug($"REST request to get a page of Cities : {loadEvent}");
            if(loadEvent != null && loadEvent != "undefined") {
                LazyLoadEvent _loadEvent = JsonConvert.DeserializeObject<LazyLoadEvent>(loadEvent);
                if(_loadEvent != null && _loadEvent.filters != null && _loadEvent.filters.Count > 0) {
                    LazyLoading<City> lazyLoading = new LazyLoading<City>(_loadEvent);
                    Expression<Func<City, bool>> expression = lazyLoading.ExpressionFromFilters();
                    IPage<City> filterResult;
                    if(expression == null) {
                        filterResult = await _cityRepository.QueryHelper()
                            .GetPageAsync(pageable);
                    }
                    else {
                        filterResult = await _cityRepository.QueryHelper()
                            .Filter(expression)
                            .GetPageAsync(pageable);
                    }
                    return Ok(filterResult.Content).WithHeaders(filterResult.GeneratePaginationHttpHeaders());
                }
            }
            var result = await _cityRepository.QueryHelper()
                .GetPageAsync(pageable);
            return Ok(result.Content).WithHeaders(result.GeneratePaginationHttpHeaders());
        }

        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<City>>> GetAllCities([FromQuery] string loadEvent, IPageable pageable)
        // {
        //     _log.LogDebug($"REST request to get a page of Cities : {loadEvent}");
        //     if(loadEvent != null && loadEvent != "undefined") {
        //         LazyLoadEvent _loadEvent = JsonConvert.DeserializeObject<LazyLoadEvent>(loadEvent);
        //         if(_loadEvent != null && _loadEvent.filters != null && _loadEvent.filters.Count > 0) {
        //             Type t = typeof(int?);
        //             Type u = Nullable.GetUnderlyingType(t);
        //             int? nullableIntValue = (int?)Convert.ChangeType(_loadEvent.filters["city.population"][0]["value"], u);
        //             Expression<Func<City, bool>> expression = city => city.Population == nullableIntValue;
        //             var filterResult = await _cityRepository.QueryHelper()
        //                 .Filter(expression)
        //                 .GetPageAsync(pageable);
        //             return Ok(filterResult.Content).WithHeaders(filterResult.GeneratePaginationHttpHeaders());
        //         }
        //     }
        //     var result = await _cityRepository.QueryHelper()
        //         .GetPageAsync(pageable);
        //     return Ok(result.Content).WithHeaders(result.GeneratePaginationHttpHeaders());
        // }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity([FromRoute] long id)
        {
            _log.LogDebug($"REST request to get City : {id}");
            var result = await _cityRepository.QueryHelper()
                .GetOneAsync(city => city.Id == id);
            return ActionResultUtil.WrapOrNotFound(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity([FromRoute] long id)
        {
            _log.LogDebug($"REST request to delete City : {id}");
            await _cityRepository.DeleteByIdAsync(id);
            await _cityRepository.SaveChangesAsync();
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id.ToString()));
        }
    }
}
