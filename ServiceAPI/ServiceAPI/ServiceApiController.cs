using Microsoft.AspNetCore.Mvc;
using ServiceAPI.Dal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace ServiceAPI
{
    [Route("api")]
    public class ServiceApiController : Controller
    {
        static readonly object setupLock = new object();
        static readonly SemaphoreSlim parallelism = new SemaphoreSlim(2);

        [HttpGet("setup")]
        public IActionResult SetupDatabase()
        {
            lock (setupLock)
            {
                using (var context = new GlobalDbContext())
                {
                    // Create database
                    context.Database.EnsureCreated();
                    Student s = new Student()
                    {
                        Name = "giovanni",
                        DateOfBirth = new DateTime(2012, 1, 1),
                    };
                    Customer s1 = new Customer()
                    {
                        Name = "Niccolo",
                        DateOfBirth = new DateTime(2012, 1, 1),

                    };
                    Vehicle s2 = new Vehicle()
                    {
                        Brand = "Ferrari",
                        Model = "XX",
                        Price = "10100",

                    };
                    context.Vehicles.Add(s2);
                    context.Customers.Add(s1);
                    context.Students.Add(s);

                    context.SaveChanges();

                }
                return Ok("database created");
            }
        }


        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            try
            {
                await parallelism.WaitAsync();

                using (var context = new StudentsDbContext())
                {
                    return Ok(context.Students.ToList());
                }
            }
            finally
            {
                parallelism.Release();
            }
        }

        [HttpGet("student")]
        public async Task<IActionResult> GetStudent([FromQuery]int id)
        {
            using (var context = new StudentsDbContext())
            {
                return Ok(await context.Students.FirstOrDefaultAsync(x => x.Id == id));
            }
        }

        [HttpPut("students")]
        public async Task<IActionResult> CreateStudent([FromBody]Student student)
        {
            using (var context = new StudentsDbContext())
            {
                context.Students.Add(student);

                await context.SaveChangesAsync();

                return Ok();
            }
        }

        [HttpPost("students")]
        public async Task<IActionResult> UpdateStudent([FromBody]Student student)
        {
            using (var context = new StudentsDbContext())
            {
                context.Students.Update(student);
                await context.SaveChangesAsync();
                return Ok();
            }
        }   


        [HttpDelete("students")]
        public async Task<IActionResult> DeleteStudent([FromQuery]int id)
        {
            using (var context = new StudentsDbContext())
            {
                var student = await context.Students.FirstOrDefaultAsync(x => x.Id == id);
                if (student != null)
                {
                    context.Students.Remove(student);
                    await context.SaveChangesAsync();
                }
                return Ok();


            }
        }

        /// <summary>
        /// //////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                await parallelism.WaitAsync();

                using (var context = new GlobalDbContext())
                {
                    return Ok(context.Customers.ToList());
                }
            }
            finally
            {
                parallelism.Release();
            }
        }

        [HttpGet("customer")]
        public async Task<IActionResult> GetCustomer([FromQuery]int id)
        {
            using (var context = new GlobalDbContext())
            {
                return Ok(await context.Customers.FirstOrDefaultAsync(x => x.Id == id));
            }
        }

        [HttpPut("customers")]
        public async Task<IActionResult> CreateCustomer([FromBody]Customer customer)
        {
            using (var context = new GlobalDbContext())
            {
                context.Customers.Add(customer);

                await context.SaveChangesAsync();

                return Ok();
            }
        }

        [HttpPost("customers")]
        public async Task<IActionResult> UpdateCustomer([FromBody]Customer customer)
        {
            using (var context = new GlobalDbContext())
            {
                context.Customers.Update(customer);
                await context.SaveChangesAsync();
                return Ok();
            }
        }


        [HttpDelete("customers")]
        public async Task<IActionResult> DeleteCustomer([FromQuery]int id)
        {
            using (var context = new GlobalDbContext())
            {
                var customer = await context.Customers.FirstOrDefaultAsync(x => x.Id == id);
                if (customer != null)
                {
                    context.Customers.Remove(customer);
                    await context.SaveChangesAsync();
                }
                return Ok();


            }
        }

        /// <summary>
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// </summary>
        /// <returns></returns>


        [HttpGet("vehicles")]
        public async Task<IActionResult> GetVehicles()
        {
            try
            {
                await parallelism.WaitAsync();

                using (var context = new GlobalDbContext())
                {
                    return Ok(context.Vehicles.ToList());
                }
            }
            finally
            {
                parallelism.Release();
            }
        }

        [HttpGet("vehicle")]
        public async Task<IActionResult> GetVehicle([FromQuery]int id)
        {
            using (var context = new GlobalDbContext())
            {
                return Ok(await context.Vehicles.FirstOrDefaultAsync(x => x.Id == id));
            }
        }

        [HttpPut("vehicles")]
        public async Task<IActionResult> CreateVehicle([FromBody]Vehicle vehicle)
        {
            using (var context = new GlobalDbContext())
            {
                context.Vehicles.Add(vehicle);

                await context.SaveChangesAsync();

                return Ok();
            }
        }

        [HttpPost("vehicles")]
        public async Task<IActionResult> UpdateVehicle([FromBody]Vehicle vehicle)
        {
            using (var context = new GlobalDbContext())
            {
                context.Vehicles.Update(vehicle);
                await context.SaveChangesAsync();
                return Ok();
            }
        }


        [HttpDelete("vehicles")]
        public async Task<IActionResult> DeleteVehicle([FromQuery]int id)
        {
            using (var context = new GlobalDbContext())
            {
                var vehicle = await context.Vehicles.FirstOrDefaultAsync(x => x.Id == id);
                if (vehicle != null)
                {
                    context.Vehicles.Remove(vehicle);
                    await context.SaveChangesAsync();
                }
                return Ok();


            }
        }

    }
}
