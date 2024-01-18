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

				await _context.SaveChangesAsync();

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
					new Category
					{
						Name = "Sofas",
						ParentCategoryId = furnitureCategory.Entity.Id
					},
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

				_context.Categories.AddRange(
					new Category
					{
						Name = "Antiques",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Collectibles",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Art",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Jewelry",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Watches",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Coins and Currency",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Vintage Clothing",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Automobilia",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Memorabilia",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Rare Books",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Stamps",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Sports Memorabilia",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Comic Books",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Trading Cards",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Toys",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Music Instruments",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Music Records Vinyl",
						ParentCategoryId = null
					},
					new Category
					{
						Name = "Wine",
						ParentCategoryId = null
					}
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
                       ,[RateId]
                       ,[CreationDate]
                       ,[ModificationDate])
                 VALUES
                       (3, 1, 1, 1, 1, 'Sample Lot 1', 'This is a sample lot description for Lot 1.', 100.00, '2023-10-12 10:00:00', '2023-10-15 15:00:00', 6, '2023-10-12 09:00:00', '2023-10-12 09:00:00'),
                       (3, 2, 5, 2, 1, 'Sample Lot 2', 'This is a sample lot description for Lot 2.', 150.00, '2023-10-13 11:00:00', '2023-10-16 16:00:00', 7, '2023-10-13 10:00:00', '2023-10-13 10:00:00'),
                       (3, 2, 1, 3, 1, 'Sample Lot 3', 'This is a sample lot description for Lot 3.', 200.00, '2023-10-14 12:00:00', '2023-10-17 17:00:00', 8, '2023-10-14 11:00:00', '2023-10-14 11:00:00')
                     "
				);

				await _context.SaveChangesAsync();
			}
		}
	}
}
