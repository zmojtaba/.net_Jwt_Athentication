using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SecondApp.Data;
using SecondApp.Models;
using SecondApp.Models.Entities;
using System.Drawing;
using System.Windows.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

namespace SecondApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public EmployeeController(ApplicationDbContext dbContext) { 
            this.dbContext = dbContext;
        }

        //[Route("api/employees")]
        [HttpGet]
        public ActionResult GetAllEmployees()
        {
            Random random = new Random();
            int width = 1024;
            int height = 1024;

            // Create an image in memory
            var image = new Image<Rgba32>(width, height);

            // Fill the image with random pixel values
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    byte r = (byte)random.Next(256);
                    byte g = (byte)random.Next(256);
                    byte b = (byte)random.Next(256);

                    image[i, j] = new Rgba32(r, g, b);
                }
            }

            // Save image to memory stream
            using (var ms = new MemoryStream())
            {
                image.Save(ms, new PngEncoder()); // Save directly to MemoryStream
                return File(ms.ToArray(), "image/png");
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public ActionResult GetApiEmployee(Guid id)
        {
            Employee employee = dbContext.Employees.Find(id = id);
            return Ok(employee);
        }

        [HttpPost]
        public ActionResult AddEmployee(AddEmployeeDto EmployeeDto)
        {
            Console.WriteLine($"========={EmployeeDto.Name}");
            Employee employee = new Employee()
            {
                Email = EmployeeDto.Email,
                Name    = EmployeeDto.Name,
            };

            dbContext.Employees.Add(employee);
            dbContext.SaveChanges();

            return Ok("added to database");
        }
    }
}
