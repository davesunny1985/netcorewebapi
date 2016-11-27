using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestWebApp.Infrastructure.Interfaces;
using TestWebApp.Core.Entities;
using TestWebApp.Infrastructure.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace TestWebApp.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class ContactController : Controller
    {
        private readonly IContactRepository _contacts;
        public ContactController(IContactRepository contacts)
        {
            _contacts = contacts;
        }
        [HttpGet]
        public IActionResult Get()
        {
            //Add();
            return  Ok(_contacts.GetAll());
        }

        [HttpGet]
        [Route("{pageIndex:int}/{pageSize:int}")]
        public IActionResult Get(int pageIndex, int pageSize)
        {
            return Ok(new PagedResponse<Contact>(_contacts.GetAll(), pageIndex, pageSize));
        }

        [HttpPost]
        public IActionResult Add()
        {
            _contacts.Add(new Contact() {
                //Id = 0,
            Name="Contact 1"
            });
            _contacts.Commit();
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var contact = _contacts.FindBy(item=> item.Id==int.Parse(id));
            if (contact == null)
            {
                return NotFound();
            }

            _contacts.Delete(contact.Single());
            return new NoContentResult();
        }
    }
}