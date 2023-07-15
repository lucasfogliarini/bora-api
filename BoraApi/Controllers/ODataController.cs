﻿using Bora.Database;
using Bora.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Bora.Api.Controllers
{
    public abstract class ODataController<TEntity> : ODataController where TEntity: class, IEntity
    {
        protected readonly IBoraDatabase _boraDatabase;

        public ODataController(IBoraDatabase boraDatabase)
        {
            _boraDatabase = boraDatabase;
        }

        [HttpOptions]
        public ActionResult GetOptionsAsync()
        {
            return null;
        }

        [EnableQuery]
        public IEnumerable<TEntity> Get()
        {
            return _boraDatabase.Query<TEntity>();
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
