using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
  [Route("api/v{version:apiVersion}/trails")]
  [ApiController]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status418ImATeapot)]
  public class TrailsController : ControllerBase
  {
    private readonly ITrailRepository _trailsRepo;
    private readonly IMapper _mapper;
    public TrailsController(ITrailRepository trailsRepo, IMapper mapper)
    {
      _trailsRepo = trailsRepo;
      _mapper = mapper;
    }

    /// <summary>
    /// Get list of all Trails
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<TrailDto>))]
    [ProducesResponseType(400)]
    public IActionResult GetTrails()
    {
      var TrailList = _trailsRepo.GetTrails();
      var TrailListDto = new List<TrailDto>();
      foreach (var item in TrailList)
      {
        TrailListDto.Add(_mapper.Map<TrailDto>(item));
      }

      return Ok(TrailListDto);
    }

    /// <summary>
    /// Get individual trail
    /// </summary>
    /// <param name="TrailId">The Id of the trail</param>
    /// <returns></returns>
    [HttpGet("{TrailId:int}", Name = "GetTrail")]
    [ProducesResponseType(200, Type = typeof(TrailDto))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesDefaultResponseType]
    [Authorize(Roles = "Admin")]
    public IActionResult GetTrail(int TrailId)
    {
      var Trail = _trailsRepo.GetTrail(TrailId);
      if (Trail == null)
      {
        return NotFound();
      }
      var TrailDto = _mapper.Map<TrailDto>(Trail);
      return Ok(TrailDto);
    }

    /// <summary>
    /// Get individual trail in national park
    /// </summary>
    /// <param name="TrailId">The Id of the trail</param>
    /// <returns></returns>
    [HttpGet("[action]/{nationalParkId:int}")]
    [ProducesResponseType(200, Type = typeof(TrailDto))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesDefaultResponseType]
    public IActionResult GetTrailInNationalPark(int nationalParkId)
    {
      var TrailList = _trailsRepo.GetTrailsInNationalPark(nationalParkId);
      if (TrailList == null)
      {
        return NotFound();
      }
      var TrailListDto = new List<TrailDto>();
      foreach (var item in TrailList)
      {
        TrailListDto.Add(_mapper.Map<TrailDto>(item));
      }
      return Ok(TrailListDto);
    }

    /// <summary>
    /// Create new national park
    /// </summary>
    /// <param name="TrailDto">new trail information</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(TrailDto))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateTrail([FromBody] TrailCreateDto TrailDto)
    {
      if (TrailDto == null)
      {
        return BadRequest(ModelState);
      }

      if (_trailsRepo.TrailExists(TrailDto.Name))
      {
        ModelState.AddModelError("", "Trail Exists");
        return StatusCode(404, ModelState);
      }

      var Trail = _mapper.Map<Trail>(TrailDto);
      if (!_trailsRepo.CreateTrail(Trail))
      {
        ModelState.AddModelError("", $"Some thing went wrong when saving the record {Trail.Name}");
        return StatusCode(500, ModelState);
      }

      return CreatedAtRoute("GetTrail", new { TrailId = Trail.Id }, Trail);
    }

    /// <summary>
    /// Update existing national park
    /// </summary>
    /// <param name="TrailId">id of the trail to update</param>
    /// <param name="TrailDto">data to update</param>
    /// <returns></returns>
    [HttpPatch("{TrailId:int}", Name = "UpdateTrail")]
    [ProducesResponseType(204)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateTrail(int TrailId, [FromBody] TrailUpdateDto TrailDto)
    {
      if (TrailDto == null || TrailId != TrailDto.Id)
      {
        return BadRequest(ModelState);
      }

      var Trail = _mapper.Map<Trail>(TrailDto);
      if (!_trailsRepo.UpdateTrail(Trail))
      {
        ModelState.AddModelError("", $"Some thing went wrong when updating the record {Trail.Name}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }

    /// <summary>
    /// Delete national park
    /// </summary>
    /// <param name="TrailId">Id of the trail to delete</param>
    /// <returns></returns>
    [HttpDelete("{TrailId:int}", Name = "DeleteTrail")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult DeleteTrail(int TrailId)
    {
      if (!_trailsRepo.TrailExists(TrailId))
      {
        return NotFound();
      }

      var Trail = _trailsRepo.GetTrail(TrailId);
      if (!_trailsRepo.DeleteTrail(Trail))
      {
        ModelState.AddModelError("", $"Some thing went wrong when deleting the record {Trail.Name}");
        return StatusCode(500, ModelState);
      }
      return NoContent();
    }

  }
}