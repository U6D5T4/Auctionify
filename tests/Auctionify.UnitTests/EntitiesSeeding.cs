using Auctionify.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Auctionify.UnitTests
{
	public static class EntitiesSeeding
	{
		public static List<Lot> GetLots()
		{
			var bids = GetBids();
			var lotStatuses = GetLotStatuses();

			return new List<Lot>
			{
				new Lot
				{
					Id = 1,
					Title = "Test lot with size",
					Description =
						"Test lot with description and some moreeeeee DECSRIPTIOn mock data with long description and some other else",
					LotStatusId = 1,
					LotStatus = lotStatuses.Find(x => x.Id == 1)!,
					StartDate = DateTime.Now,
					EndDate = DateTime.Now.AddDays(1),
					Location = new Location
					{
						Address = "test address",
						City = "Test City",
						Country = "Test country",
					},
					StartingPrice = 100,
					SellerId = 1,
					CategoryId = 1,
					CurrencyId = 1,
					Bids = new List<Bid> { bids[0], bids[1], }
				},
				new Lot
				{
					Id = 2,
					Title = "Test lot with size",
					Description =
						"Test lot with description and some moreeeeee DECSRIPTIOn mock data with long description and some other else",
					LotStatusId = 1,
					LotStatus = lotStatuses.Find(x => x.Id == 3)!,
					StartDate = DateTime.Now,
					EndDate = DateTime.Now.AddDays(1),
					Location = new Location
					{
						Address = "test address",
						City = "Test City",
						Country = "Test country",
					},
					StartingPrice = 100,
					SellerId = 1,
					CategoryId = 1,
					CurrencyId = 1,
					Bids = new List<Bid> { }
				},
				new Lot
				{
					Id = 3,
					Title = "Test lot with size",
					Description =
						"Test lot with description and some moreeeeee DECSRIPTIOn mock data with long description and some other else",
					LotStatusId = 1,
					LotStatus = lotStatuses.Find(x => x.Id == 3)!,
					StartDate = DateTime.Now,
					EndDate = DateTime.Now.AddDays(1),
					Location = new Location
					{
						Address = "test address",
						City = "Test City",
						Country = "Test country",
					},
					StartingPrice = 100,
					SellerId = 1,
					CategoryId = 1,
					CurrencyId = 1,
				},
				new Lot
				{
					Id = 4,
					Title = "Test lot with size",
					Description =
						"Test lot with description and some moreeeeee DECSRIPTIOn mock data with long description and some other else",
					LotStatusId = 1,
					LotStatus = lotStatuses.Find(x => x.Id == 3)!,
					StartDate = DateTime.Now,
					EndDate = DateTime.Now.AddDays(1),
					Location = new Location
					{
						Address = "test address",
						City = "Test City",
						Country = "Test country",
					},
					StartingPrice = 100,
					SellerId = 2,
					CategoryId = 1,
					CurrencyId = 1,
				}
			};
		}

		public static List<Bid> GetBids()
		{
			return new List<Bid>
			{
				new Bid
				{
					LotId = 1,
					BuyerId = 2,
					NewPrice = 120,
					BidRemoved = false,
				},
				new Bid
				{
					LotId = 1,
					BuyerId = 3,
					NewPrice = 140,
					BidRemoved = false,
				},
				new Bid
				{
					LotId = 2,
					BuyerId = 3,
					NewPrice = 140,
					BidRemoved = false,
				}
			};
		}

		public static List<Rate> GetRates()
		{
			return new List<Rate>
			{
				new Rate
				{
					ReceiverId = 2,
					SenderId = 1,
					RatingValue = 5,
					Comment = "Some comment",
					LotId = 2
				},
				new Rate
				{
					ReceiverId = 1,
					SenderId = 3,
					RatingValue = 5,
					Comment = "Some comment 2",
					LotId = 1
				},
				new Rate
				{
					ReceiverId = 3,
					SenderId = 2,
					RatingValue = 5,
					Comment = "Some comment 3",
					LotId = 3
				}
			};
		}

		public static List<LotStatus> GetLotStatuses()
		{
			return new List<LotStatus>
			{
				new LotStatus { Id = 1, Name = "Active", },
				new LotStatus { Id = 2, Name = "Draft" },
				new LotStatus { Id = 3, Name = "Upcoming" },
				new LotStatus { Id = 4, Name = "Cancelled" },
				new LotStatus { Id = 5, Name = "Sold" },
				new LotStatus { Id = 6, Name = "NotSold" },
				new LotStatus { Id = 7, Name = "Archive" }
			};
		}

		public static List<Category> GetCategories()
		{
			return new List<Category>
			{
				new Category { Id = 1, },
				new Category { Id = 2, },
				new Category { Id = 3, }
			};
		}

		public static List<Currency> GetCurrencies()
		{
			return new List<Currency>
			{
				new Currency { Id = 1, },
				new Currency { Id = 2, },
				new Currency { Id = 3, },
			};
		}

		public static List<Watchlist> GetWatchlists()
		{
			return new List<Watchlist>
			{
				new Watchlist { Id = 1, },
				new Watchlist { Id = 2, },
				new Watchlist { Id = 3, },
			};
		}

		public static User GetUser()
		{
			return new User
			{
				Id = 1,
				UserName = "TestUserName",
				Email = "test@test.COM",
				EmailConfirmed = true,
				PhoneNumber = "123456789",
				FirstName = "TestFirstName",
				LastName = "TestLastName",
				AboutMe = "TestAboutMe",
				ProfilePicture = "TestProfilePicture.png",
			};
		}

		public static List<User> GetUsers()
		{
			return new List<User>
			{
				new User
				{
					Id = 1,
					UserName = "TestUserName",
					Email = "test@test.COM",
					EmailConfirmed = true,
					PhoneNumber = "123456789",
					FirstName = "TestFirstName",
					LastName = "TestLastName",
					AboutMe = "TestAboutMe",
					ProfilePicture = "TestProfilePicture.png",
				},
				new User
				{
					Id = 2,
					UserName = "TestUserName",
					Email = "test@test.COM",
					EmailConfirmed = true,
					PhoneNumber = "123456789",
					FirstName = "TestFirstName",
					LastName = "TestLastName",
					AboutMe = "TestAboutMe",
					ProfilePicture = "TestProfilePicture.png"
				},
				new User
				{
					Id = 3,
					UserName = "TestUserName",
					Email = "test@test.COM",
					EmailConfirmed = true,
					PhoneNumber = "123456789",
					FirstName = "TestFirstName",
					LastName = "TestLastName",
					AboutMe = "TestAboutMe",
					ProfilePicture = "TestProfilePicture.png"
				}
			};
		}

		public static List<Core.Entities.File> GetFiles()
		{
			return new List<Core.Entities.File>
			{
				new Core.Entities.File
				{
					Id = 1,
					LotId = 1,
					FileName = "Test name",
					Path = "TestPath/AndSomeOther/photos/Path"
				},
				new Core.Entities.File
				{
					Id = 2,
					LotId = 1,
					FileName = "Test name",
					Path = "TestPath/AndSomeOther/additional-documents/Path"
				},
			};
		}

		public static List<Location> GetLocations()
		{
			return new List<Location>
			{
				new Location
				{
					Id = 1,
					City = "Tashkent",
					Country = "Uzbekistan",
					Address = "Some address"
				},
				new Location
				{
					Id = 2,
					City = "Moscow",
					Country = "Russia Federation",
					Address = "Some address"
				},
				new Location
				{
					Id = 3,
					City = "London",
					Country = "UK",
					Address = "Some address"
				},
			};
		}

		public static UserManager<User> GetUserManagerMock()
		{
			var store = new Mock<IUserStore<User>>();

			var userManager = new Mock<UserManager<User>>(
				store.Object,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			);
			userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(GetUser());
			userManager.Object.UserValidators.Add(new UserValidator<User>());
			userManager.Object.PasswordValidators.Add(new PasswordValidator<User>());
			return userManager.Object;
		}
	}
}
