﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository.AzureTables;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Bora.Api.Controllers
{
	public abstract class ODataController<TEntity> : ODataController where TEntity: Entity
    {
        protected readonly IAzureTablesRepository _boraRepository;

        public ODataController(IAzureTablesRepository boraRepository)
        {
            _boraRepository = boraRepository;
        }

        [HttpOptions]
        public ActionResult GetOptionsAsync()
        {
            return null;
        }

        [EnableQuery]
        public IEnumerable<TEntity> Get()
        {
            return _boraRepository.All<TEntity>();
        }

        public string AuthenticatedUserEmail
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                {
                    throw new ValidationException("Usuário não autenticado.");
                }
                var email = this.User.FindFirst(ClaimTypes.Email)?.Value;
                return email;
            }
        }
    }
}
