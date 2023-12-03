using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using WebApplication1.Data;
using WebApplication1.Migrations;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private static int ContractCounter;
        public ContractsController(ApplicationDbContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //show all my contracts 
        [HttpGet]
        [Route("showcontract")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<Contract>>> ShowContracts()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                Massage response = new Massage
                {
                    massage = "Invalid User Id"
                };
                return BadRequest(response);
            }

            //var role = await _userManager.IsInRoleAsync(user, Roles.Provider);
            var roles = await _userManager.GetRolesAsync(user);
            //var contracts = await _context.Contracts.ToListAsync();
            if (roles.Contains("RequesterUser"))
            {
                ContractCounter = 0;

                var requsterContract = _context.Contracts.Where(r => r.RequesterId == userId)
                   .Select(contract => new Contractviewmode
                   {
                       ContractId = contract.ContractId,
                       Title = contract.Title,
                       Status = contract.Status,
                       name = contract.Provider.FirstName + " " + contract.Provider.LastName,
                   })
                   .ToList();
                return Ok(requsterContract);
            }
            else if (roles.Contains("ProviderUser"))
            {
                ContractCounter = 0;

                var providerContract = _context.Contracts.Where(r => r.ProviderId == userId)
                    .Select(contract => new Contractviewmode
                    {
                        ContractId = contract.ContractId,
                        Title = contract.Title,
                        Status = contract.Status,
                        name = contract.Requester.FirstName + " " + contract.Requester.LastName
                    })
                    .ToList();
                return Ok(providerContract);


            }
            return Ok("");
        }
        //show all my contracts 
        [HttpGet]
        [Route("showcontractwithdetaile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<Contract>>> showcontractwithdetaile(int contractid)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await  _userManager.FindByIdAsync(userId);
            
            if(user == null)
            {
                Massage response = new Massage
                {
                    massage = "Invalid User Id"
                };
                return BadRequest(response);
            }

            //var role = await _userManager.IsInRoleAsync(user, Roles.Provider);
            var roles = await _userManager.GetRolesAsync(user);
            //var contracts = await _context.Contracts.ToListAsync();
            if (roles.Contains("RequesterUser"))
            {
                ContractCounter = 0;

                var requsterContract = _context.Contracts.Where(r => r.RequesterId == userId && r.ContractId==contractid)
                   .Select(contract => new ContractCreateViewModel
                   {
                       ContractId = contract.ContractId,
                       Description = contract.Description,
                       EndDate = contract.EndDate,
                       StartDate = contract.StartDate,
                       Price = contract.Price,
                       Total=contract.Total,
                       Title = contract.Title,
                       Status= contract.Status,
                       ProviderName = contract.Provider.FirstName + " " + contract.Provider.LastName,
                       RequesterName = contract.Requester.FirstName + " " + contract.Requester.LastName
                   })
                   .ToList();
                    return Ok(requsterContract);
            }
            else if (roles.Contains("ProviderUser"))
            {
                ContractCounter = 0;

                var providerContract = _context.Contracts.Where(r => r.ProviderId == userId && r.ContractId == contractid)
                    .Select(contract => new ContractCreateViewModel
                    {
                        ContractId = contract.ContractId,
                        Description = contract.Description,
                        EndDate = contract.EndDate,
                        StartDate = contract.StartDate,
                        Price = contract.Price,
                        Total = contract.Total,
                        Title = contract.Title,
                        Status=contract.Status,
                        ProviderName = contract.Provider.FirstName + " " + contract.Provider.LastName,
                        RequesterName = contract.Requester.FirstName + " " + contract.Requester.LastName
                    })
                    .ToList();
                     return Ok(providerContract);
            
            
            }
            return Ok("");
        }


        //show all confirm Contract 
        [HttpGet]
        [Route("showconfirm")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<ActionResult<IEnumerable<Contract>>> ShowConfirmContracts()

        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                Massage response = new Massage
                {
                    massage = "Invalid User Id"
                };

                return BadRequest(response);
            }

            var role = await _userManager.IsInRoleAsync(user, Roles.Provider);
                
            if(!role)
            {
                return Ok(await
                    _context.Contracts
                    .Where(req => req.RequesterId == user.Id && 
                    req.Status.Equals(ContractStatus.Confirm))
                    .ToListAsync());
            }

            else
            {
                return Ok(await
                   _context.Contracts
                   .Where(provider => provider.ProviderId == user.Id 
                                            && provider.Status.Equals(ContractStatus.Confirm))
                   .ToListAsync());
            }

        }


        [HttpPost]
        [Route("confirm")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "RequesterUser")]
        public async Task<ActionResult> ChangeContractStatus(ContracStatus contracts)
        {


            var contract = await _context.Contracts.FirstOrDefaultAsync(c => c.ContractId == contracts.ContractId);
            float price = contract.Price;

            if (contract.Status == ContractStatus.Pending && contracts.Status== "Confirm")
            {
                contract.Status = ContractStatus.Confirm;
                //float tax = 0.9f;
                //contract.Total = price * tax;
                _context.Update(contract);
                await _context.SaveChangesAsync();
                Massage response = new Massage
                {
                    massage = "Confirmed"
                };
                return Ok(response);
            }

            return BadRequest();

        }



        //Cancle contract
        [HttpPost]
        [Route("cancle")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RequesterUser")]

        public async Task<ActionResult> CancelContractStatus(ContracStatus contracts)
        {

            var contract = await _context.Contracts.FirstOrDefaultAsync(c => c.ContractId == contracts.ContractId);

            if (contract.Status == ContractStatus.Pending && contracts.Status == "Cancle")
            {
                contract.Status = ContractStatus.Cancle;
                _context.Update(contract);

                await _context.SaveChangesAsync();
                Massage response = new Massage
                {
                    massage = "Canceld"
                };
                return Ok(response);
            }
            return BadRequest();
        }

        // GET: api/Contracts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contract>>> GetContracts()
        {
            return await _context.Contracts.ToListAsync();
        }

        // GET: api/Contracts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contract>> GetContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return contract;
        }

        // PUT: api/Contracts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContract(int id, Contract contract)
        {
            if (id != contract.ContractId)
            {
                return BadRequest();
            }

            _context.Entry(contract).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Contracts
        
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "ProviderUser")]
        [Route("createcontract")]
        public async Task<ActionResult<Contract>> PostContract(Contract contract)
        {            
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var contractNew = new Contract
            {
                Description = contract.Description,
                EndDate = contract.EndDate,
                Price = contract.Price,
                StartDate = contract.StartDate,
                Title = contract.Title,
                Status = ContractStatus.Pending,
                ProviderId = userId,
                Total= contract.Total,
                RequesterId = contract.RequesterId,
            
            };
            ContractCounter = 1 + ContractCounter;
            _context.Contracts.Add(contractNew);
            await _context.SaveChangesAsync();

            var providerFullName = await _userManager.FindByIdAsync(contractNew.ProviderId);
            var requesterFullName = await _userManager.FindByIdAsync(contractNew.RequesterId);

            var returendContract = new ContractCreateViewModel
            {
                ContractId = contractNew.ContractId,
                Description = contractNew.Description,
                EndDate = contractNew.EndDate,
                StartDate = contractNew.StartDate,
                Price = contractNew.Price,
                Title = contractNew.Title,
                Status= contractNew.Status,
                Total= contractNew.Total,
                ProviderName = providerFullName.FirstName + " " + providerFullName.LastName,
                RequesterName = requesterFullName.FirstName + " " + requesterFullName.LastName
            };


            return CreatedAtAction("GetContract", new { id = contractNew.ContractId }, returendContract);
        }
       

        // DELETE: api/Contracts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Contract>> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return contract;
        }
        [HttpGet]
        [Route("ContractNotification")]

        public async Task<ActionResult> ContractNotification()
        {
            Note c = new Note
            {
                Count = ContractCounter

            };
            

            return Ok(c);
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.ContractId == id);
        }
    }
}
