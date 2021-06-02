using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using JHipsterNet.Core.Pagination.Extensions;
using company.world.Domain;
using company.world.Security;
using company.world.Domain.Services.Interfaces;
using company.world.Dto;
using company.world.Web.Extensions;
using company.world.Web.Filters;
using company.world.Web.Rest.Problems;
using company.world.Web.Rest.Utilities;
using company.world.Crosscutting.Constants;
using company.world.Crosscutting.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace company.world.Controllers
{
    [Authorize]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _log;
        private readonly IMailService _mailService;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(ILogger<UsersController> log, UserManager<User> userManager, IUserService userService,
            IMapper mapper, IMailService mailService)
        {
            _log = log;
            _userManager = userManager;
            _userService = userService;
            _mailService = mailService;
            _mapper = mapper;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<ActionResult<User>> CreateUser([FromBody] UserDto userDto)
        {
            _log.LogDebug($"REST request to save User : {userDto}");
            if (userDto.Id != null)
                throw new BadRequestAlertException("A new user cannot already have an ID", "userManagement",
                    "idexists");
            // Lowercase the user login before comparing with database
            if (await _userManager.FindByNameAsync(userDto.Login.ToLowerInvariant()) != null)
                throw new LoginAlreadyUsedException();
            if (await _userManager.FindByEmailAsync(userDto.Email.ToLowerInvariant()) != null)
                throw new EmailAlreadyUsedException();

            var newUser = await _userService.CreateUser(_mapper.Map<User>(userDto));
            await _mailService.SendCreationEmail(newUser);
            return CreatedAtAction(nameof(GetUser), new { login = newUser.Login }, newUser)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert("userManagement.created", newUser.Login));
        }

        [HttpPut]
        [ValidateModel]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            _log.LogDebug($"REST request to update User : {userDto}");
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null && !existingUser.Id.Equals(userDto.Id)) throw new EmailAlreadyUsedException();
            existingUser = await _userManager.FindByNameAsync(userDto.Login);
            if (existingUser != null && !existingUser.Id.Equals(userDto.Id)) throw new LoginAlreadyUsedException();

            var updatedUser = await _userService.UpdateUser(_mapper.Map<User>(userDto));

            return ActionResultUtil.WrapOrNotFound(updatedUser)
                .WithHeaders(HeaderUtil.CreateAlert("userManagement.updated", userDto.Login));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Users");
            var page = await _userManager.Users
                .Include(it => it.UserRoles)
                .ThenInclude(r => r.Role)
                .UsePageableAsync(pageable);
            var userDtos = page.Content.Select(user => _mapper.Map<UserDto>(user));
            var headers = page.GeneratePaginationHttpHeaders();
            return Ok(userDtos).WithHeaders(headers);
        }

        [HttpGet("authorities")]
        [Authorize(Roles = RolesConstants.ADMIN)]
        public ActionResult<IEnumerable<string>> GetAuthorities()
        {
            return Ok(_userService.GetAuthorities());
        }

        [HttpGet("{login}")]
        public async Task<IActionResult> GetUser([FromRoute] string login)
        {
            _log.LogDebug($"REST request to get User : {login}");
            var result = await _userManager.Users
                .Where(user => user.Login == login)
                .Include(it => it.UserRoles)
                .ThenInclude(r => r.Role)
                .SingleOrDefaultAsync();
            var userDto = _mapper.Map<UserDto>(result);
            return ActionResultUtil.WrapOrNotFound(userDto);
        }

        [HttpDelete("{login}")]
        [Authorize(Roles = RolesConstants.ADMIN)]
        public async Task<IActionResult> DeleteUser([FromRoute] string login)
        {
            _log.LogDebug($"REST request to delete User : {login}");
            await _userService.DeleteUser(login);
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert("userManagement.deleted", login));
        }
    }
}
