using Microsoft.IdentityModel.Protocols;
using PerHue.Domain.Entities;
using PerHue.Infrastructure.Persistence;
using System.Collections.Generic;

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
				if (!dbContext.CapsulePalettes.Any())
				{
					var capsulePalettes = GetPrimaryCapsulePalettes();
					dbContext.CapsulePalettes.AddRange(capsulePalettes);
					await dbContext.SaveChangesAsync();
				}
			}
		}
		private IEnumerable<Role> GetPrimaryRoles()
		{
			return new List<Role>
			{
				new() { Name = "Admin" },
				new() { Name = "User" },
				new() { Name = "Expert" },
				new() { Name = "Brand" },
			};
		}

		private IEnumerable<ServicePackage> GetPrimaryServicePackages()
		{
			return new List<ServicePackage>
			{
				new() { Name = "Free", Price = 0, Description = " Basic personal color analysis based on skin, hair, and eye color.\r\n\r\nSimple outfit/cosmetic suggestions (no in-depth styling)\r\n\r\nLimited to 1-2 analyses per month\r\n\r\nLight adversting", Duration = 0 },
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
					Email = "buihuutien3@gmail.com",
					Username = "buihuutien3",
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
					RoleId = 3,
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
					RoleId = 4,
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

		private IEnumerable<Color> GetColors()
		{
			return new List<Color> {
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
				new() { Name = "Christmas Vanilla", HexCode = "#FAF2D1" },
				new() { Name = "Cake", HexCode = "#096344" },
				new() { Name = "Christmas Tree", HexCode = "#FCC4C9" },
				new() { Name = "Crystal Rose", HexCode = "#FDF6F0" },
				new() { Name = "Backlight", HexCode = "#F8E2CF" },
				new() { Name = "Sandy Beach", HexCode = "#F5C6AA" },
				new() { Name = "Desert Sand", HexCode = "#415A80" },
				new() { Name = "Deep Azure", HexCode = "#A5D4DC" },
				new() { Name = "Midwinter Mist", HexCode = "#F2F4F8" },
				new() { Name = "Snowbelt", HexCode = "#D7E2E9" },
				new() { Name = "Early Frost", HexCode = "#A9A9C4" },
				new() { Name = "Cosmic Sky", HexCode = "#D0D1E1" },
				new() { Name = "Hailstorm", HexCode = "#EBECEF" },
				new() { Name = "Bright Grey", HexCode = "#908DB9" },
				new() { Name = "Purple Amethyst", HexCode = "#E0A39C" },
				new() { Name = "Berrie Popsicle", HexCode = "#D4A6D1" },
				new() { Name = "Pink Frosting", HexCode = "#F7D9E1" },
				new() { Name = "Soft Breeze", HexCode = "#FBF8F6" },
				new() { Name = "Nordic Breeze", HexCode = "#D3DDE6" },
				new() { Name = "Emerald Wave", HexCode = "#52ADA2" },
				new() { Name = "Baby Powder", HexCode = "#ADDDCA" },
				new() { Name = "Stem Green", HexCode = "#F7F8F3" },
				new() { Name = "Wormwood Green", HexCode = "#AADE87" },
				new() { Name = "Snowflake", HexCode = "#9DB09C" },
				new() { Name = "Wayward Willow", HexCode = "#EEF0F0" },
				new() { Name = "Hazel Gaze", HexCode = "#D6D9D0" },
				new() { Name = "Glacial Green", HexCode = "#B7BDB0" },
				new() { Name = "Lychee Pulp", HexCode = "#6EB5A5" },
				new() { Name = "Caramelized Pears", HexCode = "#F9F4DB" },
				new() { Name = "Vintage Red", HexCode = "#E7D6AC" },
				new() { Name = "Hurricane Haze", HexCode = "#A13842" },
				new() { Name = "Alpine Frost", HexCode = "#BDBBAD" },
				new() { Name = "Milk Grass", HexCode = "#E0DED2" },
				new() { Name = "Winter Frost", HexCode = "#FAF8F0" },
				new() { Name = "Purple Kiss", HexCode = "#D55F8F" },
				new() { Name = "Dark Desire Rose", HexCode = "#F9C4BA" },
				new() { Name = "Snowpink", HexCode = "#FADFDE" },
				new() { Name = "Cupcake Rose", HexCode = "#F1C5C2" },
				new() { Name = "Rose Sangria", HexCode = "#F7D4C8" },
				new() { Name = "Floral White", HexCode = "#FDFAF3" },
				new() { Name = "Frosty Day", HexCode = "#CFECF5" },
				new() { Name = "Azure Sky", HexCode = "#A9E2F5" },
				new() { Name = "Dusty Purple", HexCode = "#8E6C90" },
				new() { Name = "Coral Mantle", HexCode = "#FCD8CC" },
				new() { Name = "Maroon Oak", HexCode = "#550D0E" },
				new() { Name = "Majestic Beige", HexCode = "#DFD7CC" },
				new() { Name = "Almond Rose", HexCode = "#CE8884" },
				new() { Name = "Coral Dune", HexCode = "#FBD3C4" },
				new() { Name = "Cosmic Latte", HexCode = "#FDF8E8" },
				new() { Name = "Meadow Grass", HexCode = "#BED5AE" },
				new() { Name = "Dusty Olive", HexCode = "#7A7D68" },
				new() { Name = "Pearl Sugar", HexCode = "#F6F3EE" },
				new() { Name = "Pink Pickled Turnips", HexCode = "#E0457F" },
				new() { Name = "Green Jelly", HexCode = "#3CAD99" },
				new() { Name = "Dark Raspberry", HexCode = "#872657" },
				new() { Name = "Momo Peach", HexCode = "#F47983" },
				new() { Name = "Glamour White", HexCode = "#FFFCEB" },
				new() { Name = "Dark Purple", HexCode = "#35063E" },
				new() { Name = "Imaginary Mauve", HexCode = "#89687D" },
				new() { Name = "Luminescent Blue", HexCode = "#A7E0E9" },
				new() { Name = "Red Wrath", HexCode = "#F01159" },
				new() { Name = "Winter’s Day", HexCode = "#DFF8FE" },
				new() { Name = "Sky Of The Ocean", HexCode = "#82CDE5" },
				new() { Name = "Night Dive", HexCode = "#003458" },
				new() { Name = "Dreamy Cloud", HexCode = "#DEE0E0" },
				new() { Name = "Pink Nectar", HexCode = "#D8ABB7" },
				new() { Name = "Pale Rose", HexCode = "#EDD5D8" },
				new() { Name = "Bonfire", HexCode = "#F57B51" },
				new() { Name = "Golden Rambler", HexCode = "#FBBC58" },
				new() { Name = "Apple White", HexCode = "#F7F8E2" },
				new() { Name = "Moorland Mist", HexCode = "#DFDECA" },
				new() { Name = "Honey Butter", HexCode = "#F4D59A" },
				new() { Name = "French Toast", HexCode = "#DE8626" },
				new() { Name = "Tomato Soup", HexCode = "#C93D21" },
				new() { Name = "Tomato Sauce", HexCode = "#8E1A02" },
				new() { Name = "Chocolate Eclair", HexCode = "#563A3D" },
				new() { Name = "Dark Marmalade", HexCode = "#9A4D36" },
				new() { Name = "Toasted Barley Flakes", HexCode = "#E0D6C3" },
				new() { Name = "Gingerbread Latte", HexCode = "#B4977B" },
				new() { Name = "Pinkish Grey", HexCode = "#C6ADAD" },
				new() { Name = "Grim White", HexCode = "#F6F0F3" },
				new() { Name = "Shy Pink", HexCode = "#DFD7DB" },
				new() { Name = "Blush Grey Rose", HexCode = "#CABBBE" },
				new() { Name = "Pepper Jelly", HexCode = "#C62644" },
				new() { Name = "Blazing Autumn", HexCode = "#F3AC61" },
				new() { Name = "Green Daze", HexCode = "#8AD2C6" },
				new() { Name = "Majestic Mist", HexCode = "#9CA1B3" },
				new() { Name = "Naturally Calm", HexCode = "#CFD1DA" },
				new() { Name = "Ash Violet", HexCode = "#9593A4" },
				new() { Name = "Maple Red", HexCode = "#B8223C" },
				new() { Name = "Pumpkin Latte", HexCode = "#F3B062" },
				new() { Name = "Dying Leaves", HexCode = "#90772F" },
				new() { Name = "Dark Oak", HexCode = "#402320" },
				new() { Name = "Wisteria", HexCode = "#CAA6DB" },
				new() { Name = "Risotto", HexCode = "#F7F4E7" },
				new() { Name = "Green Chalk", HexCode = "#B6DF82" },
				new() { Name = "Red Berry", HexCode = "#721B29" },
				new() { Name = "Cinnamon Baked", HexCode = "#9D653F" },
				new() { Name = "Powder Cake", HexCode = "#DFD8CB" },
				new() { Name = "Blueberry Bark", HexCode = "#22314A" },
				new() { Name = "Wood Bark", HexCode = "#302621" },
				new() { Name = "Creamy Mushroom", HexCode = "#CBBDAD" },
				new() { Name = "Dry Maple Leaf", HexCode = "#984F0E" },
				new() { Name = "Wild Brown", HexCode = "#4A2619" },
				new() { Name = "Rose Taupe", HexCode = "#8D5F5B" },
				new() { Name = "Auburn Wave", HexCode = "#D8A194" },
				new() { Name = "Aria Ivory", HexCode = "#F9E7D8" },
				new() { Name = "Veronese Peach", HexCode = "#ECC0A5" },
				new() { Name = "Nyctophile Blue", HexCode = "#080E2C" },
				new() { Name = "Blue Dacnis", HexCode = "#44D6E9" },
				new() { Name = "Chilly White", HexCode = "#ECF4F1" },
				new() { Name = "Morning Breeze", HexCode = "#D1E0DB" },
				new() { Name = "Black Safflower", HexCode = "#342E37" },
				new() { Name = "Chanterelle", HexCode = "#DEA81C" },
				new() { Name = "Wild Oats", HexCode = "#ECDCC4" },
				new() { Name = "Sweet Cherry Red", HexCode = "#8E1533" },
				new() { Name = "Rosewood", HexCode = "#762014" },
				new() { Name = "Raw Sunset", HexCode = "#D23520" },
				new() { Name = "Anemone White", HexCode = "#F9EFE5 " },
				new() { Name = "Novelle Peach", HexCode = "#E57B30" },
				new() { Name = "Heirloom Rose", HexCode = "#D186A0" },
				new() { Name = "Tropical Peach", HexCode = "#FDC2B1" },
				new() { Name = "Magic Mint", HexCode = "#AAF0D1" },
				new() { Name = "Calestra Grey", HexCode = "#9CADB3" },
				new() { Name = "Offshore Mint", HexCode = "#CFDADA" },
				new() { Name = "Frosted Glass", HexCode = "#EBF2F2" },
				new() { Name = "Rhythmic Blue", HexCode = "#BBD5D8" },
				new() { Name = "Great Canyon", HexCode = "#CB6347" },
				new() { Name = "Cashew Delight", HexCode = "#DFCBB2" },
				new() { Name = "Dulce De Leche", HexCode = "#CC9972" },
				new() { Name = "Cocoa Dust", HexCode = "#8C533D" },
				new() { Name = "Cherry Bomb", HexCode = "#B33B44" },
				new() { Name = "Rice Bowl", HexCode = "#F1E7D6" },
				new() { Name = "Barley Seeds", HexCode = "#DABE85" },
				new() { Name = "Shadow Purple", HexCode = "#462B45" },
				new() { Name = "Twilight Lavender", HexCode = "#884A6F" },
				new() { Name = "Snow Plum", HexCode = "#F5EAEF" },
				new() { Name = "Rose Marble", HexCode = "#D0BCC4" },
				new() { Name = "Archipelago Green", HexCode = "#B7DF69" },
				new() { Name = "Shallow White", HexCode = "#F4F1EC" },
				new() { Name = "Shallow Blue", HexCode = "#9EEBE2" },
				new() { Name = "Beachy Blue", HexCode = "#1FD8D8" },
				new() { Name = "Jazzy Jade", HexCode = "#51C9C2" },
				new() { Name = "Ambrosia Cake", HexCode = "#EEE9CF" },
				new() { Name = "Whitewash", HexCode = "#FCFFF5" },
				new() { Name = "Cherry Blossom Pink", HexCode = "#FEBAC6" },
				new() { Name = "Storm Blue", HexCode = "#182F53" },
				new() { Name = "Alpaca Wool", HexCode = "#F9EEE2" },
				new() { Name = "Coral Rose", HexCode = "#F57A4D" },
				new() { Name = "Smoked Purple", HexCode = "#444251" },
				new() { Name = "Lavender Haze", HexCode = "#8D89A3" },
				new() { Name = "Violet Hush", HexCode = "#E4E3E5" },
				new() { Name = "Ghostly Grey", HexCode = "#CBCBCF" },
				new() { Name = "Sparkling Pink", HexCode = "#F5CDE6" },
				new() { Name = "Snow Mint", HexCode = "#CFF5EA" },
				new() { Name = "Mint Macaroon", HexCode = "#A7E9E1" },
				new() { Name = "Soft Blush", HexCode = "#E3BCBC" },
				new() { Name = "White Smoke", HexCode = "#F4F4F4" },
				new() { Name = "Silver Lake", HexCode = "#E0E0E0" },
				new() { Name = "Glacier Grey", HexCode = "#C4C6C8" },
				new() { Name = "Blue Haze", HexCode = "#BEBCCB" },
				new() { Name = "White Lilac", HexCode = "#F1EFF2" },
				new() { Name = "Lilac Frost", HexCode = "#DDD7DC" },
				new() { Name = "Heliotrope Grey", HexCode = "#A994A7" },
				new() { Name = "Brem Cake", HexCode = "#F1E1C9" },
				new() { Name = "Antarctic Deep", HexCode = "#3A3C42" },
				new() { Name = "Woodland", HexCode = "#9CCD62" },
				new() { Name = "Nocturnal Sea", HexCode = "#095D6A" },
				new() { Name = "Aquamarine Blue", HexCode = "#72D2E3" },
				new() { Name = "White Desert", HexCode = "#FAF8ED" },
				new() { Name = "Lavender Cream", HexCode = "#CAAAF3" },
				new() { Name = "Heavy Charcoal", HexCode = "#56514B" },
				new() { Name = "Mystic Mist", HexCode = "#E7E5DD" },
				new() { Name = "Concrete Jungle", HexCode = "#999990" },
				new() { Name = "Purple Starburst", HexCode = "#AA6890" },
				new() { Name = "Taffy Pink", HexCode = "#FCACC7" },
				new() { Name = "Girly Nursery", HexCode = "#F9EDED" },
				new() { Name = "Antique Mauve", HexCode = "#BDADAE" },
				new() { Name = "Blackcurrant", HexCode = "#2F1A35" },
				new() { Name = "Watermelon Candy", HexCode = "#FF5976" },
				new() { Name = "Lavender Blush", HexCode = "#FFF6FA" },
				new() { Name = "Aquarelle Red", HexCode = "#FDDBDB" },
				new() { Name = "Illusion Blue", HexCode = "#CDD5DC" },
				new() { Name = "Pearl Grey", HexCode = "#B2B6BB" },
				new() { Name = "Ambitious Rose", HexCode = "#E9687E" },
				new() { Name = "Lemon Meringue", HexCode = "#F7E298" },
				new() { Name = "Coal Black", HexCode = "#222220" },
				new() { Name = "Gothic Gold", HexCode = "#C28C2E" },
				new() { Name = "Vintage Ephemera", HexCode = "#D8CDB9" },
				new() { Name = "Soft Cashmere", HexCode = "#F3B8D9" },
				new() { Name = "Turquoise Blue", HexCode = "#64D7EB" },
				new() { Name = "Luxor Gold", HexCode = "#9C7627" },
				new() { Name = "Olive Hint", HexCode = "#C9BA86" },
				new() { Name = "Dirty White", HexCode = "#E8E3CC" },
				new() { Name = "Cinnamon Stick", HexCode = "#9C4A29" },
				new() { Name = "Royal Fuchsia", HexCode = "#F340AF" },
				new() { Name = "Bright Sun", HexCode = "#FBDB14" },
				new() { Name = "Bright Teal", HexCode = "#01F9C6" },
				new() { Name = "Shimmering Blush", HexCode = "#DD84A1" },
				new() { Name = "Matte Pink", HexCode = "#FFBBBE" },
				new() { Name = "Beryl Green", HexCode = "#CBE3B3" },
				new() { Name = "Cocoa Bean", HexCode = "#4F3B38" },
				new() { Name = "Malibu Beige", HexCode = "#C9C1B0" },
				new() { Name = "Foggy Day", HexCode = "#E5E1D8" },
				new() { Name = "Burlwood", HexCode = "#9C6F69" },
				new() { Name = "Dark Indigo", HexCode = "#171D4B" },
				new() { Name = "Turquoise Gemstone", HexCode = "#2ED3C6" },
				new() { Name = "Diamond Dust", HexCode = "#F8F5E6" },
				new() { Name = "Green Gooseberry", HexCode = "#AFE19F" },
				new() { Name = "Light Turquoise", HexCode = "#5FDED7" },
				new() { Name = "Vanilla Shake", HexCode = "#FFFDF8" },
				new() { Name = "Midday Sun", HexCode = "#FFDC8E" },
				new() { Name = "Pink Sapphire", HexCode = "#E22A77" },
				new() { Name = "Midnight Badger", HexCode = "#575965" },
				new() { Name = "Quiet Grey", HexCode = "#C3C4C8" },
				new() { Name = "Classic Chalk", HexCode = "#F8F8F6" },
				new() { Name = "Wolf Grey", HexCode = "#939498" },
				new() { Name = "Neon Fuchsia", HexCode = "#FA376C" },
				new() { Name = "Peach Fizz", HexCode = "#FFA780" },
				new() { Name = "Clear Moon", HexCode = "#FAF8E0" },
				new() { Name = "Nightly Aurora", HexCode = "#9BEEC1" },
				new() { Name = "Majestic Magenta", HexCode = "#F03E9E" },
				new() { Name = "Pisco Sour", HexCode = "#C0E876" },
				new() { Name = "Satin Latour", HexCode = "#FAD2AD" },
				new() { Name = "Strawberry Mousse", HexCode = "#A4647F" },
				new() { Name = "Pink Blush", HexCode = "#F8ADB5" },
				new() { Name = "Pink Dogwood", HexCode = "#F8D8D8" },
				new() { Name = "Aubergine", HexCode = "#452B30" },
				new() { Name = "Brownish Purple", HexCode = "#74404C" },
				new() { Name = "Parchment", HexCode = "#F3ECD8" },
				new() { Name = "Watercress Pesto", HexCode = "#C7C79E" },
				new() { Name = "Plum Purple", HexCode = "#631647" },
				new() { Name = "Autumn Glory", HexCode = "#FF8B0D" },
				new() { Name = "Milk Soda", HexCode = "#FDFFF0" },
				new() { Name = "Frozen Margarita", HexCode = "#E3E8CD" },
				new() { Name = "Tropical Freeze", HexCode = "#97D7CB" },
				new() { Name = "Spring Lily", HexCode = "#FCF2C7" },
				new() { Name = "Chamomile Tea", HexCode = "#DBC797" },
				new() { Name = "Chamoisee", HexCode = "#9C765D" },
				new() { Name = "Cream Gold", HexCode = "#DDC35D" },
				new() { Name = "Sunny", HexCode = "#F0EC73" },
				new() { Name = "Red Safflower", HexCode = "#C53B3D" },
				new() { Name = "Red Claret", HexCode = "#86213D" },
				new() { Name = "Tangerine Cream", HexCode = "#FFA286" },
				new() { Name = "Sweet Peach", HexCode = "#FCDCC8" },
				new() { Name = "Rose Sugar", HexCode = "#FFF4F2" },
				new() { Name = "Sweet Aqua", HexCode = "#A5EACF" },
				new() { Name = "Molten Lava", HexCode = "#B33534" },
				new() { Name = "Tropical Light", HexCode = "#A1DD70" },
				new() { Name = "Wood Charcoal", HexCode = "#464646" },
				new() { Name = "Mist Grey", HexCode = "#C4C4BC" },
				new() { Name = "Shy Beige", HexCode = "#DEDAD1" },
				new() { Name = "Stormy Strait Green", HexCode = "#13A699" },
				new() { Name = "Sunflower Yellow", HexCode = "#FFD708" },
				new() { Name = "Tamy Thyme", HexCode = "#C7C6AB" },
				new() { Name = "Bright Winter Cloud", HexCode = "#F9F5F0" },
				new() { Name = "Cameo Rose", HexCode = "#F7E1D8" },
				new() { Name = "Scotchtone", HexCode = "#EBCCB9" },
				new() { Name = "Bright Rose", HexCode = "#D11C5B" },
				new() { Name = "Moonlight", HexCode = "#F6EED5" },
				new() { Name = "Yellow Jelly", HexCode = "#FAC600" },
				new() { Name = "Nasturtium Flower", HexCode = "#EA4B1D" },
				new() { Name = "Lusty Lips", HexCode = "#D51D48" },
				new() { Name = "Smudged Lips", HexCode = "#F24463" },
				new() { Name = "Rose White", HexCode = "#FAEDE7" },
				new() { Name = "Dreamsicle", HexCode = "#F7D3C3" },
				new() { Name = "Blackout", HexCode = "#242121" },
				new() { Name = "Saffron Mango", HexCode = "#F8C25C" },
				new() { Name = "Daisy White", HexCode = "#F8F3E3" },
				new() { Name = "Red Flag", HexCode = "#FF2047" },
				new() { Name = "Safflower Red", HexCode = "#D63447" },
				new() { Name = "Birch White", HexCode = "#F6EEDF" },
				new() { Name = "Foggy Gray", HexCode = "#D1CEBD" },
				new() { Name = "Carbon", HexCode = "#333333" },
				new() { Name = "Oily Steel", HexCode = "#9BA8A8" },
				new() { Name = "Arctic Fox", HexCode = "#EBEBE7" },
				new() { Name = "Foggy Dew", HexCode = "#C8CEC4" },
				new() { Name = "Bright Lettuce", HexCode = "#95CE67" },
				new() { Name = "Pale Beryl", HexCode = "#95DDDA" },
				new() { Name = "Fuchsia Rose", HexCode = "#C74375" },
				new() { Name = "Bubblegum", HexCode = "#EE778A" },
				new() { Name = "Whitewashed Fence", HexCode = "#FAF3E3" },
				new() { Name = "Pale Daffodil", HexCode = "#FEE698" },
				new() { Name = "Soft Orange", HexCode = "#FDB15D" },
				new() { Name = "Red Pear", HexCode = "#7B3638" },
				new() { Name = "Celestial Coral", HexCode = "#E04255" },
				new() { Name = "Mango Cheesecake", HexCode = "#FCEEDD" },
				new() { Name = "Rose Harmony", HexCode = "#F49FB6" },
				new() { Name = "Lady Pink", HexCode = "#F4D2D2" },
				new() { Name = "Aqua Island", HexCode = "#A6E0DE" },
				new() { Name = "Velvet Black", HexCode = "#2C2627" },
				new() { Name = "Cherry Lush", HexCode = "#BC2C3D" },
				new() { Name = "Peach Puree", HexCode = "#EFD2BC" }
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

		private IEnumerable<CapsulePalette> GetPrimaryCapsulePalettes()
		{
			var allColors = dbContext.Colors.AsQueryable();

			return new List<CapsulePalette>
			{
				// 1. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 1)
				// Các màu: #AEBE89 (Fresh Guacamole), #DAE3BB (Aloe Cream), #FFF7EC (White Lace), #A8EAD5  	(Brook Green)
				// Phân tích: Có màu nhạt, sáng, xanh (#A8EAD5), sắc xám nhẹ (#AEBE89). Phù hợp với "Cool   	Winter" hoặc "Light Summer".
				// Chọn: Cool Winter (1) - vì có cả tông xanh/xám nhạt
				new(){
					ColorTypeId = 1, // Cool Winter
				    Colors = allColors.Where(c => c.HexCode == "#AEBE89" ||
													c.HexCode == "#DAE3BB" ||
													c.HexCode == "#FFF7EC" ||
													c.HexCode == "#A8EAD5").ToList()
				},
				// 2. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 2)
				// Các màu: #C5B08B (Earthy Cane), #DED5BC (Caraway Seeds), #F8F3E6 (Swan White), #C6CEBE   	(Frosty Pine)
				// Phân tích: Màu sắc dịu nhẹ, thiên về đất, có trắng và xanh xám nhạt. Phù hợp với "Soft   	Summer" hoặc "Warm Autumn" nếu Earthy Cane đủ ấm.
				// Chọn: Soft Summer (5) - vì tính chất dịu nhẹ, mờ nhạt
				new(){
					ColorTypeId = 5, // Soft Summer
				    Colors = allColors.Where(c => c.HexCode == "#C5B08B" ||
													c.HexCode == "#DED5BC" ||
													c.HexCode == "#F8F3E6" ||
													c.HexCode == "#C6CEBE").ToList()
				},
				// 3. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 3)
				// Các màu: #850121 (Aurora Red), #B1002A (Incarnadine), #EC7A49 (Sizzling Sunset), #800733 		(Dark Scarlet Red)
				// Phân tích: Đỏ đậm, mạnh mẽ. Có cả màu cam cháy. Phù hợp với "Deep Winter" hoặc "Warm			Autumn".
				// Chọn: Deep Winter (2) - vì tính chất đậm và mạnh mẽ của các màu đỏ.
				new(){
					ColorTypeId = 2, // Deep Winter
				    Colors = allColors.Where(c => c.HexCode == "#850121" ||
													c.HexCode == "#B1002A" ||
													c.HexCode == "#EC7A49" ||
													c.HexCode == "#800733").ToList()
				},
				// 4. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 4)
				// Các màu: #52A7CC (Frozen Wave), #FBC7C3 (Gossamer Pink), #F6EFE3 (Rustic Cream), #FABB7C 		(Rich Honey)
				// Phân tích: Có xanh sáng, hồng nhạt, kem và cam nhạt. Sự kết hợp giữa tông lạnh và ấm		nhẹ,	nhưng có màu sáng, tinh khiết.
				// Chọn: Clear Spring (12) - vì có sự sáng và rõ ràng.
				new(){
					ColorTypeId = 12, // Clear Spring
				    Colors = allColors.Where(c => c.HexCode == "#52A7CC" ||
													c.HexCode == "#FBC7C3" ||
													c.HexCode == "#F6EFE3" ||
													c.HexCode == "#FABB7C").ToList()
				},
				// 5. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 5)
				// Các màu: #15192F (Christmas Eve), #B11E31 (Christmas Red), #FAF2D1 (Christmas Vanilla),  	#096344 (Cake)
				// Phân tích: Các màu đậm, truyền thống, có cả xanh đậm và đỏ đậm. Phù hợp với "Deep	Winter"	hoặc "Deep Autumn".
				// Chọn: Deep Winter (2) - vì sự đậm và rõ nét của các màu.
				new(){
					ColorTypeId = 2, // Deep Winter
				    Colors = allColors.Where(c => c.HexCode == "#15192F" ||
													c.HexCode == "#B11E31" ||
													c.HexCode == "#FAF2D1" ||
													c.HexCode == "#096344").ToList()
				},
				// 6. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 6)
				// Các màu: #FCC4C9 (Christmas Tree), #FDF6F0 (Crystal Rose), #F8E2CF (Backlight), #F5C6AA  	(Sandy Beach)
				// Phân tích: Các màu nhạt, hồng, be, đào. Thiên về sắc sáng, dịu nhẹ.
				// Chọn: Light Summer (6) - vì tính chất sáng, dễ chịu, thanh thoát.
				new(){
					ColorTypeId = 6, // Light Summer
				    Colors = allColors.Where(c => c.HexCode == "#FCC4C9" ||
													c.HexCode == "#FDF6F0" ||
													c.HexCode == "#F8E2CF" ||
													c.HexCode == "#F5C6AA").ToList()
				},
				// 7. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 7)
				// Các màu: #415A80 (Desert Sand), #A5D4DC (Deep Azure), #F2F4F8 (Midwinter Mist), #D7E2E9  	(Snowbelt)
				// Phân tích: Các màu xanh đậm, xanh nhạt, xám nhạt. Thiên về tông lạnh và có sắc xám.
				// Chọn: Cool Winter (1) - vì có sắc lạnh và sắc xám.
				new(){
					ColorTypeId = 1, // Cool Winter
				    Colors = allColors.Where(c => c.HexCode == "#415A80" ||
													c.HexCode == "#A5D4DC" ||
													c.HexCode == "#F2F4F8" ||
													c.HexCode == "#D7E2E9").ToList()
				},
				// 8. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 8)
				// Các màu: #A9A9C4 (Early Frost), #D0D1E1 (Cosmic Sky), #EBECEF (Hailstorm), #908DB9	(Bright	Grey)
				// Phân tích: Các màu tím/xám nhạt, xám trung tính. Có sắc lạnh, nhạt và sắc xám.
				// Chọn: Cool Winter (1) - rất phù hợp với mô tả.
				new(){
					ColorTypeId = 1, // Cool Winter
				    Colors = allColors.Where(c => c.HexCode == "#A9A9C4" ||
													c.HexCode == "#D0D1E1" ||
													c.HexCode == "#EBECEF" ||
													c.HexCode == "#908DB9").ToList()
				},
				// 9. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 9)
				// Các màu: #E0A39C (Purple Amethyst), #FBC7C3 (Coral Cove), #F6EFE3 (Rustic Cream),	#FABB7C	(Rich Honey)
				// Phân tích: Hồng tím, hồng san hô, kem, cam mật. Có sự kết hợp giữa ấm và lạnh, nhưng		thiên	về sắc độ mềm mại và hơi ấm.
				// Chọn: Warm Autumn (7) - do có màu ấm áp, tông đỏ/cam nhẹ.
				new(){
					ColorTypeId = 7, // Warm Autumn
				    Colors = allColors.Where(c => c.HexCode == "#E0A39C" ||
													c.HexCode == "#FBC7C3" || // Lưu ý: FBC7C3		cũng	xuất hiện trong Clear Spring ví dụ
				                                    c.HexCode == "#F6EFE3" || // Lưu ý: F6EFE3		cũng	xuất hiện trong Clear Spring ví dụ
				                                    c.HexCode == "#FABB7C").ToList()
				},
				// 10. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 10)
				// Các màu: #D4A6D1 (Berrie Popsicle), #F7D9E1 (Pink Frosting), #F9F8F6 (Soft Breeze - có	vẻ	là lỗi đánh máy, ý là FBF8F6), #FBF8F6 (Soft Breeze)
				// Phân tích: Các màu hồng/tím nhạt, rất nhẹ nhàng. Phù hợp với "Soft Summer" hoặc "Light   	Summer".
				// Chọn: Soft Summer (5) - do tính chất nhẹ nhàng, mờ.
				new(){
					ColorTypeId = 5, // Soft Summer
				    Colors = allColors.Where(c => c.HexCode == "#D4A6D1" ||
													c.HexCode == "#F7D9E1" ||
													c.HexCode == "#F9F8F6" || // Giả định là	FBF8F6	hoặc một màu tương tự
				                                    c.HexCode == "#FBF8F6").ToList()
				},
				// 11. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 11)
				// Các màu: #A8EAD5 (Brook Green), #D3DDE6 (Nordic Breeze), #F7F4E8 (Snowpink - có vẻ là	lỗi	đánh máy, ý là F9C4BA?), #9DB09C (Snowflake)
				// Phân tích: Xanh mint, xanh xám, trắng kem, xanh xám. Các màu này tươi sáng nhưng nhẹ		nhàng	và có sắc lạnh.
				// Chọn: Light Spring (11) - vì tính chất sáng, nhẹ nhàng, tinh khiết và có màu xanh mát.
				new(){
					ColorTypeId = 11, // Light Spring
				    Colors = allColors.Where(c => c.HexCode == "#A8EAD5" ||
													c.HexCode == "#D3DDE6" ||
													c.HexCode == "#F7F4E8" || // Giả định là	F9C4BA	hoặc một màu tương tự
				                                    c.HexCode == "#9DB09C").ToList()
				},
				// 12. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 12)
				// Các màu: #EEF0F0 (Wayward Willow), #F8F8F3 (Stem Green), #ADDDCA (Baby Powder), #F8F4E8  	(Light Summer)
				// Phân tích: Trắng xám, xanh lá nhạt, xanh mint, trắng kem. Các màu rất sáng và nhẹ nhàng.
				// Chọn: Light Spring (11) - rất phù hợp.
				new(){
					ColorTypeId = 11, // Light Spring
				    Colors = allColors.Where(c => c.HexCode == "#EEF0F0" ||
													c.HexCode == "#F8F8F3" ||
													c.HexCode == "#ADDDCA" ||
													c.HexCode == "#F8F4E8").ToList()
				},
				// 13. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 13)
				// Các màu: #F7F8F3 (Wormwood Green), #A8EAD5 (Brook Green), #9DB09C (Snowflake), #F9F4DB   	(Caramelized Pears)
				// Phân tích: Xanh lá nhạt, xanh mint, xanh xám, vàng kem. Tổng thể sáng và hơi ấm.
				// Chọn: Light Spring (11) - vì sự kết hợp màu sắc sáng, nhẹ nhàng.
				new(){
					ColorTypeId = 11, // Light Spring
				    Colors = allColors.Where(c => c.HexCode == "#F7F8F3" ||
													c.HexCode == "#A8EAD5" ||
													c.HexCode == "#9DB09C" ||
													c.HexCode == "#F9F4DB").ToList()
				},
				// 14. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 14)
				// Các màu: #A13842 (Hurricane Haze), #BDBBAD (Alpine Frost), #E0DED2 (Milk Grass), #FAF8F0 		(Winter Frost)
				// Phân tích: Đỏ nâu, xám xanh nhạt, kem xám. Có tông ấm và lạnh.
				// Chọn: Soft Summer (5) - nếu nhấn mạnh tính chất mờ và dịu nhẹ.
				new(){
					ColorTypeId = 5, // Soft Summer
				    Colors = allColors.Where(c => c.HexCode == "#A13842" ||
													c.HexCode == "#BDBBAD" ||
													c.HexCode == "#E0DED2" ||
													c.HexCode == "#FAF8F0").ToList()
				},
				// 15. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 15)
				// Các màu: #E4DFCF (Purple Kiss), #BDBBAD (Alpine Frost), #E0DED2 (Milk Grass), #FAF8F0		(Winter Frost)
				// Phân tích: Tím nhạt, xám xanh nhạt, kem xám. Rất mềm mại và có sắc lạnh/trung tính.
				// Chọn: Cool Summer (4) - do tính chất dịu nhẹ, tươi sáng nhưng có sắc lạnh nhẹ.
				new(){
					ColorTypeId = 4, // Cool Summer
				    Colors = allColors.Where(c => c.HexCode == "#E4DFCF" ||
													c.HexCode == "#BDBBAD" ||
													c.HexCode == "#E0DED2" ||
													c.HexCode == "#FAF8F0").ToList()
				},
				// 16. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 16)
				// Các màu: #D55F8F (Dark Desire Rose), #F9C4BA (Snowpink), #FAF8F0 (Winter Frost), #E4DFCF 		(Purple Kiss)
				// Phân tích: Hồng đậm, hồng nhạt, trắng kem, tím nhạt. Tổng thể có sắc lạnh và tươi sáng.
				// Chọn: Cool Summer (4) - vì sự tươi sáng nhưng có sắc lạnh.
				new(){
					ColorTypeId = 4, // Cool Summer
				    Colors = allColors.Where(c => c.HexCode == "#D55F8F" ||
													c.HexCode == "#F9C4BA" ||
													c.HexCode == "#FAF8F0" ||
													c.HexCode == "#E4DFCF").ToList()
				},
				// 17. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 17)
				// Các màu: #3D1144 (Cupcake Rose), #CC135C (Rose Sangria), #FADFDE (Floral White), #F1C5C2 		(Frosty Day)
				// Phân tích: Tím đậm, hồng đậm, hồng nhạt. Các màu có độ tương phản và sắc đậm.
				// Chọn: Deep Winter (2) - do tính chất đậm và mạnh mẽ.
				new(){
					ColorTypeId = 2, // Deep Winter
				    Colors = allColors.Where(c => c.HexCode == "#3D1144" ||
													c.HexCode == "#CC135C" ||
													c.HexCode == "#FADFDE" ||
													c.HexCode == "#F1C5C2").ToList()
				},
				// 18. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 18)
				// Các màu: #D0114D (Azure Sky), #EED5D1 (Dusty Purple), #F38593 (Coral Mantle), #D5527D	(  Sweet Aqua)
				// Phân tích: Đỏ tươi, tím nhạt, hồng san hô, hồng. Các màu tươi sáng và có sắc lạnh/trung  	tính.
				// Chọn: Clear Winter (3) - vì tính chất sáng, tinh khiết.
				new(){
					ColorTypeId = 3, // Clear Winter
				    Colors = allColors.Where(c => c.HexCode == "#D0114D" ||
													c.HexCode == "#EED5D1" ||
													c.HexCode == "#F38593" ||
													c.HexCode == "#D5527D").ToList()
				},
				// 19. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 19)
				// Các màu: #511628 (Maroon Oak), #D0114D (Azure Sky), #9E0131 (Soy Milk), #820829	(Majestic	Beige)
				// Phân tích: Nâu đỏ đậm, đỏ tươi, đỏ sẫm, be đậm. Các màu đậm và có vẻ ấm/trung tính.
				// Chọn: Deep Autumn (9) - do tính chất đậm và tối, giống màu lá thu.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#511628" ||
													c.HexCode == "#D0114D" || // Lưu ý: D0114D		cũng	xuất hiện trong Clear Winter ví dụ
				                                    c.HexCode == "#9E0131" ||
													c.HexCode == "#820829").ToList()
				},
				// 20. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 20)
				// Các màu: #4D3211 (Almond Rose), #C28C2E (Coral Dune), #D8CDB9 (Cosmic Latte), #721B29		(Meadow Grass)
				// Phân tích: Nâu, cam đất, be, xanh rêu đậm. Các màu ấm và thiên về tông đất.
				// Chọn: Warm Autumn (7) - rất phù hợp với mô tả.
				new(){
					ColorTypeId = 7, // Warm Autumn
				    Colors = allColors.Where(c => c.HexCode == "#4D3211" ||
													c.HexCode == "#C28C2E" ||
													c.HexCode == "#D8CDB9" ||
													c.HexCode == "#721B29").ToList()
				},
				// 21. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 21)
				// Các màu: #F7D4C8 (Dusty Olive), #FDFAF3 (Pearl Sugar), #CFECF5 (Pink Pickled Turnips),   	#A9E2F5 (Green Jelly)
				// Phân tích: Đào nhạt, trắng ngà, xanh da trời nhạt, xanh lá nhạt. Các màu sáng, nhẹ	nhàng.
				// Chọn: Light Spring (11) - rất phù hợp với mô tả.
				new(){
					ColorTypeId = 11, // Light Spring
				    Colors = allColors.Where(c => c.HexCode == "#F7D4C8" ||
													c.HexCode == "#FDFAF3" ||
													c.HexCode == "#CFECF5" ||
													c.HexCode == "#A9E2F5").ToList()
				},
				// 22. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 22)
				// Các màu: #8E6C90 (Dark Raspberry), #FCD8CC (Momo Peach), #FFF7EC (Glamour White - có vẻ	là	lỗi đánh máy, ý là Swan White?), #A8EAD5 (Brook Green)
				// Phân tích: Tím đậm, đào nhạt, trắng kem, xanh mint. Sự pha trộn giữa lạnh và ấm, nhưng	tông	màu có xu hướng dịu nhẹ.
				// Chọn: Soft Summer (5) - do tính chất nhẹ nhàng, mờ.
				new(){
					ColorTypeId = 5, // Soft Summer
				    Colors = allColors.Where(c => c.HexCode == "#8E6C90" ||
													c.HexCode == "#FCD8CC" ||
													c.HexCode == "#FFF7EC" || // Giả định là một	màu	trắng kem tương tự
				                                    c.HexCode == "#A8EAD5").ToList()
				},
				// 23. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 23)
				// Các màu: #550D0E (Glamour White), #A13842 (Dark Purple), #FFF7EC (White Lace), #DFD7CC   	(Imaginary Mauve)
				// Phân tích: Đỏ sẫm, tím đậm, trắng, tím nhạt. Các màu có độ sâu và có tông lạnh/trung		tính.
				// Chọn: Deep Winter (2) - do sự đậm và mạnh mẽ.
				new(){
					ColorTypeId = 2, // Deep Winter
				    Colors = allColors.Where(c => c.HexCode == "#550D0E" ||
													c.HexCode == "#A13842" ||
													c.HexCode == "#FFF7EC" || // Lưu ý: FFF7EC		cũng	là White Lace trong Cool Winter ví dụ
				                                    c.HexCode == "#DFD7CC").ToList()
				},
				// 24. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 24)
				// Các màu: #CE8884 (Red Wrath), #FBD3C4 (Winter's Day), #FDF8E8 (Sky of the Ocean),	#BED5AE	(Night Dive)
				// Phân tích: Đỏ gạch nhạt, đào nhạt, vàng kem, xanh rêu nhạt. Tổng thể ấm áp và dịu nhẹ.
				// Chọn: Warm Autumn (7) - do tính chất ấm áp và có tông đỏ/vàng/cam.
				new(){
					ColorTypeId = 7, // Warm Autumn
				    Colors = allColors.Where(c => c.HexCode == "#CE8884" ||
													c.HexCode == "#FBD3C4" ||
													c.HexCode == "#FDF8E8" ||
													c.HexCode == "#BED5AE").ToList()
				},
				// 25. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 25)
				// Các màu: #7A7D68 (Dreamy Cloud), #F6F3EE (Pale Rose), #E0DED2 (Bonfire - có vẻ là lỗi	đánh	máy, ý là Milk Grass?), #BDBBAD (Alpine Frost)
				// Phân tích: Xám xanh rêu, hồng nhạt, kem xám. Các màu này dịu nhẹ và có sắc xám.
				// Chọn: Soft Summer (5) - do tính chất nhẹ nhàng, mờ và sắc xám.
				new(){
					ColorTypeId = 5, // Soft Summer
				    Colors = allColors.Where(c => c.HexCode == "#7A7D68" ||
													c.HexCode == "#F6F3EE" ||
													c.HexCode == "#E0DED2" || // Giả định là một	màu	tương tự E0DED2
				                                    c.HexCode == "#BDBBAD").ToList()
				},
				// 26. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 26)
				// Các màu: #E0457F (Golden Rambler), #F9F4DB (Apple White), #E7D6AC (Moorland Mist),	#3CAD99	(Honey Butter)
				// Phân tích: Hồng đậm, trắng kem, vàng đất, xanh lá cây. Sự kết hợp giữa màu sáng và màu	đất.
				// Chọn: Warm Spring (10) - do có màu sáng và ấm áp.
				new(){
					ColorTypeId = 10, // Warm Spring
				    Colors = allColors.Where(c => c.HexCode == "#E0457F" ||
													c.HexCode == "#F9F4DB" ||
													c.HexCode == "#E7D6AC" ||
													c.HexCode == "#3CAD99").ToList()
				},
				// 27. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 27)
				// Các màu: #872657 (French Toast), #F47983 (Tomato Soup), #FFFCEB (Tomato Sauce), #35063E  	(Chocolate Eclair)
				// Phân tích: Đỏ tím đậm, hồng đỏ, trắng ngà, tím sẫm. Các màu có độ sâu và đậm.
				// Chọn: Deep Winter (2) - vì sự đậm và mạnh mẽ.
				new(){
					ColorTypeId = 2, // Deep Winter
				    Colors = allColors.Where(c => c.HexCode == "#872657" ||
													c.HexCode == "#F47983" ||
													c.HexCode == "#FFFCEB" ||
													c.HexCode == "#35063E").ToList()
				},
				// 28. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 28)
				// Các màu: #89687D (Dark Marmalade), #FCD8CC (Momo Peach), #FFF7EC (Gingerbread Latte),		#A7E0E9 (Pinkish Grey)
				// Phân tích: Xám tím, đào nhạt, trắng kem, xanh xám nhạt. Các màu có tông dịu nhẹ và hơi	mờ.
				// Chọn: Soft Summer (5) - do tính chất nhẹ nhàng, mờ.
				new(){
					ColorTypeId = 5, // Soft Summer
				    Colors = allColors.Where(c => c.HexCode == "#89687D" ||
													c.HexCode == "#FCD8CC" ||
													c.HexCode == "#FFF7EC" || // Giả định là một	màu	trắng kem tương tự
				                                    c.HexCode == "#A7E0E9").ToList()
				},
				// 29. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 29)
				// Các màu: #F01159 (Grim White), #DFF8FE (Shy Pink), #82CDE5 (Blush Grey Rose), #003458		(Pepper Jelly)
				// Phân tích: Đỏ tươi, xanh nhạt, xanh dương nhạt, xanh navy đậm. Các màu rõ ràng và có sắc 		lạnh.
				// Chọn: Clear Winter (3) - vì tính chất sáng, tinh khiết và rõ ràng.
				new(){
					ColorTypeId = 3, // Clear Winter
				    Colors = allColors.Where(c => c.HexCode == "#F01159" ||
													c.HexCode == "#DFF8FE" ||
													c.HexCode == "#82CDE5" ||
													c.HexCode == "#003458").ToList()
				},
				// 30. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 30)
				// Các màu: #52A7CC (Blazing Autumn), #A5D4DC (Green Daze), #F2F4F8 (Majestic Mist),	#DEE0E0	(Naturally Calm)
				// Phân tích: Xanh da trời, xanh nhạt, xám nhạt. Các màu dịu nhẹ và có sắc lạnh.
				// Chọn: Cool Summer (4) - do tính chất dịu nhẹ, tươi sáng nhưng có sắc lạnh nhẹ.
				new(){
					ColorTypeId = 4, // Cool Summer
				    Colors = allColors.Where(c => c.HexCode == "#52A7CC" || // Lưu ý: 52A7CC cũng  	xuất hiện trong Clear Spring ví dụ
				                                    c.HexCode == "#A5D4DC" || // Lưu ý: A5D4DC		cũng	xuất hiện trong Clear Spring ví dụ
				                                    c.HexCode == "#F2F4F8" ||
													c.HexCode == "#DEE0E0").ToList()
				},
				// 31. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 31)
				// Các màu: #D8ABB7 (Ash Violet), #EDD5D8 (Maple Red), #FAF8F0 (Winter Frost), #E4DFCF	(Purple	Kiss)
				// Phân tích: Tím hồng nhạt, hồng đỏ nhạt, trắng kem, tím nhạt. Tổng thể mềm mại và có sắc  	lạnh/trung tính.
				// Chọn: Cool Summer (4) - vì sự tươi sáng nhưng có sắc lạnh.
				new(){
					ColorTypeId = 4, // Cool Summer
				    Colors = allColors.Where(c => c.HexCode == "#D8ABB7" ||
													c.HexCode == "#EDD5D8" ||
													c.HexCode == "#FAF8F0" || // Lưu ý: FAF8F0		cũng	xuất hiện trong Soft Summer ví dụ
				                                    c.HexCode == "#E4DFCF").ToList()
				},
				// 32. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 32)
				// Các màu: #F57B51 (Pumpkin Latte), #FBBC58 (Dying Leaves), #F7F8E2 (Dark Oak), #DFDECA		(Wisteria)
				// Phân tích: Cam, vàng cam, xanh rêu đất, nâu đất. Các màu ấm áp và đậm chất mùa thu.
				// Chọn: Warm Autumn (7) - rất phù hợp với mô tả.
				new(){
					ColorTypeId = 7, // Warm Autumn
				    Colors = allColors.Where(c => c.HexCode == "#F57B51" ||
													c.HexCode == "#FBBC58" ||
													c.HexCode == "#F7F8E2" ||
													c.HexCode == "#DFDECA").ToList()
				},
				// 33. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 33)
				// Các màu: #F4D59A (Risotto), #DE8626 (Green Chalk), #C93D21 (Red Berry), #8E1A02	(Cinnamon	Baked)
				// Phân tích: Vàng be, cam cháy, đỏ, nâu cam. Các màu ấm và có độ sâu.
				// Chọn: Deep Autumn (9) - do tính chất đậm và tối.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#F4D59A" ||
													c.HexCode == "#DE8626" ||
													c.HexCode == "#C93D21" ||
													c.HexCode == "#8E1A02").ToList()
				},
				// 34. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 34)
				// Các màu: #563A3D (Powder Cake), #9A4D36 (Blueberry Bark), #E0D6C3 (Wood Bark), #B4977B   	(Creamy Mushroom)
				// Phân tích: Nâu sẫm, nâu, nâu đất, nâu be. Các màu đất ấm và có độ sâu.
				// Chọn: Deep Autumn (9) - rất phù hợp.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#563A3D" ||
													c.HexCode == "#9A4D36" ||
													c.HexCode == "#E0D6C3" ||
													c.HexCode == "#B4977B").ToList()
				},
				// 35. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 35)
				// Các màu: #C6ADAD (Dry Maple Leaf), #F6F0F3 (Wild Brown), #DFD7DB (Rose Taupe), #CABBBE   	(Auburn Wave)
				// Phân tích: Các màu nâu xám, hồng xám, hồng tím xám. Các màu này rất mềm mại và có sắc	xám.
				// Chọn: Soft Summer (5) - rất phù hợp với mô tả.
				new(){
					ColorTypeId = 5, // Soft Summer
				    Colors = allColors.Where(c => c.HexCode == "#C6ADAD" ||
													c.HexCode == "#F6F0F3" ||
													c.HexCode == "#DFD7DB" ||
													c.HexCode == "#CABBBE").ToList()
				},
				// 36. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 36)
				// Các màu: #C62644 (Aria Ivory), #F3AC61 (Veronese Peach), #F6EFE3 (Nyctophile Blue),	#8AD2C6	(Blue Dacnis)
				// Phân tích: Đỏ đậm, cam đào, trắng kem, xanh ngọc. Có cả màu đậm và màu sáng, thiên về	sự rõ	nét.
				// Chọn: Clear Spring (12) - do sự sáng và rõ ràng.
				new(){
					ColorTypeId = 12, // Clear Spring
				    Colors = allColors.Where(c => c.HexCode == "#C62644" ||
													c.HexCode == "#F3AC61" ||
													c.HexCode == "#F6EFE3" || // Lưu ý: F6EFE3		cũng	xuất hiện trong Clear Spring ví dụ
				                                    c.HexCode == "#8AD2C6").ToList()
				},
				// 37. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 37)
				// Các màu: #9CA1B3 (Chilly White), #CFD1DA (Morning Breeze), #EBECEF (Black Safflower),		#9593A4 (Chanterelle)
				// Phân tích: Các màu xám nhạt, xám trung tính. Có sắc lạnh, nhạt và sắc xám.
				// Chọn: Cool Winter (1) - rất phù hợp.
				new(){
					ColorTypeId = 1, // Cool Winter
				    Colors = allColors.Where(c => c.HexCode == "#9CA1B3" ||
													c.HexCode == "#CFD1DA" ||
													c.HexCode == "#EBECEF" ||
													c.HexCode == "#9593A4").ToList()
				},
				// 38. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 38)
				// Các màu: #B8223C (Wild Oats), #F3B062 (Sweet Cherry Red), #90772F (Rosewood), #402320	(Raw	Sunset)
				// Phân tích: Đỏ nâu, cam đào, nâu đất, nâu sẫm. Các màu ấm áp và có độ sâu.
				// Chọn: Deep Autumn (9) - do tính chất đậm và tối, giống màu lá thu.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#B8223C" ||
													c.HexCode == "#F3B062" ||
													c.HexCode == "#90772F" ||
													c.HexCode == "#402320").ToList()
				},
				// 39. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 39)
				// Các màu: #CAA6DB (Anemone White), #FBC7C3 (Novelle Peach), #F7F4E7 (Heilroom Rose),	#B6DF82	(Tropical Peach)
				// Phân tích: Tím nhạt, hồng nhạt, trắng kem, xanh lá nhạt. Các màu sáng, nhẹ nhàng.
				// Chọn: Light Spring (11) - rất phù hợp.
				new(){
					ColorTypeId = 11, // Light Spring
				    Colors = allColors.Where(c => c.HexCode == "#CAA6DB" ||
													c.HexCode == "#FBC7C3" || // Lưu ý: FBC7C3		cũng	xuất hiện trong Clear Spring ví dụ
				                                    c.HexCode == "#F7F4E7" ||
													c.HexCode == "#B6DF82").ToList()
				},
				// 40. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 40)
				// Các màu: #721B29 (Magic Mint), #9D653F (Calestra Grey), #DFD8CB (Offshore Mint), #22314A 		(Frosted Glass)
				// Phân tích: Đỏ sẫm, nâu đất, nâu be, xanh navy đậm. Các màu có độ sâu và đậm.
				// Chọn: Deep Autumn (9) - do tính chất đậm và tối.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#721B29" || // Lưu ý: 721B29 cũng  	xuất hiện trong Warm Autumn ví dụ
				                                    c.HexCode == "#9D653F" || // Lưu ý: 9D653F		cũng	xuất hiện trong Deep Autumn ví dụ
				                                    c.HexCode == "#DFD8CB" ||
													c.HexCode == "#22314A").ToList()
				},
				// 41. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 41)
				// Các màu: #0096D1 (Rhythmic Blue), #FFF4EA (Great Canyon), #A8EAD5 (Cashew Delight),	#3EBDC6	(Dulce de Leche)
				// Phân tích: Xanh dương sáng, trắng kem, xanh mint, xanh ngọc. Các màu sáng và rõ ràng.
				// Chọn: Clear Spring (12) - rất phù hợp.
				new(){
					ColorTypeId = 12, // Clear Spring
				    Colors = allColors.Where(c => c.HexCode == "#0096D1" ||
													c.HexCode == "#FFF4EA" ||
													c.HexCode == "#A8EAD5" ||
													c.HexCode == "#3EBDC6").ToList()
				},
				// 42. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 42)
				// Các màu: #302621 (Cocoa Dust), #CBBDAD (Cherry Bomb), #984F0E (Rice Bowl), #4A2619	(Barley	Seeds)
				// Phân tích: Nâu sẫm, nâu be, nâu cam, nâu đậm. Các màu ấm và đậm chất đất.
				// Chọn: Deep Autumn (9) - rất phù hợp.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#302621" ||
													c.HexCode == "#CBBDAD" ||
													c.HexCode == "#984F0E" ||
													c.HexCode == "#4A2619").ToList()
				},
				// 43. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 43)
				// Các màu: #8D5F5B (Shadow Purple), #D8A194 (Twilight Lavender), #F9E7D8 (Snow Plum),	#ECC0A5	(Rose Marble)
				// Phân tích: Tím xám, tím hồng nhạt, hồng đào nhạt, cam đào nhạt. Các màu mềm mại và có	sắc	xám/tím.
				// Chọn: Soft Summer (5) - rất phù hợp.
				new(){
					ColorTypeId = 5, // Soft Summer
				    Colors = allColors.Where(c => c.HexCode == "#8D5F5B" ||
													c.HexCode == "#D8A194" ||
													c.HexCode == "#F9E7D8" ||
													c.HexCode == "#ECC0A5").ToList()
				},
				// 44. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 44)
				// Các màu: #080E2C (Archipelago Green), #44D6E9 (Shallow White), #ECF4F1 (Shallow Blue),   	#D1E0DB (Beachy Blue)
				// Phân tích: Xanh navy rất đậm, xanh sáng, trắng xanh, xanh xám. Các màu có độ sâu và sắc  	lạnh.
				// Chọn: Cool Winter (1) - do sự đậm và sắc lạnh.
				new(){
					ColorTypeId = 1, // Cool Winter
				    Colors = allColors.Where(c => c.HexCode == "#080E2C" ||
													c.HexCode == "#44D6E9" ||
													c.HexCode == "#ECF4F1" ||
													c.HexCode == "#D1E0DB").ToList()
				},
				// 45. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 45)
				// Các màu: #342E37 (Jazzy Jade), #DEA81C (Ambrosia Cake), #ECDCC4 (Whitewash), #8E1533		(   Cherry Blossom Pink)
				// Phân tích: Xám xanh đậm, vàng đất, be kem, hồng đỏ đậm. Sự kết hợp giữa màu đậm và màu	ấm.
				// Chọn: Deep Autumn (9) - do tính chất đậm và tối.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#342E37" ||
													c.HexCode == "#DEA81C" ||
													c.HexCode == "#ECDCC4" ||
													c.HexCode == "#8E1533").ToList()
				},
				// 46. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 46)
				// Các màu: #762014 (Storm Blue), #D23520 (Alpaca Wool), #FFFFFF (Coral Rose), #E57B30		(Brownish Red)
				// Phân tích: Nâu đỏ đậm, đỏ cam, trắng tinh khiết, cam cháy. Các màu có độ sâu và sắc ấm.
				// Chọn: Warm Autumn (7) - do tính chất ấm áp và tông đỏ/cam.
				new(){
					ColorTypeId = 7, // Warm Autumn
				    Colors = allColors.Where(c => c.HexCode == "#762014" ||
													c.HexCode == "#D23520" ||
													c.HexCode == "#FFFFFF" ||
													c.HexCode == "#E57B30").ToList()
				},
				// 47. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 47)
				// Các màu: #5B0004 (Smoked Purple), #F85528 (Lavender Haze), #F9EFE5 (Violet Hush),	#E6D1BF	(Ghostly Grey)
				// Phân tích: Đỏ sẫm, cam đỏ, tím nhạt, xám nâu. Các màu có độ sâu và tông ấm/trung tính.
				// Chọn: Deep Autumn (9) - do tính chất đậm và tối.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#5B0004" || // Lưu ý: 5B0004 cũng  	xuất hiện trong Deep Winter ví dụ
				                                    c.HexCode == "#F85528" ||
													c.HexCode == "#F9EFE5" ||
													c.HexCode == "#E6D1BF").ToList()
				},
				// 48. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 48)
				// Các màu: #D186A0 (Sparkling Pink), #FDC2B1 (Snow Mint), #FDF8E8 (Mint Macaroon), #AAF0D1 		(Soft Blush)
				// Phân tích: Hồng tím nhạt, đào nhạt, vàng kem, xanh mint. Các màu sáng, dịu nhẹ và có sắc 		lạnh.
				// Chọn: Light Summer (6) - do tính chất sáng, dễ chịu và thanh thoát.
				new(){
					ColorTypeId = 6, // Light Summer
				    Colors = allColors.Where(c => c.HexCode == "#D186A0" ||
													c.HexCode == "#FDC2B1" ||
													c.HexCode == "#FDF8E8" ||
													c.HexCode == "#AAF0D1").ToList()
				},
				// 49. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 49)
				// Các màu: #9CADB3 (Soft Blush), #CFDADA (White Smoke), #EBF2F2 (Silver Lake), #BBD5D8			(Glacier Grey)
				// Phân tích: Các màu xám xanh, xám trắng, xanh xám. Các màu lạnh, nhạt và có sắc xám.
				// Chọn: Cool Summer (4) - do tính chất dịu nhẹ, tươi sáng nhưng có sắc lạnh nhẹ.
				new(){
					ColorTypeId = 4, // Cool Summer
				    Colors = allColors.Where(c => c.HexCode == "#9CADB3" ||
													c.HexCode == "#CFDADA" ||
													c.HexCode == "#EBF2F2" ||
													c.HexCode == "#BBD5D8").ToList()
				},
				// 50. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 50)
				// Các màu: #CB6347 (Blue Haze), #DFCBB2 (White Lilac), #CC9972 (Lilac Frost), #8C533D		(Heliotrope Grey)
				// Phân tích: Cam cháy, trắng ngà, nâu cam, nâu đất. Các màu ấm và có độ sâu.
				// Chọn: Warm Autumn (7) - do tính chất ấm áp và tông đỏ/cam.
				new(){
					ColorTypeId = 7, // Warm Autumn
				    Colors = allColors.Where(c => c.HexCode == "#CB6347" ||
													c.HexCode == "#DFCBB2" ||
													c.HexCode == "#CC9972" ||
													c.HexCode == "#8C533D").ToList()
				},
				// 51. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 51)
				// Các màu: #711E2B (Brem Cake), #B33B44 (Antarctic Deep), #F1E7D6 (Woodland), #DABE85		(Nocturnal Sea)
				// Phân tích: Nâu đỏ đậm, đỏ sẫm, be đất, vàng đất. Các màu có độ sâu và ấm áp.
				// Chọn: Deep Autumn (9) - rất phù hợp.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#711E2B" ||
													c.HexCode == "#B33B44" ||
													c.HexCode == "#F1E7D6" ||
													c.HexCode == "#DABE85").ToList()
				},
				// 52. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 52)
				// Các màu: #462B45 (Aquamarine Blue), #884A6F (White Desert), #F5EAEF (Lavender Cream),		#D0BCC4 (Heavy Charcoal)
				// Phân tích: Tím đậm, tím hồng, tím nhạt, xám tím. Các màu có sắc lạnh và độ sâu nhẹ.
				// Chọn: Cool Winter (1) - do sự đậm và sắc lạnh.
				new(){
					ColorTypeId = 1, // Cool Winter
				    Colors = allColors.Where(c => c.HexCode == "#462B45" ||
													c.HexCode == "#884A6F" ||
													c.HexCode == "#F5EAEF" ||
													c.HexCode == "#D0BCC4").ToList()
				},
				// 53. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 53)
				// Các màu: #B7DF69 (Mystic Mist), #F4F1EC (Concrete Jungle), #9EEBE2 (Purple Starburst),   	#1FD8D8 (Taffy Pink)
				// Phân tích: Xanh lá nhạt, xám trắng, xanh ngọc, xanh cyan sáng. Các màu sáng và có sắc	lạnh.
				// Chọn: Light Spring (11) - vì tính chất sáng, nhẹ nhàng, tinh khiết.
				new(){
					ColorTypeId = 11, // Light Spring
				    Colors = allColors.Where(c => c.HexCode == "#B7DF69" ||
													c.HexCode == "#F4F1EC" ||
													c.HexCode == "#9EEBE2" ||
													c.HexCode == "#1FD8D8").ToList()
				},
				// 54. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 54)
				// Các màu: #51C9C2 (Girly Nursery), #EEE9CF (Antique Mauve), #FCFFF5 (Blackcurrant),	#FEBAC6	(Watermelon Candy)
				// Phân tích: Xanh ngọc, vàng kem, trắng ngà, hồng nhạt. Các màu sáng, dịu nhẹ và có sắc	lạnh.
				// Chọn: Light Summer (6) - do tính chất sáng, dễ chịu và thanh thoát.
				new(){
					ColorTypeId = 6, // Light Summer
				    Colors = allColors.Where(c => c.HexCode == "#51C9C2" ||
													c.HexCode == "#EEE9CF" ||
													c.HexCode == "#FCFFF5" ||
													c.HexCode == "#FEBAC6").ToList()
				},
				// 55. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 55)
				// Các màu: #182F53 (Aquarelle Red), #F9EEE2 (Illusion Blue), #F57A4D (Pearl Grey), #AEBE89 		(Ambitious Rose)
				// Phân tích: Xanh navy đậm, trắng kem, cam cháy, xanh rêu nhạt. Sự kết hợp giữa màu đậm và 		màu đất/ấm.
				// Chọn: Deep Autumn (9) - do tính chất đậm và tối.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#182F53" ||
													c.HexCode == "#F9EEE2" ||
													c.HexCode == "#F57A4D" ||
													c.HexCode == "#AEBE89").ToList()
				},
				// 56. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 56)
				// Các màu: #444251 (Lemon Meringue), #8D89A3 (Coal Black), #E4E3E5 (Gothic Gold), #CBCBCF  	(Vintage Ephemera)
				// Phân tích: Xám đậm, đen than, xám trắng, xám nhạt. Các màu trung tính và có sắc lạnh.
				// Chọn: Cool Winter (1) - vì có sắc xám và tông lạnh.
				new(){
					ColorTypeId = 1, // Cool Winter
				    Colors = allColors.Where(c => c.HexCode == "#444251" ||
													c.HexCode == "#8D89A3" ||
													c.HexCode == "#E4E3E5" ||
													c.HexCode == "#CBCBCF").ToList()
				},
				// 57. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 57)
				// Các màu: #F5CDE6 (Soft Cashmere), #FAF8F0 (Turquoise Blue), #CFF5EA (Luxor Gold),	#A7E9E1	(Olive Hint)
				// Phân tích: Hồng nhạt, xanh ngọc, xanh mint, xanh lá nhạt. Các màu sáng, dịu nhẹ và có	sắc	lạnh.
				// Chọn: Light Summer (6) - do tính chất sáng, dễ chịu và thanh thoát.
				new(){
					ColorTypeId = 6, // Light Summer
				    Colors = allColors.Where(c => c.HexCode == "#F5CDE6" ||
													c.HexCode == "#FAF8F0" || // Lưu ý: FAF8F0		cũng	xuất hiện trong Soft Summer ví dụ
				                                    c.HexCode == "#CFF5EA" || // Lưu ý: CFF5EA		cũng	xuất hiện trong Soft Summer ví dụ
				                                    c.HexCode == "#A7E9E1").ToList()
				},
				// 58. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 58)
				// Các màu: #E3BCBC (Dirty White), #F4F4F4 (Cinnamon Stick), #E0E0E0 (Royal Fuchsia),	#C4C6C8	(Bright Sun)
				// Phân tích: Hồng nhạt, trắng xám, xám nhạt. Các màu trung tính và có sắc lạnh.
				// Chọn: Cool Summer (4) - do tính chất dịu nhẹ, tươi sáng nhưng có sắc lạnh nhẹ.
				new(){
					ColorTypeId = 4, // Cool Summer
				    Colors = allColors.Where(c => c.HexCode == "#E3BCBC" ||
													c.HexCode == "#F4F4F4" ||
													c.HexCode == "#E0E0E0" ||
													c.HexCode == "#C4C6C8").ToList()
				},
				// 59. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 59)
				// Các màu: #BEBCCB (Bright Teal), #F1EFF2 (Shimmering Blush), #DDD7DC (Matte Pink),	#A994A7	(Beryl Green)
				// Phân tích: Tím xám, hồng nhạt, hồng tím nhạt, xanh xám. Các màu dịu nhẹ và có sắc xám.
				// Chọn: Soft Summer (5) - rất phù hợp.
				new(){
					ColorTypeId = 5, // Soft Summer
				    Colors = allColors.Where(c => c.HexCode == "#BEBCCB" ||
													c.HexCode == "#F1EFF2" ||
													c.HexCode == "#DDD7DC" ||
													c.HexCode == "#A994A7").ToList()
				},
				// 60. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 60)
				// Các màu: #F1E1C9 (Cocoa Bean), #FAF8F0 (Turquoise Blue), #CFECF5 (Pink Pickled Turnips), 		#A9E2F5 (Green Jelly)
				// Phân tích: Nâu be, xanh ngọc, xanh da trời nhạt, xanh lá nhạt. Các màu sáng, dịu nhẹ và	có	sắc lạnh/ấm trung tính.
				// Chọn: Light Spring (11) - vì tính chất sáng, nhẹ nhàng, tinh khiết.
				new(){
					ColorTypeId = 11, // Light Spring
				    Colors = allColors.Where(c => c.HexCode == "#F1E1C9" ||
													c.HexCode == "#FAF8F0" ||
													c.HexCode == "#CFECF5" ||
													c.HexCode == "#A9E2F5").ToList()
				},
				// 61. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 61)
				// Các màu: #3A3C42 (Carbon), #9CCD62 (Bright Sun), #F7F8E2 (Burlwood), #DFDECA (Wisteria)
				// Phân tích: Xám đậm, xanh lá cây, vàng đất, nâu đất. Các màu có độ sâu và tông ấm/trung	tính.
				// Chọn: Deep Autumn (9) - do tính chất đậm và tối.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#3A3C42" ||
													c.HexCode == "#9CCD62" ||
													c.HexCode == "#F7F8E2" ||
													c.HexCode == "#DFDECA").ToList()
				},
				// 62. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 62)
				// Các màu: #F57B51 (Pumpkin Latte), #FDF6F0 (Crystal Rose), #FBBC58 (Dying Leaves),	#095D6A	(Dark Indigo)
				// Phân tích: Cam, hồng nhạt, vàng cam, xanh navy đậm. Sự kết hợp giữa màu ấm và màu đậm/   lạnh.
				// Chọn: Warm Autumn (7) - do tính chất ấm áp và tông đỏ/cam.
				new(){
					ColorTypeId = 7, // Warm Autumn
				    Colors = allColors.Where(c => c.HexCode == "#F57B51" || // Lưu ý: F57B51 cũng  	xuất hiện trong Warm Spring ví dụ
				                                    c.HexCode == "#FDF6F0" || // Lưu ý: FDF6F0		cũng	xuất hiện trong Soft Summer ví dụ
				                                    c.HexCode == "#FBBC58" || // Lưu ý: FBBC58		cũng	xuất hiện trong Warm Spring ví dụ
				                                    c.HexCode == "#095D6A").ToList()
				},
				// 63. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 63)
				// Các màu: #72D2E3 (Sky of the Ocean), #A6EBE7 (Midwinter Mist), #FAF8ED (Snowbelt),	#CAAAF3	(Early Frost)
				// Phân tích: Xanh da trời sáng, xanh mint, vàng kem, tím nhạt. Các màu sáng, nhẹ nhàng và	có	sắc lạnh.
				// Chọn: Light Summer (6) - do tính chất sáng, dễ chịu và thanh thoát.
				new(){
					ColorTypeId = 6, // Light Summer
				    Colors = allColors.Where(c => c.HexCode == "#72D2E3" ||
													c.HexCode == "#A6EBE7" ||
													c.HexCode == "#FAF8ED" ||
													c.HexCode == "#CAAAF3").ToList()
				},
				// 64. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 64)
				// Các màu: #56514B (Woodland), #E7E5DD (Nocturnal Sea), #BDBBAD (Alpine Frost), #999990	(Milk	Grass)
				// Phân tích: Nâu xám, trắng ngà, xám xanh nhạt, xám. Các màu dịu nhẹ và có sắc xám.
				// Chọn: Soft Summer (5) - rất phù hợp.
				new(){
					ColorTypeId = 5, // Soft Summer
				    Colors = allColors.Where(c => c.HexCode == "#56514B" ||
													c.HexCode == "#E7E5DD" ||
													c.HexCode == "#BDBBAD" || // Lưu ý: BDBBAD		cũng	xuất hiện trong Soft Summer ví dụ
				                                    c.HexCode == "#999990").ToList()
				},
				// 65. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 65)
				// Các màu: #AA6890 (Purple Kiss), #FCACC7 (Snowpink), #F9EDED (Cupcake Rose), #BDADAE	(Rose	Sangria)
				// Phân tích: Tím nhạt, hồng nhạt, hồng kem, nâu hồng. Các màu dịu nhẹ và có sắc lạnh/trung 		tính.
				// Chọn: Soft Summer (5) - rất phù hợp.
				new(){
					ColorTypeId = 5, // Soft Summer
				    Colors = allColors.Where(c => c.HexCode == "#AA6890" ||
													c.HexCode == "#FCACC7" ||
													c.HexCode == "#F9EDED" ||
													c.HexCode == "#BDADAE").ToList()
				},
				// 66. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 66)
				// Các màu: #2F1A35 (Rose Sangria), #FF5976 (Floral White), #FFF6FA (Frosty Day), #FDDBDB   	(Azure Sky)
				// Phân tích: Tím đậm, hồng tươi, trắng hồng, hồng nhạt. Các màu có độ sâu và rõ nét.
				// Chọn: Clear Winter (3) - do tính chất sáng và rõ ràng.
				new(){
					ColorTypeId = 3, // Clear Winter
				    Colors = allColors.Where(c => c.HexCode == "#2F1A35" ||
													c.HexCode == "#FF5976" ||
													c.HexCode == "#FFF6FA" ||
													c.HexCode == "#FDDBDB").ToList()
				},
				// 67. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 67)
				// Các màu: #CDD5DC (White Smoke), #F4F4F4 (Cinnamon Stick), #E0E0E0 (Royal Fuchsia),	#B2B6BB	(Bright Sun)
				// Phân tích: Các màu xám nhạt, trắng xám, xám. Các màu trung tính và có sắc lạnh.
				// Chọn: Cool Summer (4) - do tính chất dịu nhẹ, tươi sáng nhưng có sắc lạnh nhẹ.
				new(){
					ColorTypeId = 4, // Cool Summer
				    Colors = allColors.Where(c => c.HexCode == "#CDD5DC" ||
													c.HexCode == "#F4F4F4" ||
													c.HexCode == "#E0E0E0" ||
													c.HexCode == "#B2B6BB").ToList()
				},
				// 68. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 68)
				// Các màu: #E9687E (Raspberry Pink), #FDC2B1 (Peach Blush), #FDFAF3 (Pearl Sugar), #F7E298 		(Golden Rambler)
				// Phân tích: Hồng, đào nhạt, trắng ngà, vàng nhạt. Các màu sáng và ấm áp.
				// Chọn: Warm Spring (10) - rất phù hợp.
				new(){
					ColorTypeId = 10, // Warm Spring
				    Colors = allColors.Where(c => c.HexCode == "#E9687E" ||
													c.HexCode == "#FDC2B1" ||
													c.HexCode == "#FDFAF3" ||
													c.HexCode == "#F7E298").ToList()
				},
				// 69. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 69)
				// Các màu: #222220 (Coal Black), #C28C2E (Coral Dune), #D8CDB9 (Cosmic Latte), #721B29		(   Meadow Grass)
				// Phân tích: Đen, cam đất, be, xanh rêu đậm. Các màu có độ sâu và ấm.
				// Chọn: Deep Autumn (9) - rất phù hợp.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#222220" ||
													c.HexCode == "#C28C2E" ||
													c.HexCode == "#D8CDB9" ||
													c.HexCode == "#721B29").ToList()
				},
				// 70. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 70)
				// Các màu: #F3B8D9 (Purple Amethyst), #F7F4E7 (Heilroom Rose), #F7D4C8 (Dusty Olive),	#64D7EB	(Green Jelly)
				// Phân tích: Tím nhạt, trắng kem, đào nhạt, xanh dương sáng. Các màu sáng và có sắc lạnh.
				// Chọn: Light Spring (11) - vì tính chất sáng, nhẹ nhàng, tinh khiết.
				new(){
					ColorTypeId = 11, // Light Spring
				    Colors = allColors.Where(c => c.HexCode == "#F3B8D9" ||
													c.HexCode == "#F7F4E7" ||
													c.HexCode == "#F7D4C8" || // Lưu ý: F7D4C8		cũng	xuất hiện trong Warm Spring ví dụ
				                                    c.HexCode == "#64D7EB").ToList()
				},
				// 71. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 71)
				// Các màu: #9C7627 (Turquoise Gemstone), #C9BA86 (Diamond Dust), #E8E3CC ( Gooseberry), #   9C4A29 (Light Turquoise)
				// Phân tích: Vàng đất, be vàng, vàng kem, nâu cam. Các màu ấm và có độ sâu.
				// Chọn: Warm Autumn (7) - rất phù hợp.
				new(){
					ColorTypeId = 7, // Warm Autumn
				    Colors = allColors.Where(c => c.HexCode == "#9C7627" ||
													c.HexCode == "#C9BA86" ||
													c.HexCode == "#E8E3CC" ||
													c.HexCode == "#9C4A29").ToList()
				},
				// 72. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 72)
				// Các màu: #F340AF (Vanilla Shake), #FBDB14 (Midday Sun), #FFFCEB (Pink Sapphire), #01F9C6 		(Midnight Badger)
				// Phân tích: Hồng tươi, vàng tươi, trắng ngà, xanh mint tươi. Các màu sáng và rõ ràng.
				// Chọn: Clear Spring (12) - rất phù hợp.
				new(){
					ColorTypeId = 12, // Clear Spring
				    Colors = allColors.Where(c => c.HexCode == "#F340AF" ||
													c.HexCode == "#FBDB14" ||
													c.HexCode == "#FFFCEB" || // Lưu ý: FFFCEB		cũng	xuất hiện trong Warm Autumn ví dụ
				                                    c.HexCode == "#01F9C6").ToList()
				},
				// 73. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 73)
				// Các màu: #DD84A1 (Quiet Grey), #FFBBBE (Classic Chalk), #FDF8E8 (Wolf Grey), #CBE3B3		(Neon	Fuchsia)
				// Phân tích: Hồng tím nhạt, hồng phấn, vàng kem, xanh lá nhạt. Các màu sáng, dịu nhẹ và có 		sắc lạnh.
				// Chọn: Light Summer (6) - rất phù hợp.
				new(){
					ColorTypeId = 6, // Light Summer
				    Colors = allColors.Where(c => c.HexCode == "#DD84A1" ||
													c.HexCode == "#FFBBBE" ||
													c.HexCode == "#FDF8E8" || // Lưu ý: FDF8E8		cũng	xuất hiện trong Light Summer ví dụ
				                                    c.HexCode == "#CBE3B3").ToList()
				},
				// 74. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 74)
				// Các màu: #4F3B38 (Peach Fizz), #C9C1B0 (Clear Moon), #E5E1D8 (Nightly Aurora), #9C6F69   	(Majestic Magenta)
				// Phân tích: Nâu sẫm, be xám, trắng xám, tím hồng. Các màu có độ sâu và tông trung tính/   lạnh.
				// Chọn: Deep Winter (2) - do sự đậm và mạnh mẽ.
				new(){
					ColorTypeId = 2, // Deep Winter
				    Colors = allColors.Where(c => c.HexCode == "#4F3B38" ||
													c.HexCode == "#C9C1B0" ||
													c.HexCode == "#E5E1D8" ||
													c.HexCode == "#9C6F69").ToList()
				},
				// 75. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 75)
				// Các màu: #171D4B (Pisco Sour), #2ED3C6 (Satin Latour), #F8F5E6 (Strawberry Mousse),	#AFE19F	(Pink Blush)
				// Phân tích: Xanh navy rất đậm, xanh ngọc sáng, trắng kem, xanh mint. Các màu rõ ràng và	có	sắc lạnh.
				// Chọn: Clear Winter (3) - rất phù hợp.
				new(){
					ColorTypeId = 3, // Clear Winter
				    Colors = allColors.Where(c => c.HexCode == "#171D4B" ||
													c.HexCode == "#2ED3C6" ||
													c.HexCode == "#F8F5E6" ||
													c.HexCode == "#AFE19F").ToList()
				},
				// 76. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 76)
				// Các màu: #5FDED7 (Pink Dogwood), #FFFDF8 (Aubergine), #FFDC8E (Brownish Purple), #E22A77 		(Parchment)
				// Phân tích: Xanh mint sáng, trắng tinh khiết, vàng cam, hồng đậm. Các màu sáng và rõ	ràng.
				// Chọn: Clear Spring (12) - rất phù hợp.
				new(){
					ColorTypeId = 12, // Clear Spring
				    Colors = allColors.Where(c => c.HexCode == "#5FDED7" ||
													c.HexCode == "#FFFDF8" ||
													c.HexCode == "#FFDC8E" ||
													c.HexCode == "#E22A77").ToList()
				},
				// 77. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 77)
				// Các màu: #575965 (Watercress Pesto), #C3C4C8 (Plum Purple), #F8F8F6 (Autumn Glory),	#939498	(Milk Soda)
				// Phân tích: Xám xanh đậm, xám nhạt, trắng xám. Các màu trung tính và có sắc lạnh.
				// Chọn: Cool Summer (4) - do tính chất dịu nhẹ, tươi sáng nhưng có sắc lạnh nhẹ.
				new(){
					ColorTypeId = 4, // Cool Summer
				    Colors = allColors.Where(c => c.HexCode == "#575965" ||
													c.HexCode == "#C3C4C8" ||
													c.HexCode == "#F8F8F6" ||
													c.HexCode == "#939498").ToList()
				},
				// 78. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 78)
				// Các màu: #FA376C (Frozen Margarita), #FFA780 (Tropical Freeze), #FAF8E0 (Spring Lily),   	#9BEEC1 (Chamomile Tea)
				// Phân tích: Hồng tươi, cam nhạt, vàng kem, xanh lá nhạt. Các màu sáng và ấm áp.
				// Chọn: Warm Spring (10) - rất phù hợp.
				new(){
					ColorTypeId = 10, // Warm Spring
				    Colors = allColors.Where(c => c.HexCode == "#FA376C" ||
													c.HexCode == "#FFA780" ||
													c.HexCode == "#FAF8E0" ||
													c.HexCode == "#9BEEC1").ToList()
				},
				// 79. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 79)
				// Các màu: #F03E9E (Chamoisee), #C0E876 (Cream Gold), #F7F4E7 (Sunny), #FAD2AD (Red	Safflower)
				// Phân tích: Hồng đậm, xanh lá vàng, vàng kem, cam đào. Các màu sáng và ấm áp.
				// Chọn: Warm Spring (10) - rất phù hợp.
				new(){
					ColorTypeId = 10, // Warm Spring
				    Colors = allColors.Where(c => c.HexCode == "#F03E9E" ||
													c.HexCode == "#C0E876" ||
													c.HexCode == "#F7F4E7" ||
													c.HexCode == "#FAD2AD").ToList()
				},
				// 80. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 80)
				// Các màu: #A4647F (Red Claret), #F8ADB5 (Tangerine Cream), #FFF6FA (Sweet Peach), #F8D8D8 		(Rose Sugar)
				// Phân tích: Hồng đỏ đậm, hồng cam nhạt, trắng hồng, hồng nhạt. Các màu dịu nhẹ và có sắc	ấm.
				// Chọn: Soft Autumn (8) - do tính chất dịu dàng, mờ và có tông hồng ấm.
				new(){
					ColorTypeId = 8, // Soft Autumn
				    Colors = allColors.Where(c => c.HexCode == "#A4647F" ||
													c.HexCode == "#F8ADB5" ||
													c.HexCode == "#FFF6FA" ||
													c.HexCode == "#F8D8D8").ToList()
				},
				// 81. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 81)
				// Các màu: #452B30 (Molten Lava), #74404C (Lime Meringue), #F3ECD8 (Coconut Cream),	#C7C79E	(Tropical Light)
				// Phân tích: Nâu sẫm, nâu đỏ, trắng kem, xanh rêu nhạt. Các màu có độ sâu và ấm áp.
				// Chọn: Deep Autumn (9) - rất phù hợp.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#452B30" ||
													c.HexCode == "#74404C" ||
													c.HexCode == "#F3ECD8" ||
													c.HexCode == "#C7C79E").ToList()
				},
				// 82. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 82)
				// Các màu: #631647 (Wood Charcoal), #FF8B0D (Mist Grey), #FDFFF0 (Shy Beige), #E3E8CD	(Stormy	Strait Green)
				// Phân tích: Tím sẫm, cam tươi, trắng kem, xanh lá cây nhạt. Sự kết hợp giữa màu đậm và	màu	sáng/ấm.
				// Chọn: Warm Spring (10) - do có màu sáng và ấm áp.
				new(){
					ColorTypeId = 10, // Warm Spring
				    Colors = allColors.Where(c => c.HexCode == "#631647" ||
													c.HexCode == "#FF8B0D" ||
													c.HexCode == "#FDFFF0" ||
													c.HexCode == "#E3E8CD").ToList()
				},
				// 83. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 83)
				// Các màu: #97D7CB (Sunflower Yellow), #FCF2C7 (White Romance), #DBC797 (Tamy Thyme),	#9C765D	(Bright Winter Cloud)
				// Phân tích: Xanh mint, vàng kem, vàng đất, nâu xám. Các màu này dịu nhẹ và có sắc ấm/ trung	tính.
				// Chọn: Soft Autumn (8) - rất phù hợp.
				new(){
					ColorTypeId = 8, // Soft Autumn
				    Colors = allColors.Where(c => c.HexCode == "#97D7CB" ||
													c.HexCode == "#FCF2C7" ||
													c.HexCode == "#DBC797" ||
													c.HexCode == "#9C765D").ToList()
				},
				// 84. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 84)
				// Các màu: #DDC35D (Cameo Rose), #F0EC73 (Scotchtone), #C53B3D (Bright Rose), #86213D		(Moonlight)
				// Phân tích: Vàng đất, vàng tươi, đỏ tươi, nâu đỏ. Các màu sáng, rõ ràng và ấm áp.
				// Chọn: Clear Spring (12) - rất phù hợp.
				new(){
					ColorTypeId = 12, // Clear Spring
				    Colors = allColors.Where(c => c.HexCode == "#DDC35D" ||
													c.HexCode == "#F0EC73" ||
													c.HexCode == "#C53B3D" ||
													c.HexCode == "#86213D").ToList()
				},
				// 85. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 85)
				// Các màu: #FFA286 (Yellow Jelly), #FCDCC8 (Nasturtium Flower), #FFF4F2 (Lusty Lips),	#A5EACF	(Smudged Lips)
				// Phân tích: Cam nhạt, đào nhạt, trắng hồng, xanh mint. Các màu sáng, nhẹ nhàng và ấm áp.
				// Chọn: Light Spring (11) - rất phù hợp.
				new(){
					ColorTypeId = 11, // Light Spring
				    Colors = allColors.Where(c => c.HexCode == "#FFA286" ||
													c.HexCode == "#FCDCC8" ||
													c.HexCode == "#FFF4F2" ||
													c.HexCode == "#A5EACF").ToList()
				},
				// 86. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 86)
				// Các màu: #B33534 (Blackout), #E3E8CD (Saffron Mango), #FAFCEE (Daisy White), #A1DD70		(Red	Flag)
				// Phân tích: Đỏ đậm, xanh lá vàng nhạt, trắng kem, xanh lá cây. Các màu có độ sâu và sắc	ấm/   trung tính.
				// Chọn: Warm Autumn (7) - do tính chất ấm áp.
				new(){
					ColorTypeId = 7, // Warm Autumn
				    Colors = allColors.Where(c => c.HexCode == "#B33534" ||
													c.HexCode == "#E3E8CD" ||
													c.HexCode == "#FAFCEE" ||
													c.HexCode == "#A1DD70").ToList()
				},
				// 87. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 87)
				// Các màu: #464646 (Safflower Red), #C4C4BC (Birch White), #F4F4F4 (Foggy Gray), #DEDAD1   	(Carbon)
				// Phân tích: Xám đậm, trắng xám, xám nhạt. Các màu trung tính và có sắc lạnh.
				// Chọn: Cool Winter (1) - vì có sắc xám và tông lạnh.
				new(){
					ColorTypeId = 1, // Cool Winter
				    Colors = allColors.Where(c => c.HexCode == "#464646" ||
													c.HexCode == "#C4C4BC" ||
													c.HexCode == "#F4F4F4" || // Lưu ý: F4F4F4		cũng	xuất hiện trong Cool Summer ví dụ
				                                    c.HexCode == "#DEDAD1").ToList()
				},
				// 88. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 88)
				// Các màu: #13A699 (Oily Steel), #FFD708 (Arctic Fox), #FFF7ED (Foggy Dew), #AAF0D1	(Bright	Lettuce)
				// Phân tích: Xanh ngọc, vàng tươi, trắng kem, xanh mint. Các màu sáng và rõ ràng.
				// Chọn: Clear Spring (12) - rất phù hợp.
				new(){
					ColorTypeId = 12, // Clear Spring
				    Colors = allColors.Where(c => c.HexCode == "#13A699" ||
													c.HexCode == "#FFD708" ||
													c.HexCode == "#FFF7ED" ||
													c.HexCode == "#AAF0D1").ToList()
				},
				// 89. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 89)
				// Các màu: #C7C6AB (Pale Beryl), #F9F5F0 (Fuchsia Rose), #F7E1D8 (Bubblegum), #EBCCB9		(Whitewashed Fence)
				// Phân tích: Xám xanh nhạt, hồng tím nhạt, hồng phấn, nâu be. Các màu dịu nhẹ và có sắc	xám/   ấm.
				// Chọn: Soft Summer (5) - rất phù hợp.
				new(){
					ColorTypeId = 5, // Soft Summer
				    Colors = allColors.Where(c => c.HexCode == "#C7C6AB" ||
													c.HexCode == "#F9F5F0" ||
													c.HexCode == "#F7E1D8" ||
													c.HexCode == "#EBCCB9").ToList()
				},
				// 90. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 90)
				// Các màu: #D11C5B (Briquette), #F6EED5 (Coconut Pulp), #FAC600 (Pale Daffodil), #EA4B1D	( Soft Orange)
				// Phân tích: Đỏ đậm, trắng kem, vàng, cam. Các màu tươi sáng và ấm áp.
				// Chọn: Warm Spring (10) - rất phù hợp.
				new(){
					ColorTypeId = 10, // Warm Spring
				    Colors = allColors.Where(c => c.HexCode == "#D11C5B" ||
													c.HexCode == "#F6EED5" ||
													c.HexCode == "#FAC600" ||
													c.HexCode == "#EA4B1D").ToList()
				},
				// 91. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 91)
				// Các màu: #D51D48 (Red Pear), #F24463 (Celestial Coral), #FAEDE7 (Mango Cheesecake),	#F7D3C3	(Rose Harmony)
				// Phân tích: Đỏ đậm, hồng cam, trắng kem, hồng nhạt. Các màu sáng, rõ ràng và có sắc ấm/   trung	tính.
				// Chọn: Clear Spring (12) - rất phù hợp.
				new(){
					ColorTypeId = 12, // Clear Spring
				    Colors = allColors.Where(c => c.HexCode == "#D51D48" ||
													c.HexCode == "#F24463" ||
													c.HexCode == "#FAEDE7" ||
													c.HexCode == "#F7D3C3").ToList()
				},
				// 92. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 92)
				// Các màu: #242121 (Lady Pink), #F8C25C (Aqua Island), #F8F3E3 (Velvet Black), #FF2047		(   Cherry Lush)
				// Phân tích: Đen, vàng cam, trắng kem, đỏ tươi. Các màu có độ tương phản cao và rõ nét.
				// Chọn: Deep Winter (2) - do sự đậm và mạnh mẽ.
				new(){
					ColorTypeId = 2, // Deep Winter
				    Colors = allColors.Where(c => c.HexCode == "#242121" ||
													c.HexCode == "#F8C25C" ||
													c.HexCode == "#F8F3E3" ||
													c.HexCode == "#FF2047").ToList()
				},
				// 93. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 93)
				// Các màu: #D63447 (Peach Puree), #F57B51 (Pumpkin Latte), #F6EEDF (Dry Maple Leaf),	#D1CEBD	(Caraway Seeds)
				// Phân tích: Đỏ cam, cam, nâu be, be. Các màu ấm áp và có tông đất.
				// Chọn: Warm Autumn (7) - rất phù hợp.
				new(){
					ColorTypeId = 7, // Warm Autumn
				    Colors = allColors.Where(c => c.HexCode == "#D63447" ||
													c.HexCode == "#F57B51" || // Lưu ý: F57B51		cũng	xuất hiện trong Warm Spring ví dụ
				                                    c.HexCode == "#F6EEDF" ||
													c.HexCode == "#D1CEBD").ToList()
				},
				// 94. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 94)
				// Các màu: #333333 (Chanterelle), #9BA8A8 (Toasted Barley Flakes), #EBEBE7 (Gingerbread		Latte), #C8CEC4 (Wood Bark)
				// Phân tích: Xám đậm, xám nhạt, trắng kem, xanh xám. Các màu trung tính và có sắc xám.
				// Chọn: Cool Winter (1) - vì có sắc xám và tông lạnh.
				new(){
					ColorTypeId = 1, // Cool Winter
				    Colors = allColors.Where(c => c.HexCode == "#333333" ||
													c.HexCode == "#9BA8A8" ||
													c.HexCode == "#EBEBE7" ||
													c.HexCode == "#C8CEC4").ToList()
				},
				// 95. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 95)
				// Các màu: #95CE67 (Fresh Guacamole), #DAE3BB (Aloe Cream), #FAF8F0 (Winter Frost),	#95DDDA	(Frosty Pine)
				// Phân tích: Xanh lá tươi, xanh lá nhạt, trắng kem, xanh mint. Các màu sáng, nhẹ nhàng và	có	sắc lạnh.
				// Chọn: Light Spring (11) - rất phù hợp.
				new(){
					ColorTypeId = 11, // Light Spring
				    Colors = allColors.Where(c => c.HexCode == "#95CE67" ||
													c.HexCode == "#DAE3BB" ||
													c.HexCode == "#FAF8F0" || // Lưu ý: FAF8F0		cũng	xuất hiện trong Soft Summer ví dụ
				                                    c.HexCode == "#95DDDA").ToList()
				},
				// 96. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 96)
				// Các màu: #C74375 (Rose Taupe), #EE778A (Auburn Wave), #FAF3E3 (Aria Ivory), #FFFFFF		(Veronese Peach)
				// Phân tích: Hồng tím đậm, hồng cam, trắng kem, trắng tinh khiết. Các màu sáng, rõ ràng	và có	sắc ấm/lạnh.
				// Chọn: Clear Spring (12) - do sự sáng và rõ ràng.
				new(){
					ColorTypeId = 12, // Clear Spring
				    Colors = allColors.Where(c => c.HexCode == "#C74375" ||
													c.HexCode == "#EE778A" ||
													c.HexCode == "#FAF3E3" ||
													c.HexCode == "#FFFFFF").ToList()
				},
				// 97. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 97)
				// Các màu: #E75A5F (Nyctophile Blue), #FAFADC (Blue Dacnis), #FEE698 (Chilly White),	#FDB15D	(Morning Breeze)
				// Phân tích: Đỏ cam, xanh lá cây nhạt, vàng nhạt, cam. Các màu sáng và ấm áp.
				// Chọn: Warm Spring (10) - rất phù hợp.
				new(){
					ColorTypeId = 10, // Warm Spring
				    Colors = allColors.Where(c => c.HexCode == "#E75A5F" ||
													c.HexCode == "#FAFADC" ||
													c.HexCode == "#FEE698" ||
													c.HexCode == "#FDB15D").ToList()
				},
				// 98. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 98)
				// Các màu: #7B3638 (Black Safflower), #E04255 (Wild Oats), #FCEEDD (Sweet Cherry	Red), #   A1DD70 (Red Flag)
				// Phân tích: Nâu đỏ sẫm, đỏ tươi, hồng kem, xanh lá cây. Các màu có độ sâu và rõ nét.
				// Chọn: Deep Autumn (9) - do tính chất đậm và tối.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#7B3638" ||
													c.HexCode == "#E04255" ||
													c.HexCode == "#FCEEDD" ||
													c.HexCode == "#A1DD70").ToList()
				},
				// 99. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 99)
				// Các màu: #F49FB6 (Coral Mantle), #F4D2D2 (Pink Pickled Turnips), #FAF8F0 (Winter Frost), 		#A6E0DE (Brook Green)
				// Phân tích: Hồng nhạt, hồng đào, trắng kem, xanh mint. Các màu sáng, nhẹ nhàng và có sắc  	lạnh.
				// Chọn: Light Summer (6) - rất phù hợp.
				new(){
					ColorTypeId = 6, // Light Summer
				    Colors = allColors.Where(c => c.HexCode == "#F49FB6" ||
													c.HexCode == "#F4D2D2" ||
													c.HexCode == "#FAF8F0" || // Lưu ý: FAF8F0		cũng	xuất hiện trong Soft Summer ví dụ
				                                    c.HexCode == "#A6E0DE").ToList()
				},
				// 100. Phân loại lại CapsulePalette ban đầu (ColorTypeId = 100)
				// Các màu: #2C2627 (Cocoa Dust), #BC2C3D (Cherry Bomb), #F8F3E6 (Swan White), #EFD2BC	(Sandy	Beach)
				// Phân tích: Nâu sẫm, đỏ đậm, trắng kem, be đào. Các màu có độ sâu và tông ấm.
				// Chọn: Deep Autumn (9) - rất phù hợp.
				new(){
					ColorTypeId = 9, // Deep Autumn
				    Colors = allColors.Where(c => c.HexCode == "#2C2627" ||
													c.HexCode == "#BC2C3D" ||
													c.HexCode == "#F8F3E6" ||
													c.HexCode == "#EFD2BC").ToList()
				}
			};
		}
	}
}