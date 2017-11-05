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
                   
                    Customer c1 = new Customer()
                    {
                        Name = "Niccolo",
                        Surname="Consoli",
                        DateOfBirth = new DateTime(1992, 1, 1),

                    };
                    Customer c2 = new Customer() {
                        Name = "Riccardo",
                        Surname = "Andro",
                        DateOfBirth = new DateTime(1992, 1, 1),
                    };
                    Customer c3 = new Customer()
                    {
                    Name = "Gianluca",
                    Surname = "Aziella",
                    DateOfBirth = new DateTime(1992, 1, 1),
                    };
                    Customer c4= new Customer()
                    {
                        Name = "Gigi",
                        Surname = "Buffon",
                        DateOfBirth = new DateTime(1987, 1, 1),
                    };

                
                    Vehicle s2 = new Vehicle()
                    {
                        Brand = "Ferrari",
                        Model = "Rossa",
                        Price = "10000000",
                    };
                    Vehicle s3 = new Vehicle()
                    {
                        Brand = "Mercedez",
                        Model = "XLK-9",
                        Price = "50000",
                    };
                    Vehicle s4 = new Vehicle()
                    {
                        Brand = "BMW",
                        Model = "Serie A",
                        Price = "250000",
                    };
                    Vehicle s5 = new Vehicle()
                    {
                        Brand = "Opel",
                        Model = "Meriva",
                        Price = "90000",
                    };


                    context.Vehicles.Add(s2);
                    context.Vehicles.Add(s3);
                    context.Vehicles.Add(s4);
                    context.Vehicles.Add(s5);
                    context.Customers.Add(c1);
                    context.Customers.Add(c2);
                    context.Customers.Add(c3);
                    context.Customers.Add(c4);
                    context.SaveChanges();

                }
                return Ok("database created");
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
