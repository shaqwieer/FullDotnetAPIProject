using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repository;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResgionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<ResgionsController> logger;

        public ResgionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper,ILogger<ResgionsController> logger )
            
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }


        //GET ALL REGIONS
        //GET:HTTPS//localhost:portnumber/api/regions
        [HttpGet]
        [Authorize(Roles ="Reader")]

        public async Task<IActionResult>  GetAll() 
        {
          
                // log to file
                //get data from database-domain models
                //oldmethod: var regionsDomain= await dbContext.Regions.ToListAsync();
                var regionsDomain = await regionRepository.GetAllAsync();

                //map domain models to DTOs

                //old:var regionsDto = new List<RegionDto>();
                //foreach (var regionDomain in regionsDomain)
                //{
                //    regionsDto.Add(new RegionDto()
                //    {
                //        Id = regionDomain.Id,
                //        Code = regionDomain.Code,
                //        Name = regionDomain.Name,
                //        RegionImageUrl = regionDomain.RegionImageUrl,

                //    });
                //}
                //new autoMapper
                var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);
                // Return DTOs
                return Ok(regionsDto);

            
           
        }





        //GET SINGLE REGION (Get Region By ID)
        //GET:HTTPS//localhost:portnumber/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader")]


        public async Task<IActionResult>  GetById([FromRoute]Guid id)
        {
            //var regionDomain =  dbContext.Regions.Find(id);
            //old: var regionDomain=await dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id);
            var regionDomain = await regionRepository.GetByIdAsync(id);
            //Get Region Domain Model From Database
            if (regionDomain == null)
            {
                return NotFound();
            }

            //Map/Convert Region Domain model to Region Dto 


            //old:var regionDto = new RegionDto()
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomain.Id,
            //    Code = regionDomain.Code,
            //    Name = regionDomain.Name,
            //    RegionImageUrl = regionDomain.RegionImageUrl,
            //};
                
            return Ok(mapper.Map<RegionDto>(regionDomain));

        }


        //Post To Create New Region 
        //Post:https://localhost:portnumber/api/regions
        [HttpPost]
        // instead of if validating , we use custom validate
        [ValidateModel]
        [Authorize(Roles = "Writer,Reader")]

        public async Task<IActionResult>  Create([FromBody]AddRegionRequestDto addRegionRequestDto)
        {
            //if (ModelState.IsValid)

            //{


                // Map or Convert Dto to Domain Model

                //var regionDomainModal = new Region
                //{
                //    Code = addRegionRequestDto.Code,
                //    Name = addRegionRequestDto.Name,
                //    RegionImageUrl = addRegionRequestDto.RegionImageUrl
                //};
                var regionDomainModal = mapper.Map<Region>(addRegionRequestDto);
                //Use Domain Model to create Region
                //old: await dbContext.Regions.AddAsync(regionDomainModal);
                //old: await dbContext.SaveChangesAsync();
                regionDomainModal = await regionRepository.CreateAsync(regionDomainModal);
                //Map Domain model back to DTO

                //var regionDto = new RegionDto
                //{
                //    Id = regionDomainModal.Id,
                //    Code = regionDomainModal.Code,
                //    Name = regionDomainModal.Name,
                //    RegionImageUrl = regionDomainModal.RegionImageUrl,
                //};
                var regionDto = mapper.Map<RegionDto>(regionDomainModal);

                return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);

            //}
           // return BadRequest();
         

        }


        //Update region 
        //Put: https://localhost:portnumber/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]

        public async Task<IActionResult>  Update([FromRoute]Guid id, [FromBody]UpdateRegionRequestDto updateRegionRequestDto)
        {
            if(ModelState.IsValid)
            {
                //map Dto to Domain Model

                //var regionDomainModel = new Region
                //{
                //    Code = updateRegionRequestDto.Code,
                //    Name = updateRegionRequestDto.Name,
                //    RegionImageUrl= updateRegionRequestDto.RegionImageUrl,  

                //};
                var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

                //check if region exists

                //old:var regionDomainModel=await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
                regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);
                if (regionDomainModel == null)
                {
                    return NotFound();
                }
                //Map DTO to Domain model(donot necessary if use repository)
                //regionDomainModel.Code = updateRegionRequestDto.Code;
                //regionDomainModel.Name = updateRegionRequestDto.Name;
                //regionDomainModel.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;
                //await dbContext.SaveChangesAsync();

                //Convert Domain Model to DTO
                var regionDto = mapper.Map<RegionDto>(regionDomainModel);
                return Ok(regionDto);




            }
            else
            {
                return BadRequest();

            }



        }

        //Delete Region 
        //DELETE:HTTPS//localhost:portnumber/api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]

        public async Task<IActionResult>  Delete([FromRoute]Guid id)
        {
            //var regionDomainModel= await dbContext.Regions.FirstOrDefaultAsync(x=>x.Id == id);
            var regionDomainModel = await regionRepository.DeleteAsync(id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }
            //Delete region
            // dbContext.Regions.Remove(regionDomainModel);
            //await dbContext.SaveChangesAsync();
            //return deleted Region back

            //map Domain Model to DTO
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);
            return Ok(regionDto);
        
        }

    }
}
