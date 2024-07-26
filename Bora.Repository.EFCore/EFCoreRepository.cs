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
            var hasResponsibilityArea = Any<ResponsibilityArea>();
            if (!hasResponsibilityArea)
            {
                var responsibilityAreas = new List<ResponsibilityArea>
                {
                    new("Ciências Exatas e da Terra", "Matemática, Física, Química, Astronomia, Geologia e Meteorologia"),
                    new("Ciências Humanas e Sociais", "Filosofia, Direito, Ciências Políticas, Sociologia, Psicologia, Antropologia e História"),
                    new("Engenharia e Tecnologia", "Engenharia de Computação, Tecnologia da Informação, Engenharia de Software, Engenharia Elétrica, Engenharia Civil, Engenharia Mecânica e Engenharia Química"),
                    new("Ciências Sociais Aplicadas", "Economia, Administração, Finanças, Contabilidade, Marketing e Serviço Social"),
                    new("Ciências da Saúde", "Medicina, Enfermagem, Odontologia, Farmácia, Fisioterapia, Nutrição"),
                    new("Ciências Biológicas", "Biologia, Ecologia, Botânica, Zoologia, Genética e Microbiologia"),
                };
                foreach (var responsibilityArea in responsibilityAreas)
                {
                    responsibilityArea.CreatedAt = DateTime.Now;
                    Add(responsibilityArea);
                }

                await CommitAsync();
            }

            var hasResponsibilities = Any<Responsibility>();
            if (!hasResponsibilities)
            {
                var responsibilities = new List<Responsibility>
                {
                    new("Arquiteto de Software", ResponsibilityArea.TecnologyScience, "Define a arquitetura do sistema, incluindo a estrutura de módulos, padrões e tecnologias a serem utilizadas"),
                    new("Engenheiro DevOps", ResponsibilityArea.TecnologyScience, "Automatiza processos de desenvolvimento e operações, gerencia integração contínua e entrega contínua (CI/CD), e monitora a infraestrutura"),
                    new("Gerente de Projetos (TI)", ResponsibilityArea.TecnologyScience, "Planejamento, execução e controle de projetos de software. Gerencia cronogramas, orçamentos e coordena equipes"),
                    new("Desenvolvedor Back-end", ResponsibilityArea.TecnologyScience, "Desenvolve a lógica do lado do servidor, integra com bancos de dados e gerencia a comunicação com o front-end"),
                    new("Desenvolvedor Front-end", ResponsibilityArea.TecnologyScience, "Desenvolve a interface do usuário e a experiência do usuário em um site ou aplicativo"),
                    new("Quality Assurance (QA)", ResponsibilityArea.TecnologyScience, "Garantir a qualidade do software através de testes manuais e automatizados. Cria e executa planos de teste"),
                    new("Líder Técnico", ResponsibilityArea.TecnologyScience, "Fornece orientação técnica para a equipe de desenvolvimento. Resolve problemas técnicos e faz a ponte entre a equipe de desenvolvimento e os stakeholders"),
                    new("Agilista", ResponsibilityArea.TecnologyScience, "Facilita a aplicação de metodologias ágeis, como Scrum, e ajuda a equipe a remover impedimentos"),
                    new("Product Owner", ResponsibilityArea.TecnologyScience, "Define e prioriza os requisitos do produto com base nas necessidades do cliente e no valor para o negócio"),
                    new("Designer de UX/UI", ResponsibilityArea.TecnologyScience, "Design da experiência do usuário (UX) e interfaces do usuário (UI). Realiza pesquisas de usuários e testes de usabilidade"),
                    new("Desenvolvedor Full Stack", ResponsibilityArea.TecnologyScience, "Trabalha tanto no front-end quanto no back-end do desenvolvimento de software. Possui conhecimentos em todas as camadas da aplicação"),

                    /*new("Engenheiro de Dados", ResponsibilityArea.TecnologyScience),
                    new("Especialista em Segurança da Informação", ResponsibilityArea.TecnologyScience),
                    new("Analista de Suporte", ResponsibilityArea.TecnologyScience),
                    new("Engenheiro de Manutenção", ResponsibilityArea.TecnologyScience),*/
                };
                foreach (var responsibility in responsibilities)
                {
                    Add(responsibility);
                }

                await CommitAsync();
            }

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
                        Linkedin = "lucasfogliarini",
                        Locations =
                        [
                            new Location("Bora Work","bora.work") { IsHome = true },
                            new Location("💻 Bora Discord","💻 Bora Discord https://discord.gg/Yf4TCsSTG5") { IsHome = true },
                        ]
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
                    account.Locations ??= [];
                    var wa = new Location("📱 WhatsApp");
                    account.Locations.Add(wa);
                    var gMeet = new Location("💻 Google Meet", "meet");
                    account.Locations.Add(gMeet);
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
