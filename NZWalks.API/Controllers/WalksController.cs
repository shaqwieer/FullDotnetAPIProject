﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repository;

namespace NZWalks.API.Controllers
{
    // /api/walks
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper,IWalkRepository walkRepository) 
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }
        //CREATE Walk
        //POST: /api/walks
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]AddWalkRequestDto addWalkRequestDto)
        {
            //map DTO to Domain Model
            var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);
            await walkRepository.CreateAsync(walkDomainModel);
            //Map Domain model to DTO
            return Ok(mapper.Map<walkDto>(walkDomainModel));
        }
        //GET walks
        //GET: /api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending, 
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize =1000)
        {
          var walksDomainModel=  await walkRepository.GetAllAsync(filterOn,filterQuery,sortBy,isAscending ?? true,
                                                                  pageNumber,pageSize);
           // to test global exception logger by customize middleware
           // throw new Exception("this is a new exception ");

          // Map Domain Model to DTO 
          return Ok(mapper.Map<List<walkDto>>(walksDomainModel));

        }

        //Get walk byId
        //Get: /api/walks/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute]Guid id)
        {
            var walkDomainModel=await walkRepository.GetByIdAsync(id);
            if (walkDomainModel == null)
            {
                return NotFound();
            }
            //map domain model to dto 
            return Ok(mapper.Map<walkDto>(walkDomainModel));

        }

        //update walk by id 
        //Put: /api/walks/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id,UpdateWalkRequestDto updateRegionRequestDto)
        {
            //map dto to domain model 
            var walkDomainModel = mapper.Map<Walk>(updateRegionRequestDto);

            walkDomainModel= await walkRepository.UpdateAsync(id, walkDomainModel);
            if (walkDomainModel == null)
            { return NotFound(); }
            // map domain model to dto 
            return Ok(mapper.Map<walkDto>(walkDomainModel));
            

        }

        // delete walk by id
        //Delete: /api/walks/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
           var deleteWalkDomainModel= await walkRepository.DeleteAsync(id);
            if(deleteWalkDomainModel == null)
            { return NotFound(); }
            // map domain model to dto 
            return Ok(mapper.Map<walkDto>(deleteWalkDomainModel));
        }
    }   
}
