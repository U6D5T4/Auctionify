using Auctionify.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Auctionify.Infrastructure.Persistence
{
    public class ApplicationDbContextInitializer
    {
        private readonly ILogger<ApplicationDbContextInitializer> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
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
                if (_roleManager.Roles.All(r => r.Name != role.Name))
                {
                    await _roleManager.CreateAsync(role);
                }
            }

            // Default users
            var users = new List<User>
            {
                new User { UserName = "admin@localhost.com", Email = "admin@localhost.com", EmailConfirmed = true },
                new User { UserName = "buyer@localhost.com", Email = "buyer@localhost.com", EmailConfirmed = true },
                new User { UserName = "seller@localhost.com", Email = "seller@localhost.com", EmailConfirmed = true  },
            };

            foreach(var user in users)
            {
                if (_userManager.Users.All(u => u.UserName != user.UserName))
                {
                    await _userManager.CreateAsync(user, "Test123!");
                    switch (user.UserName) {
                        case "admin@localhost.com":
                            if (roles is null) return;
                            var adminRole = roles.FirstOrDefault(r => r.Name == "Administrator");
                            if (adminRole != null)
                            {
                                await _userManager.AddToRolesAsync(user, new List<string> { adminRole.Name });
                            }
                            break;
                        case "buyer@localhost.com":
                            if (roles is null) return;
                            var buyerRole = roles.FirstOrDefault(r => r.Name == "Buyer");
                            if (buyerRole != null)
                            {
                                await _userManager.AddToRolesAsync(user, new List<string> { buyerRole.Name });
                            }
                            break;
                        case "seller@localhost.com":
                            if (roles is null) return;
                            var sellerRole = roles.FirstOrDefault(r => r.Name == "Seller");
                            if (sellerRole != null)
                            {
                                await _userManager.AddToRolesAsync(user, new List<string> { sellerRole.Name });
                            }
                            break;
                    }
                }
            }

            if(!_context.Categories.Any())
            {
                var foodCategory = _context.Categories.Add(new Category
                {
                    Name = "Food",
                });

                await _context.SaveChangesAsync();

                _context.Categories.Add(new Category
                {
                    Name = "Meat",
                    ParentCategoryId = foodCategory.Entity.Id
                });
            }

            if (!_context.LotStatuses.Any())
            {
                _context.LotStatuses.AddRange(new LotStatus[]
                {
                    new LotStatus
                    {
                        Name = "Draft"
                    },
                    new LotStatus
                    {
                        Name = "Upcoming"
                    },
                    new LotStatus
                    {
                        Name = "Active"
                    }
                });
            }

            if (!_context.Currency.Any())
            {
                _context.Currency.AddRange(new Currency[]
                {
                    new Currency
                    {
                        Code = "USD"
                    },
                    new Currency
                    {
                        Code = "RUB"
                    }
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
