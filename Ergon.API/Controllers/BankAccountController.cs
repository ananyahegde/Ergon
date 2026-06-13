using Asp.Versioning;
using Ergon.DTOs.BankAccount;
using Ergon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Ergon.Exceptions;

namespace Ergon.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/employees/{employeeId}/bank-accounts")]
    [Authorize(Policy = "AllRoles")]
    [EnableRateLimiting("Fixed")]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBankAccounts(Guid employeeId)
        {
            var bankAccounts = await _bankAccountService.GetAllBankAccountsAsync(employeeId);
            return Ok(bankAccounts);
        }

        [HttpGet("{bankAccountId}")]
        public async Task<IActionResult> GetBankAccountById(Guid bankAccountId)
        {
            var bankAccount = await _bankAccountService.GetBankAccountByIdAsync(bankAccountId);
            return Ok(bankAccount);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBankAccount(Guid employeeId, [FromBody] BankAccountRequest request)
        {
            var loggedInId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            if (role == "Employee" && loggedInId != employeeId)
                throw new ForbiddenException("You can only manage your own bank accounts.");

            var bankAccount = await _bankAccountService.CreateBankAccountAsync(employeeId, request);
            return Created("", bankAccount);
        }

        [HttpPut("{bankAccountId}")]
        public async Task<IActionResult> UpdateBankAccount(Guid employeeId, Guid bankAccountId, [FromBody] BankAccountRequest request)
        {
            var loggedInId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            if (role == "Employee" && loggedInId != employeeId)
                throw new ForbiddenException("You can only manage your own bank accounts.");

            var bankAccount = await _bankAccountService.UpdateBankAccountAsync(bankAccountId, request);
            return Ok(bankAccount);
        }

        [HttpDelete("{bankAccountId}")]
        public async Task<IActionResult> DeleteBankAccount(Guid employeeId, Guid bankAccountId)
        {
            var loggedInId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            if (role == "Employee" && loggedInId != employeeId)
                throw new ForbiddenException("You can only manage your own bank accounts.");

            var bankAccount = await _bankAccountService.DeleteBankAccountAsync(bankAccountId);
            return Ok(new { message = "Bank account deleted.", data = bankAccount });
        }

        [HttpPut("{bankAccountId}/set-primary")]
        public async Task<IActionResult> SetPrimary(Guid employeeId, Guid bankAccountId)
        {
            var loggedInId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            if (role == "Employee" && loggedInId != employeeId)
                throw new ForbiddenException("You can only manage your own bank accounts.");

            var bankAccount = await _bankAccountService.SetPrimaryAsync(employeeId, bankAccountId);
            return Ok(bankAccount);
        }
    }
}
