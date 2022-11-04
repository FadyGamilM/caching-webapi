using Microsoft.AspNetCore.Mvc;
using CachingApi.Services;
using CachingApi.Data;
using CachingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CachingApi.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
   private readonly ICacheService _cacheService; 
   private readonly AppDbContext _context;
   public StudentsController(
      ICacheService cacheService,
      AppDbContext context
   )
   {
      this._cacheService = cacheService;
      this._context = context;
   }


   [HttpGet]
   public async Task<IActionResult> GetStudents()
   {
      // check the data in the cache
      var cachedData = _cacheService.GetData<IEnumerable<Student>>("students");
      if(cachedData != null && cachedData.Count() > 0){
         return Ok(cachedData);
      }

      // get the data from the DB and cache it
      var DbData = await _context.Students.ToListAsync();

      // set the expiry time
      var expiryTime = DateTimeOffset.Now.AddSeconds(40);
      _cacheService.SetData<IEnumerable<Student>>("students", DbData, expiryTime);

      // return the data
      return Ok(DbData);
   }

   [HttpPost]
   public async Task<IActionResult> CreateStudent(Student entity)
   {
      var created = await _context.Students.AddAsync(entity);
      
   }
}