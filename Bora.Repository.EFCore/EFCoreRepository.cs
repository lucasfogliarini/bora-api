using Bora.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Bora.Repository
{
    internal class EFCoreRepository(BoraDbContext dbContext) : IRepository
    {
        readonly DbContext _dbContext = dbContext;

        public IQueryable<TEntity> Query<TEntity>() where TEntity : Entity
        {
            var query = _dbContext.Set<TEntity>();
            return query;
        }
        public void Add<TEntity>(TEntity entity) where TEntity : Entity
        {
            _dbContext.Add(entity);
        }
        public async Task<int> CommitAsync()
        {
            try
            {
                var changes = await _dbContext.SaveChangesAsync();
                return changes;
            }
            catch (DbUpdateException ex)
            {
                throw new ValidationException(ex.GetBaseException().Message);
            }
        }
        public void Update<TEntity>(TEntity entity) where TEntity : Entity
        {
            _dbContext.Update(entity);
        }
        public void Remove<TEntity>(TEntity entity) where TEntity : Entity
        {
            _dbContext.Remove(entity);
        }
        public IQueryable<TEntity> Where<TEntity>(Expression<Func<TEntity, bool>>? where) where TEntity : Entity
        {
            return Query<TEntity>().Where(where);
        }
        public IQueryable<TEntity> All<TEntity>() where TEntity : Entity
        {
            return Query<TEntity>().ToList().AsQueryable();
        }
        public bool Any<TEntity>(Expression<Func<TEntity, bool>>? where = null) where TEntity : Entity
        {
            return where == null ? Query<TEntity>().Any() : Query<TEntity>().Any(where);
        }
        public TEntity? FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : Entity
        {
            return Query<TEntity>().FirstOrDefault(where);
        }
        public void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity
        {
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }
        public async Task SeedAsync()
        {
            var hasAccount = Any<Account>();
            if (!hasAccount)
            {
                var accounts = new List<Account>
                {
                    new("bora.reunir@gmail.com", 1, new DateTime(2024, 5, 14))
                    {
                        Username = "bora.work",
                        Name = "Bora!",
                        Photo = "https://lh3.googleusercontent.com/a/ACg8ocLnDADC73MLqQGSXles9WknoIIjN1KHizpqjfsLd2DBi_Em_Xw=s360-c-no",
                        WhatsApp = "51992364249",
                        Instagram = "lucasfogliarini",
                        Spotify = "12145833562",
                        Linkedin = "lucasfogliarini"
                    },
                    new("lucasfogliarini@gmail.com", 2, new DateTime(2024, 5, 14))
                    {
                        Name = "Lucas Fogliarini Pedroso",
                        Photo = "https://lh3.googleusercontent.com/a-/AOh14Ggingx4m5A-dFGLwEJv-acJ-KEDtApHCAO0NxfUig=s96-c",
                        WhatsApp = "51992364249",
                        Instagram = "lucasfogliarini",
                        Spotify = "12145833562",
                        Linkedin = "lucasfogliarini"
                    },
                    new("luanaleticiabueno@gmail.com", 3, new DateTime(2024, 5, 14))
                    {
                        Name = "Luana Bueno",
                        WhatsApp = "5193840006",
                        Instagram = "luanabuenoflores",
                        Spotify = "224juavirzfsjsxt5yva6fvly",
                        Photo= "https://lh3.googleusercontent.com/a-/AOh14GhWN-zhlu_93Me88oT9v8554pdaJQdNYKpUp-i__c0=s340-p-k-rw-no",
                    },
                    new("gui_staub@hotmail.com", 4, new DateTime(2024, 5, 14))
                    {
                        Name = "Guilherme Staub",
                        Photo= "https://lh3.googleusercontent.com/a-/AOh14Gi14cQFSeyn5q6u3ZB_derhI7yIcA9dgX27OkBl=s96-c",
                    },
                    new("varreira.adv@gmail.com", 5, new DateTime(2024, 5, 14))
                    {
                        Name = "Anderson Varreira",
                        Photo= "https://lh3.googleusercontent.com/a/AATXAJw-6J_C5vAh-d9Gp3ssN_ziJrOkzp6HMWXE6Ubm=s96-c",
                    },
                    new("lucasbuenomagalhaes@gmail.com", 6, new DateTime(2024, 5, 14))
                    {
                        Name = "Lucas Bueno",
                        Photo= "https://lh3.googleusercontent.com/a-/AOh14Ggfyxso7uuqWxLMqvI3JTDOcKDRKkOgsz0oOwLWPw=s96-c",
                    }
                };
                foreach (var account in accounts)
                {
                    account.Id = null;
                    Add(account);
                }

                await CommitAsync();
            }

            var hasContent = Any<Content>();
            if (!hasContent)
            {
                var homeContents = new List<Content>
                {
                    new() {
                        Collection = "home",
                        Key = "boraLink",
                        Text = "/bora"
                    },
                    new() {
                        Collection = "home",
                        Key = "boraText",
                        Text = "Bora!"
                    }
                };

                foreach (var homeContent in homeContents)
                {
                    homeContent.CreatedAt = DateTime.Now;
                    homeContent.AccountId = 1;//lucasfogliarini
                    Add(homeContent);
                }

                await CommitAsync();
            }
        }
    }
}
