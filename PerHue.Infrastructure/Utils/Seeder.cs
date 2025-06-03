using Microsoft.IdentityModel.Protocols;
using PerHue.Domain.Entities;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Utils
{
	public class Seeder(PerHueDbContext dbContext)
	{
		public async Task Seed()
		{
			if (await dbContext.Database.CanConnectAsync())
			{
				if (!dbContext.Roles.Any())
				{
					var roles = GetPrimaryRoles();
					dbContext.Roles.AddRange(roles);
					await dbContext.SaveChangesAsync();
				}
				if (!dbContext.ServicePackages.Any())
				{
					var servicePackages = GetPrimaryServicePackages();
					dbContext.ServicePackages.AddRange(servicePackages);
					await dbContext.SaveChangesAsync();
				}
				if (!dbContext.UserAccounts.Any())
				{
					var users = GetPrimaryUser();
					dbContext.UserAccounts.AddRange(users);
					await dbContext.SaveChangesAsync();
				}
				if (!dbContext.ColorTypes.Any())
				{
					var colorTypes = GetColorTypes();
					dbContext.ColorTypes.AddRange(colorTypes);
					await dbContext.SaveChangesAsync();
				}
				if (!dbContext.Colors.Any())
				{
					var colors = GetColors();
					dbContext.Colors.AddRange(colors);
					await dbContext.SaveChangesAsync();
				}
			}
		}
		private IEnumerable<Role> GetPrimaryRoles()
		{
			return new List<Role>
			{
				new() { Name = "Admin" },
				new() { Name = "Staff" },
				new() { Name = "User" },
				new() { Name = "Expert" },
				new() { Name = "Brand" },
			};
		}

		private IEnumerable<ServicePackage> GetPrimaryServicePackages()
		{
			return new List<ServicePackage>
			{
				new() { Name = "Free", Price = 0, Description = "Free service package", Duration = 0 },
				new() { Name = "OneTime", Price = 2000, Description = "AI-powered detailed color analysis\r\n\r\nOne-time AR try-on (makeup/outfits)\r\n\r\nEnhanced styling recommendations", Duration = 30 },
				new() { Name = "Premium", Price = 4000, Description = "Unlimited AI color analysis & AR/VR try-ons\r\n\r\nPersonalized styling for seasons & events\r\n\r\nAd-free & exclusive brand offers\r\n\r\nMonthly 1-on-1 session with a top stylist or beauty expert", Duration = 30 },
				new() { Name = "Premium", Price = 10000, Description = "Unlimited AI color analysis & AR/VR try-ons\r\n\r\nPersonalized styling for seasons & events\r\n\r\nAd-free & exclusive brand offers\r\n\r\nMonthly 1-on-1 session with a top stylist or beauty expert", Duration = 365 },
				new() { Name = "Partnership", Price = 150000, Description = "AI-Powered Matching\r\n\r\nSmart Filtering\r\n\r\nReal-Time Updates\r\n\r\nReal-Time Metrics\r\n\r\nCustomer Insights", Duration = 30 }
			};
		}

		private IEnumerable<UserAccount> GetPrimaryUser()
		{
			return new List<UserAccount>{
				new(){
					Email = "mentorlinkadmin@gmail.com",
					Username = "mentorlinkadmin",
					Password = "Admin12345!",
					Fullname = "Ad Thi Min",
					Phone = "0378661398",
					Gender = false,
					Dob = new DateOnly(2000, 1, 1),
					ProfilePicture = string.Empty,
					IsActive = true,
					IsAitested = false,
					RoleId = 1,
				},
				new()
				{
					Email = "trongldse173125@fpt.edu.vn",
					Username = "trongldse173125",
					Password = "Trong12345!",
					Fullname = "Le Duc Trong",
					Phone = "0378661398",
					Gender = true,
					Dob = new DateOnly(2003, 6, 16),
					ProfilePicture = string.Empty,
					IsActive = true,
					IsAitested = false,
					RoleId = 2,
				},
				new()
				{
					Email = "tienbhse172562@fpt.edu.vn",
					Username = "tienbhse172562",
					Password = "Tien12345!",
					Fullname = "Bui Huu Tien",
					Phone = "0345678910",
					Gender = true,
					Dob = new DateOnly(2003, 1, 11),
					ProfilePicture = string.Empty,
					IsActive = true,
					IsAitested = false,
					RoleId = 2,
				},
				new()
				{
					Email = "duyenntpse172534@fpt.edu.vn",
					Username = "duyenntpse172534",
					Password = "Duyen12345!",
					Fullname = "Nguyen Tram Phuc Duyen",
					Phone = "0345678910",
					Gender = false,
					Dob = new DateOnly(2003, 1, 11),
					ProfilePicture = string.Empty,
					IsActive = true,
					IsAitested = false,
					RoleId = 2,
				},

			};
		}

		private IEnumerable<ColorType> GetColorTypes()
		{
			return new List<ColorType>
			{
				new() { Name = "Cool Winter" },
				new() { Name = "Deep Winter" },
				new() { Name = "Clear Winter" },
				new() { Name = "Cool Summer" },
				new() { Name = "Soft Summer" },
				new() { Name = "Light Summer" },
				new() { Name = "Warm Autumn" },
				new() { Name = "Soft Autumn" },
				new() { Name = "Deep Autumn" },
				new() { Name = "Warm Spring" },
				new() { Name = "Light Spring" },
				new() { Name = "Clear Spring" }
			};
		}

		private IEnumerable<Color> GetColors()
		{
			return new List<Color>
			{
				new() { Name = "Fresh Guacamole", HexCode = "#AEBE89" },
				new() { Name = "Aloe Cream", HexCode = "#DAE3BB" },
				new() { Name = "White Lace", HexCode = "#FFF7EC" },
				new() { Name = "Brook Green", HexCode = "#A8EAD5" },
				new() { Name = "Earthy Cane", HexCode = "#C5B08B" },
				new() { Name = "Caraway Seeds", HexCode = "#DED5BC" },
				new() { Name = "Swan White", HexCode = "#F8F3E6" },
				new() { Name = "Frosty Pine", HexCode = "#C6CEBE" },
				new() { Name = "Aurora Red", HexCode = "#850121" },
				new() { Name = "Incarnadine", HexCode = "#B1002A" },
				new() { Name = "Sizzling Sunset", HexCode = "#EC7A49" },
				new() { Name = "Dark Scarlet Red", HexCode = "#800733" },
				new() { Name = "Frozen Wave", HexCode = "#52A7CC" },
				new() { Name = "Gossamer Pink", HexCode = "#FBC7C3" },
				new() { Name = "Rustic Cream", HexCode = "#F6EFE3" },
				new() { Name = "Rich Honey", HexCode = "#FABB7C" },
				new() { Name = "Christmas Eve", HexCode = "#15192F" },
				new() { Name = "Christmas Red", HexCode = "#B11E31" },
				new() { Name = "Christmas Vanilla Cake", HexCode = "#FAF2D1" },
				new() { Name = "Christmas Tree", HexCode = "#096344" },
				new() { Name = "Crystal Rose", HexCode = "#FCC4C9" },
				new() { Name = "Backlight", HexCode = "#FDF6F0" },
				new() { Name = "Sandy Beach", HexCode = "#F8E2CF" },
				new() { Name = "Desert Sand", HexCode = "#F5C6AA" },
				new() { Name = "Deep Azure", HexCode = "#415A80" },
				new() { Name = "Midwinter Mist", HexCode = "#A5D4DC" },
				new() { Name = "Snowbelt", HexCode = "#F2F4F8" },
				new() { Name = "Early Frost", HexCode = "#D7E2E9" },
				new() { Name = "Cosmic Sky", HexCode = "#A9A9C4" },
				new() { Name = "Hailstorm", HexCode = "#D0D1E1" },
				new() { Name = "Bright Grey", HexCode = "#EBECEF" },
				new() { Name = "Purple Amethyst", HexCode = "#908DB9" },
				new() { Name = "Coral Cove", HexCode = "#E0A39C" },
				new() { Name = "Berrie Popsicle", HexCode = "#D4A6D1" },
				new() { Name = "Pink Frosting", HexCode = "#F7D9E1" },
				new() { Name = "Soft Breeze", HexCode = "#FBF8F6" },
				new() { Name = "Nordic Breeze", HexCode = "#D3DDE6" },
				new() { Name = "Emerald Wave", HexCode = "#52ADA2" },
				new() { Name = "Baby Powder", HexCode = "#F7F8F3" },
				new() { Name = "Stem Green", HexCode = "#A7E087" },
				new() { Name = "Wormwood Green", HexCode = "#9DB09C" },
				new() { Name = "Snowflake", HexCode = "#EEF0F0" },
				new() { Name = "Wayward Willow", HexCode = "#D6D9D0" },
				new() { Name = "Hazel Gaze", HexCode = "#B7BDB0" },
				new() { Name = "Glacial Green", HexCode = "#6EB5A5" },
				new() { Name = "Lychee Pulp", HexCode = "#F9F4DB" },
				new() { Name = "Caramelized Pears", HexCode = "#E7D6AC" },
				new() { Name = "Vintage Red", HexCode = "#A13842" },
				new() { Name = "Hurricane Haze", HexCode = "#BDBBAD" },
				new() { Name = "Alpine Frost", HexCode = "#E0DED2" },
				new() { Name = "Milk Grass", HexCode = "#FAF8F0" },
				new() { Name = "Winter Frost", HexCode = "#E4DFCF" },
				new() { Name = "Purple Kiss", HexCode = "#D55F8F" },
				new() { Name = "Dark Desire Rose", HexCode = "#F9C4BA" },
				new() { Name = "Snowpink", HexCode = "#FADFDE" },
				new() { Name = "Cupcake Rose", HexCode = "#F1C5C2" },
				new() { Name = "Wood Bark", HexCode = "#4D3211" },
				new() { Name = "Creamy Mushroom", HexCode = "#C28C2E" },
				new() { Name = "Dry Maple Leaf", HexCode = "#D8CDB9" },
				new() { Name = "Wild Brown", HexCode = "#721B29" },
				new() { Name = "Rose Taupe", HexCode = "#B8223C" },
				new() { Name = "Auburn Wave", HexCode = "#F3B062" },
				new() { Name = "Aria Ivory", HexCode = "#90772F" },
				new() { Name = "Veronese Peach", HexCode = "#402320" },
				new() { Name = "Nyctophile Blue", HexCode = "#CAA6DB" },
				new() { Name = "Blue Dacnis", HexCode = "#FBC7C3" },
				new() { Name = "Chilly White", HexCode = "#F7F4E7" },
				new() { Name = "Morning Breeze", HexCode = "#B6DF82" },
				new() { Name = "Black Safflower", HexCode = "#721B29" },
				new() { Name = "Chanterelle", HexCode = "#9D653F" },
				new() { Name = "Wild Oats", HexCode = "#DFD8CB" },
				new() { Name = "Sweet Cherry Red", HexCode = "#22314A" },
				new() { Name = "Rosewood", HexCode = "#B33534" },
				new() { Name = "Raw Sunset", HexCode = "#E3E8CD" },
				new() { Name = "Anemone White", HexCode = "#FAFCEE" },
				new() { Name = "Novelle Peach", HexCode = "#A1DD70" },
				new() { Name = "Heilroom Rose", HexCode = "#464646" },
				new() { Name = "Tropical Peach", HexCode = "#C4C4BC" },
				new() { Name = "Cosmic Latte", HexCode = "#F4F4F4" },
				new() { Name = "Magic Mint", HexCode = "#DEDAD1" },
				new() { Name = "Calestra Grey", HexCode = "#13A699" },
				new() { Name = "Offshore Mint", HexCode = "#FFD708" },
				new() { Name = "Frosted Glass", HexCode = "#FFF7ED" },
				new() { Name = "Rhytnmic Blue", HexCode = "#AAF0D1" },
				new() { Name = "Great Canyon", HexCode = "#C7C6AB" },
				new() { Name = "Cashew Delight", HexCode = "#F9F5F0" },
				new() { Name = "Dulce De Leche", HexCode = "#F7E1D8" },
				new() { Name = "Cocoa Dust", HexCode = "#EBCCB9" },
				new() { Name = "Red Wrath", HexCode = "#D11C5B" },
				new() { Name = "Winter’s Day", HexCode = "#F6EED5" },
				new() { Name = "Sky of the Ocean", HexCode = "#FAC600" },
				new() { Name = "Night Dive", HexCode = "#EA4B1D" },
				new() { Name = "Frozen Wave", HexCode = "#FDC2B1" },
				new() { Name = "Midwinter Mist", HexCode = "#FDF8E8" },
				new() { Name = "Snowbelt", HexCode = "#AAF0D1" },
				new() { Name = "Dreamy Cloud", HexCode = "#9CADB3" },
				new() { Name = "Pink Nectar", HexCode = "#CFDADA" },
				new() { Name = "Pale Rose", HexCode = "#EBF2F2" },
				new() { Name = "Milk Grass", HexCode = "#BBD5D8" },
				new() { Name = "Bonfire", HexCode = "#CB6347" },
				new() { Name = "Golden Rambler", HexCode = "#DFCBB2" },
				new() { Name = "Apple White", HexCode = "#CC9972" },
				new() { Name = "Moorland Mist", HexCode = "#8C533D" },
				new() { Name = "Honey Butter", HexCode = "#B8223C" },
				new() { Name = "French Toast", HexCode = "#F3B062" },
				new() { Name = "Tomato Soup", HexCode = "#90772F" },
				new() { Name = "Tomato Sauce", HexCode = "#402320" },
				new() { Name = "Chocolate Eclair", HexCode = "#CAA6DB" },
				new() { Name = "Dark Marmalade", HexCode = "#FBC7C3" },
				new() { Name = "Toasted Barley", HexCode = "#F7F4E7" },
				new() { Name = "Flakes", HexCode = "#B6DF82" },
				new() { Name = "Gingerbread Latte", HexCode = "#721B29" },
				new() { Name = "Pinkish Grey", HexCode = "#9D653F" },
				new() { Name = "Grim White", HexCode = "#DFD8CB" },
				new() { Name = "Shy Pink", HexCode = "#22314A" },
				new() { Name = "Blush Grey Rose", HexCode = "#F4F4F4" },
				new() { Name = "Pepper Jelly", HexCode = "#E0E0E0" },
				new() { Name = "Blazing Autumn", HexCode = "#C4C6C8" },
				new() { Name = "Rustic Cream", HexCode = "#BEBCCB" },
				new() { Name = "Green Daze", HexCode = "#F1EFF2" },
				new() { Name = "Majestic Mist", HexCode = "#DDD7DC" },
				new() { Name = "Naturally Calm", HexCode = "#A994A7" },
				new() { Name = "Bright Grey", HexCode = "#F1E1C9" },
				new() { Name = "Ash Violet", HexCode = "#FAF8F0" },
				new() { Name = "Marple Red", HexCode = "#CFECF5" },
				new() { Name = "Pumpkin Latte", HexCode = "#A9E2F5" },
				new() { Name = "Dying Leaves", HexCode = "#182F53" },
				new() { Name = "Dark Oak", HexCode = "#F9EEE2" },
				new() { Name = "Wisteria", HexCode = "#F57A4D" },
				new() { Name = "Gossamer Pink", HexCode = "#AEBE89" },
				new() { Name = "Risotto", HexCode = "#444251" },
				new() { Name = "Green Chalk", HexCode = "#8D89A3" },
				new() { Name = "Red Berry", HexCode = "#E4E3E5" },
				new() { Name = "Cinnamon Baked", HexCode = "#CBCBCF" },
				new() { Name = "Powder Cake", HexCode = "#F5CDE6" },
				new() { Name = "Blueberry Bark", HexCode = "#FAF8F0" }
			};
		} 
	}
}
