using AutoMapper;
using Ifasanat.MeterMonitoringManagement.Domain;
using Ifasanat.MeterMonitoringManagement.Dto;
using Ifasanat.MeterMonitoringManagement.Repositories;
using Ifasanat.MeterMonitoringManagement.Validators;
using Microsoft.AspNetCore.Mvc;

namespace Ifasanat.MeterMonitoringManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public CityController(ICityRepository cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CityDto>> GetByIdAsync(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CityDto>(city));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetAllAsync()
        {
            var cities = await _cityRepository.GetAllAsync();

            return Ok(_mapper.Map<IEnumerable<CityDto>>(cities));
        }

        [HttpPost]
        public async Task<ActionResult<CityDto>> AddAsync(CityDto cityDto)
        {
            var validator = new CityValidator();
            var validationResult = await validator.ValidateAsync(cityDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var city = _mapper.Map<City>(cityDto);
            await _cityRepository.AddAsync(city);

            return CreatedAtAction("AddAsync", new { id = city.Id }, _mapper.Map<CityDto>(city));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CityDto>> UpdateAsync(int id, CityDto cityDto)
        {
            if (id != cityDto.Id)
            {
                return BadRequest();
            }

            var validator = new CityValidator();
            var validationResult = await validator.ValidateAsync(cityDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var city = _mapper.Map<City>(cityDto);
            await _cityRepository.UpdateAsync(city);

            return Ok(_mapper.Map<CityDto>(city));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            await _cityRepository.DeleteAsync(city);

            return NoContent();
        }
    }
}
