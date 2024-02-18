using Auctionify.Core.Entities;
using Auctionify.Infrastructure.Common.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Auctionify.Infrastructure.Persistence
{
	public class ApplicationDbContextInitializer
	{
		private readonly ILogger<ApplicationDbContextInitializer> _logger;
		private readonly ApplicationDbContext _context;
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<Role> _roleManager;
		private readonly UsersSeedingData _usersData;

		public ApplicationDbContextInitializer(
			ILogger<ApplicationDbContextInitializer> logger,
			ApplicationDbContext context,
			UserManager<User> userManager,
			RoleManager<Role> roleManager,
			IOptions<UsersSeedingData> usersData
		)
		{
			_logger = logger;
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
			_usersData = usersData.Value;
		}

		public async Task InitialiseAsync()
		{
			try
			{
				if (_context.Database.IsSqlServer())
				{
					await _context.Database.MigrateAsync();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while initialising the database.");
				throw;
			}
		}

		public async Task SeedAsync()
		{
			try
			{
				await TrySeedAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while seeding the database.");
				throw;
			}
		}

		public async Task TrySeedAsync()
		{
			// Default roles
			var roles = new List<Role>
			{
				new Role { Name = "Administrator", },
				new Role { Name = "Seller", },
				new Role { Name = "Buyer", }
			};

			foreach (var role in roles)
			{
				if (!await _roleManager.RoleExistsAsync(role.Name!))
				{
					await _roleManager.CreateAsync(role);
				}
			}

			// Default users
			var users = new List<User>
			{
				new User
				{
					UserName = _usersData.Emails.Admin,
					Email = _usersData.Emails.Admin,
					EmailConfirmed = true
				},
				new User
				{
					UserName = _usersData.Emails.Buyer,
					Email = _usersData.Emails.Buyer,
					EmailConfirmed = true
				},
				new User
				{
					UserName = _usersData.Emails.Seller,
					Email = _usersData.Emails.Seller,
					EmailConfirmed = true
				},
			};

			foreach (var user in users)
			{
				if ((await _userManager.FindByNameAsync(user.UserName!) is null))
				{
					await _userManager.CreateAsync(user, "Test123!");
					switch (user.UserName)
					{
						case "admin@localhost.com":
							var adminRole = roles.Find(r => r.Name == "Administrator");
							if (adminRole != null)
							{
								await _userManager.AddToRolesAsync(
									user,
									new List<string> { adminRole.Name! }
								);
							}
							break;
						case "buyer@localhost.com":
							var buyerRole = roles.Find(r => r.Name == "Buyer");
							if (buyerRole != null)
							{
								await _userManager.AddToRolesAsync(
									user,
									new List<string> { buyerRole.Name! }
								);
							}
							break;
						case "seller@localhost.com":
							var sellerRole = roles.Find(r => r.Name == "Seller");
							if (sellerRole != null)
							{
								await _userManager.AddToRolesAsync(
									user,
									new List<string> { sellerRole.Name! }
								);
							}
							break;
					}
				}
			}

			if (!_context.Categories.Any())
			{
				var electronicsCategory = _context.Categories.Add(
					new Category { Name = "Electronics" }
				);

				var furnitureCategory = _context.Categories.Add(
					new Category { Name = "Furniture" }
				);

				var collectiblesCategory = _context.Categories.Add(
					new Category { Name = "Collectibles" }
				);

				var artCategory = _context.Categories.Add(new Category { Name = "Art" });

				var musicCategory = _context.Categories.Add(new Category { Name = "Music" });

				var jewelryCategory = _context.Categories.Add(new Category { Name = "Jewelry" });

				var carsCategory = _context.Categories.Add(new Category { Name = "Cars" });

				await _context.SaveChangesAsync();

				_context.Categories.AddRange(
					new Category
					{
						Name = "Motorcycles",
						ParentCategoryId = carsCategory.Entity.Id
					},
					new Category { Name = "Trucks", ParentCategoryId = carsCategory.Entity.Id },
					new Category { Name = "Boats", ParentCategoryId = carsCategory.Entity.Id },
					new Category { Name = "Aircraft", ParentCategoryId = carsCategory.Entity.Id },
					new Category { Name = "RVs", ParentCategoryId = carsCategory.Entity.Id },
					new Category
					{
						Name = "Commercial Vehicles",
						ParentCategoryId = carsCategory.Entity.Id
					},
					new Category { Name = "Trailers", ParentCategoryId = carsCategory.Entity.Id },
					new Category
					{
						Name = "Classic Cars",
						ParentCategoryId = carsCategory.Entity.Id
					},
					new Category
					{
						Name = "Modern Cars",
						ParentCategoryId = carsCategory.Entity.Id
					},
					new Category
					{
						Name = "Collectible Cars",
						ParentCategoryId = carsCategory.Entity.Id
					},
					new Category
					{
						Name = "Other Vehicles",
						ParentCategoryId = carsCategory.Entity.Id
					}
				);

				_context.Categories.AddRange(
					new Category
					{
						Name = "Mobile Phones",
						ParentCategoryId = electronicsCategory.Entity.Id
					},
					new Category
					{
						Name = "Computers",
						ParentCategoryId = electronicsCategory.Entity.Id
					},
					new Category
					{
						Name = "Cameras",
						ParentCategoryId = electronicsCategory.Entity.Id
					},
					new Category
					{
						Name = "Audio Devices",
						ParentCategoryId = electronicsCategory.Entity.Id
					},
					new Category
					{
						Name = "Kitchen Appliances",
						ParentCategoryId = electronicsCategory.Entity.Id
					},
					new Category
					{
						Name = "Smart Home Devices",
						ParentCategoryId = electronicsCategory.Entity.Id
					},
					new Category
					{
						Name = "Video Games",
						ParentCategoryId = electronicsCategory.Entity.Id
					},
					new Category { Name = "TVs", ParentCategoryId = electronicsCategory.Entity.Id },
					new Category
					{
						Name = "Printers",
						ParentCategoryId = electronicsCategory.Entity.Id
					}
				);

				_context.Categories.AddRange(
					new Category
					{
						Name = "Tables",
						ParentCategoryId = furnitureCategory.Entity.Id
					},
					new Category
					{
						Name = "Wardrobes",
						ParentCategoryId = furnitureCategory.Entity.Id
					},
					new Category
					{
						Name = "Office Chairs",
						ParentCategoryId = furnitureCategory.Entity.Id
					},
					new Category { Name = "Sofas", ParentCategoryId = furnitureCategory.Entity.Id },
					new Category
					{
						Name = "Dining Tables",
						ParentCategoryId = furnitureCategory.Entity.Id
					},
					new Category
					{
						Name = "Bedroom Furniture",
						ParentCategoryId = furnitureCategory.Entity.Id
					}
				);

				_context.AddRange(
					new Category
					{
						Name = "Trading Cards",
						ParentCategoryId = collectiblesCategory.Entity.Id
					},
					new Category
					{
						Name = "Toys",
						ParentCategoryId = collectiblesCategory.Entity.Id
					},
					new Category
					{
						Name = "Action Figures",
						ParentCategoryId = collectiblesCategory.Entity.Id
					},
					new Category
					{
						Name = "Pins",
						ParentCategoryId = collectiblesCategory.Entity.Id
					},
					new Category
					{
						Name = "Key Chains",
						ParentCategoryId = collectiblesCategory.Entity.Id
					},
					new Category
					{
						Name = "Model Cars",
						ParentCategoryId = collectiblesCategory.Entity.Id
					},
					new Category
					{
						Name = "Rare Books",
						ParentCategoryId = collectiblesCategory.Entity.Id
					},
					new Category
					{
						Name = "Comic Books",
						ParentCategoryId = collectiblesCategory.Entity.Id
					},
					new Category
					{
						Name = "Stamps",
						ParentCategoryId = collectiblesCategory.Entity.Id
					},
					new Category
					{
						Name = "Coins and Currency",
						ParentCategoryId = collectiblesCategory.Entity.Id
					}
				);

				_context.Categories.AddRange(
					new Category { Name = "Paintings", ParentCategoryId = artCategory.Entity.Id },
					new Category { Name = "Sculptures", ParentCategoryId = artCategory.Entity.Id },
					new Category { Name = "Books", ParentCategoryId = artCategory.Entity.Id },
					new Category { Name = "Comics", ParentCategoryId = artCategory.Entity.Id },
					new Category { Name = "Photography", ParentCategoryId = artCategory.Entity.Id },
					new Category { Name = "Posters", ParentCategoryId = artCategory.Entity.Id }
				);

				_context.Categories.AddRange(
					new Category { Name = "Guitars", ParentCategoryId = musicCategory.Entity.Id },
					new Category
					{
						Name = "Bass Guitars",
						ParentCategoryId = musicCategory.Entity.Id
					},
					new Category { Name = "Drums", ParentCategoryId = musicCategory.Entity.Id },
					new Category { Name = "Pianos", ParentCategoryId = musicCategory.Entity.Id },
					new Category { Name = "Keyboards", ParentCategoryId = musicCategory.Entity.Id },
					new Category
					{
						Name = "Synthesizers",
						ParentCategoryId = musicCategory.Entity.Id
					},
					new Category
					{
						Name = "DJ Equipment",
						ParentCategoryId = musicCategory.Entity.Id
					},
					new Category
					{
						Name = "Microphones",
						ParentCategoryId = musicCategory.Entity.Id
					},
					new Category
					{
						Name = "Music Accessories",
						ParentCategoryId = musicCategory.Entity.Id
					},
					new Category
					{
						Name = "Music Instruments",
						ParentCategoryId = musicCategory.Entity.Id
					},
					new Category
					{
						Name = "Vinyl Records",
						ParentCategoryId = musicCategory.Entity.Id
					},
					new Category { Name = "CDs", ParentCategoryId = musicCategory.Entity.Id },
					new Category { Name = "Cassettes", ParentCategoryId = musicCategory.Entity.Id },
					new Category
					{
						Name = "8-Track Tapes",
						ParentCategoryId = musicCategory.Entity.Id
					},
					new Category
					{
						Name = "Reel-to-Reel Tapes",
						ParentCategoryId = musicCategory.Entity.Id
					}
				);

				_context.Categories.AddRange(
					new Category { Name = "Rings", ParentCategoryId = jewelryCategory.Entity.Id },
					new Category
					{
						Name = "Necklaces",
						ParentCategoryId = jewelryCategory.Entity.Id
					},
					new Category
					{
						Name = "Bracelets",
						ParentCategoryId = jewelryCategory.Entity.Id
					},
					new Category
					{
						Name = "Earrings",
						ParentCategoryId = jewelryCategory.Entity.Id
					},
					new Category
					{
						Name = "Brooches",
						ParentCategoryId = jewelryCategory.Entity.Id
					},
					new Category
					{
						Name = "Pendants",
						ParentCategoryId = jewelryCategory.Entity.Id
					},
					new Category { Name = "Watches", ParentCategoryId = jewelryCategory.Entity.Id }
				);
			}

			if (!_context.LotStatuses.Any())
			{
				_context.LotStatuses.AddRange(
					new LotStatus { Name = "Draft" },
					new LotStatus { Name = "PendingApproval" },
					new LotStatus { Name = "Rejected" },
					new LotStatus { Name = "Upcoming" },
					new LotStatus { Name = "Active" },
					new LotStatus { Name = "Sold" },
					new LotStatus { Name = "NotSold" },
					new LotStatus { Name = "Cancelled" },
					new LotStatus { Name = "Reopened" },
					new LotStatus { Name = "Archive" }
				);
			}

			if (!_context.Currency.Any())
			{
				_context.Currency.AddRange(
					new Currency { Code = "USD" },
					new Currency { Code = "RUB" }
				);
			}

			if (!_context.Locations.Any())
			{
				_context.Locations.AddRange(
					new Location
					{
						City = "New York",
						State = "NY",
						Country = "USA",
						Address = "1234 Elm St"
					},
					new Location
					{
						City = "Los Angeles",
						State = "CA",
						Country = "USA",
						Address = "5678 Oak Ave"
					},
					new Location
					{
						City = "London",
						Country = "UK",
						Address = "90 Baker St"
					},
					new Location
					{
						City = "Moscow",
						Country = "Russia",
						Address = "1 Red Square"
					},
					new Location
					{
						City = "Paris",
						Country = "France",
						Address = "2 Champs Elysees"
					},
					new Location
					{
						City = "Berlin",
						Country = "Germany",
						Address = "3 Unter den Linden"
					},
					new Location
					{
						City = "Tokyo",
						Country = "Japan",
						Address = "4 Ginza"
					},
					new Location
					{
						City = "Tashkent",
						Country = "Uzbekistan",
						Address = "5 Amir Temur"
					},
					new Location
					{
						City = "Sydney",
						Country = "Australia",
						Address = "6 George St"
					}
				);

				await _context.SaveChangesAsync();
			}

			if (!_context.Lots.Any())
			{
				_context.Database.ExecuteSqlRaw(
					@"
                    INSERT INTO [dbo].[Lots]
                       ([SellerId]
                       ,[CategoryId]
                       ,[LotStatusId]
                       ,[LocationId]
                       ,[CurrencyId]
                       ,[Title]
                       ,[Description]
                       ,[StartingPrice]
                       ,[StartDate]
                       ,[EndDate]
                       ,[CreationDate]
                       ,[ModificationDate])
                 VALUES
                       (3, 1, 4, 1, 1, 'Sample Lot 1', 'This is a sample lot description for Lot 1.', 100.00, '2023-10-12 10:00:00', '2023-10-15 15:00:00', '2023-10-12 09:00:00', '2023-10-12 09:00:00'),
                       (3, 2, 5, 2, 1, 'Sample Lot 2', 'This is a sample lot description for Lot 2.', 150.00, '2023-10-13 11:00:00', '2023-10-16 16:00:00', '2023-10-13 10:00:00', '2023-10-13 10:00:00'),
                       (3, 2, 4, 3, 1, 'Sample Lot 3', 'This is a sample lot description for Lot 3.', 200.00, '2023-10-14 12:00:00', '2023-10-17 17:00:00', '2023-10-14 11:00:00', '2023-10-14 11:00:00')
                     "
				);

				await _context.SaveChangesAsync();
			}

			if (!_context.SubscriptionTypes.Any())
			{
				_context.SubscriptionTypes.Add(new SubscriptionType
				{
					Name = "Pro"
				});

				await _context.SaveChangesAsync();
			}

			await _context.SaveChangesAsync();
		}
	}
}
