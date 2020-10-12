using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/nationalparks")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    public class NationalParksController : ControllerBase
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;
        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of all national parks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetNationalParks()
        {
            var ParkList = _npRepo.GetNationalParks();
            var ParkListDto = new List<NationalParkDto>();
            foreach (var item in ParkList)
            {
                ParkListDto.Add(_mapper.Map<NationalParkDto>(item));
            }

            return Ok(ParkListDto);
        }

        /// <summary>
        /// Get individual national park
        /// </summary>
        /// <param name="NationalParkId">The Id of the national park</param>
        /// <returns></returns>
        [HttpGet("{NationalParkId:int}", Name = "GetNationalPark")]
        [ProducesResponseType(200, Type = typeof(NationalParkDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Authorize]
        [ProducesDefaultResponseType]
        public IActionResult GetNationalPark(int NationalParkId)
        {
            var Park = _npRepo.GetNationalPark(NationalParkId);
            if (Park == null)
            {
                return NotFound();
            }
            var ParkDto = _mapper.Map<NationalParkDto>(Park);
            return Ok(ParkDto);
        }

        /// <summary>
        /// Create new national park
        /// </summary>
        /// <param name="NationalParkDto">new national park information</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto NationalParkDto)
        {
            if (NationalParkDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_npRepo.NationalParkExists(NationalParkDto.Name))
            {
                ModelState.AddModelError("", "National Park Exists");
                return StatusCode(404, ModelState);
            }

            var NationalPark = _mapper.Map<NationalPark>(NationalParkDto);
            if (!_npRepo.CreateNationalPark(NationalPark))
            {
                ModelState.AddModelError("", $"Some thing went wrong when saving the record {NationalPark.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationalPark", new { version = HttpContext.GetRequestedApiVersion(), NationalParkId = NationalPark.Id }, NationalPark);
        }

        /// <summary>
        /// Update existing national park
        /// </summary>
        /// <param name="NationalParkId">id of the park to update</param>
        /// <param name="NationalParkDto">data to update</param>
        /// <returns></returns>
        [HttpPatch("{NationalParkId:int}", Name = "UpdateNationalPark")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateNationalPark(int NationalParkId, [FromBody] NationalParkDto NationalParkDto)
        {
            if (NationalParkDto == null || NationalParkId != NationalParkDto.Id)
            {
                return BadRequest(ModelState);
            }

            var NationalPark = _mapper.Map<NationalPark>(NationalParkDto);
            if (!_npRepo.UpdateNationalPark(NationalPark))
            {
                ModelState.AddModelError("", $"Some thing went wrong when updating the record {NationalPark.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Delete national park
        /// </summary>
        /// <param name="NationalParkId">Id of the park to delete</param>
        /// <returns></returns>
        [HttpDelete("{NationalParkId:int}", Name = "DeleteNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteNationalPark(int NationalParkId)
        {
            if (!_npRepo.NationalParkExists(NationalParkId))
            {
                return NotFound();
            }

            var NationalPark = _npRepo.GetNationalPark(NationalParkId);
            if (!_npRepo.DeleteNationalPark(NationalPark))
            {
                ModelState.AddModelError("", $"Some thing went wrong when deleting the record {NationalPark.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}