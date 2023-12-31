using Bora.Database;
using Bora.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bora.Repository.MSSQL
{
	public static class Seed
	{
		static async void MigrateAndSeed(BoraDbContext boraDbContext)
		{
			if (boraDbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
			{
				boraDbContext.Database.Migrate();
			}
			else
			{
				var accounts = new List<Account>
				{
					new Account("lucasfogliarini@gmail.com")
					{
						Name = "Lucas Fogliarini",
						Photo = "https://lh3.googleusercontent.com/a-/AOh14Ggingx4m5A-dFGLwEJv-acJ-KEDtApHCAO0NxfUig=s96-c",
						WhatsApp = "51992364249",
						Instagram = "lucasfogliarini",
						Spotify = "12145833562",
						CreatedAt = new DateTime(2022, 04, 01),
					},
					new Account("luanaleticiabueno@gmail.com")
					{
						Name = "Luana Bueno",
						WhatsApp = "5193840006",
						Instagram = "luanabuenoflores",
						Spotify = "224juavirzfsjsxt5yva6fvly",
						Photo= "https://lh3.googleusercontent.com/a-/AOh14GhWN-zhlu_93Me88oT9v8554pdaJQdNYKpUp-i__c0=s340-p-k-rw-no",
						CreatedAt = new DateTime(2022, 04, 01),
					},
					new Account("ricardoschieck@gmail.com")
					{
						Name = "Ricardo Schieck",
						Photo= "https://lh3.googleusercontent.com/a-/AOh14GiT4eOvOQI-vvBxGhxWLlRtteBMtXyICzAC1q45pQ=s96-c",
						CreatedAt = new DateTime(2022, 04, 30),
					},
					new Account("gui_staub@hotmail.com")
					{
						Name = "Guilherme Staub",
						Photo= "https://lh3.googleusercontent.com/a-/AOh14Gi14cQFSeyn5q6u3ZB_derhI7yIcA9dgX27OkBl=s96-c",
						CreatedAt = new DateTime(2022, 04, 30),
					},
					new Account("varreira.adv@gmail.com")
					{
						Name = "Anderson Varreira",
						Photo= "https://lh3.googleusercontent.com/a/AATXAJw-6J_C5vAh-d9Gp3ssN_ziJrOkzp6HMWXE6Ubm=s96-c",
						CreatedAt = new DateTime(2022, 05, 03),
					},
					new Account("lucasbuenomagalhaes@gmail.com")
					{
						Name = "Lucas Bueno",
						Photo= "https://lh3.googleusercontent.com/a-/AOh14Ggfyxso7uuqWxLMqvI3JTDOcKDRKkOgsz0oOwLWPw=s96-c",
						CreatedAt = new DateTime(2022, 05, 08),
					},
					new Account("rodrigoschieck.pro@gmail.com")
					{
						Name = "Rodrigo Schieck",
						Photo= "https://lh3.googleusercontent.com/a-/AOh14Gjikfm33H1HZqqwzSV10X1H1ZQGUuA5hqo15fY0Zw=s96-c",
						CreatedAt = new DateTime(2022, 06, 24),
					}
				};
				boraDbContext.AddRange(accounts);

				var homeContents = new List<Content>
				{
					new Content
					{
						Collection = "home",
						Key = "boraLink",
						Text = "/lucasfogliarini"
					},
					new Content
					{
						Collection = "home",
						Key = "boraText",
						Text = "Bora!"
					}
				};

				foreach (var homeContent in homeContents)
				{
					homeContent.CreatedAt = DateTime.Now;
					homeContent.AccountId = 1;//lucasfogliarini
				}

				boraDbContext.AddRange(homeContents);

				await boraDbContext.SaveChangesAsync();
			}
		}
	}
}
