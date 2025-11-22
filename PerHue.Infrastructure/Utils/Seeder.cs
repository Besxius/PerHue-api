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
			};
		}

		private IEnumerable<ServicePackage> GetPrimaryServicePackages()
		{
			return new List<ServicePackage>
			{
				new() {
					Name = "Freemium",
					Price = 0,
					Description = "Unlimited normal test color\r\n\r\nRelevant suggestions from the brand",
					Duration = 0,
					CreatedDate = DateTime.Now
				},
				new() {
					Name = "AI Test 1",
					Price = 69000,
					Description = "Three times AI-powered detailed color analysis\r\n\r\nUnlimited normal test color\r\n\r\nRelevant suggestions from the brand",
					Duration = 3,
					CreatedDate = DateTime.Now
				},
				new() {
					Name = "AI Test 2",
					Price = 129000,
					Description = "Ten times AI-powered detailed color analysis\r\n\r\nUnlimited normal test color\r\n\r\nRelevant suggestions from the brand",
					Duration = 10,
					CreatedDate = DateTime.Now
				},
				new() {
					Name = "AI Test 3",
					Price = 229000,
					Description = "Advertising brand's product for a month\r\n\r\nTen times AI-powered detailed color analysis\r\n\r\nUnlimited normal test color\r\n\r\nRelevant suggestions from the brand\r\n\r\nNote: This is frequently for brands which want to advertise their products",
					Duration = 10,
					CreatedDate = DateTime.Now
				},
				new() {
					Name = "Expert Suggestion 1",
					Price = 199000,
					Description = "Advertising brand's product for a month\r\n\r\nTen times AI-powered detailed color analysis\r\n\r\nUnlimited normal test color\r\n\r\nRelevant suggestions from the brand\r\n\r\nNote: This is frequently for brands which want to advertise their products",
					Duration = 3,
					CreatedDate = DateTime.Now
				},
				new() {
					Name = "Expert Suggestion 2",
					Price = 299000,
					Description = "Advertising brand's product for a month\r\n\r\nTen times AI-powered detailed color analysis\r\n\r\nUnlimited normal test color\r\n\r\nRelevant suggestions from the brand\r\n\r\nNote: This is frequently for brands which want to advertise their products",
					Duration = 5,
					CreatedDate = DateTime.Now
				},
				new() {
					Name = "Expert Suggestion 3",
					Price = 499000,
					Description = "Advertising brand's product for a month\r\n\r\nTen times AI-powered detailed color analysis\r\n\r\nUnlimited normal test color\r\n\r\nRelevant suggestions from the brand\r\n\r\nNote: This is frequently for brands which want to advertise their products",
					Duration = 10,
					CreatedDate = DateTime.Now
				},
				new() {
					Name = "Test system 1",
					Price = 5000,
					Description = "Advertising brand's product for a month\r\n\r\nTen times AI-powered detailed color analysis\r\n\r\nUnlimited normal test color\r\n\r\nRelevant suggestions from the brand\r\n\r\nNote: This is frequently for brands which want to advertise their products",
					Duration = 10,
					CreatedDate = DateTime.Now
				},
				new() {
					Name = "Test System 2",
					Price = 100000,
					Description = "Advertising brand's product for a month\r\n\r\nTen times AI-powered detailed color analysis\r\n\r\nUnlimited normal test color\r\n\r\nRelevant suggestions from the brand\r\n\r\nNote: This is frequently for brands which want to advertise their products",
					Duration = 10,
					CreatedDate = DateTime.Now
				},
				
			};
		}

		private IEnumerable<UserAccount> GetPrimaryUser()
		{
			return new List<UserAccount>{
				new(){
					Email = "perhue2025@gmail.com",
					Username = "Perhue",
					Password = HashPassWithSHA256.HashWithSHA256("Admin12345!"),
					Fullname = "Ad Thi Min",
					Phone = "0378661398",
					Gender = true,
					Dob = new DateOnly(2000, 1, 1),
					ProfilePicture = string.Empty,
					IsActive = true,
					IsAitested = false,
					CreatedDate = DateTime.Now,
					RoleId = 1,
				},
				new()
				{
					Email = "trongldse173125@fpt.edu.vn",
					Username = "trongldse173125",
					Password =  HashPassWithSHA256.HashWithSHA256("Trong12345!"),
					Fullname = "Le Duc Trong",
					Phone = "0378661398",
					Gender = true,
					Dob = new DateOnly(2003, 6, 16),
					ProfilePicture = string.Empty,
					IsActive = true,
					IsAitested = false,
					CreatedDate = DateTime.Now,
					RoleId = 2,
				},
				new()
				{
					Email = "trongle1161@gmail.com",
					Username = "trongle1161",
					Password =  HashPassWithSHA256.HashWithSHA256("Trong12345!"),
					Fullname = "Le Duc Trong",
					Phone = "0378661398",
					Gender = true,
					Dob = new DateOnly(2003, 6, 16),
					ProfilePicture = string.Empty,
					IsActive = true,
					IsAitested = false,
					CreatedDate = DateTime.Now,
					RoleId = 2,
				},
				new()
				{
					Email = "hungndse173155@fpt.edu.vn",
					Username = "hungndse173155",
					Password = HashPassWithSHA256.HashWithSHA256("Hung12345!"),
					Fullname = "Nguyen Duc Hung",
					Phone = "0345678910",
					Gender = true,
					Dob = new DateOnly(2003, 1, 11),
					ProfilePicture = string.Empty,
					IsActive = true,
					IsAitested = false,
					CreatedDate = DateTime.Now,
					RoleId = 2,
				},
				new()
				{
					Email = "nguyenduchungdh2103@gmail.com",
					Username = "nguyenduchungdh2103",
					Password = HashPassWithSHA256.HashWithSHA256("Hung12345!"),
					Fullname = "Nguyen Duc Hung",
					Phone = "0345678910",
					Gender = true,
					Dob = new DateOnly(2003, 1, 11),
					ProfilePicture = string.Empty,
					IsActive = true,
					IsAitested = false,
					CreatedDate = DateTime.Now,
					RoleId = 2,
				},
				new()
				{
					Email = "annqkhe180905@fpt.edu.vn",
					Username = "annqkhe180905",
					Password = HashPassWithSHA256.HashWithSHA256("An12345!"),
					Fullname = "Nguyen Quang Khanh An",
					Phone = "0345678910",
					Gender = false,
					Dob = new DateOnly(2003, 1, 11),
					ProfilePicture = string.Empty,
					IsActive = true,
					IsAitested = false,
					CreatedDate = DateTime.Now,
					RoleId = 2,
				},
				new()
				{
					Email = "annqk569@gmail.com",
					Username = "annqk569",
					Password = HashPassWithSHA256.HashWithSHA256("An12345!"),
					Fullname = "Nguyen Quang Khanh An",
					Phone = "0345678910",
					Gender = false,
					Dob = new DateOnly(2003, 1, 11),
					ProfilePicture = string.Empty,
					IsActive = true,
					IsAitested = false,
					CreatedDate = DateTime.Now,
					RoleId = 2,
				},
				new()
				{
					Email = "kientse171441@fpt.edu.vn",
					Username = "kientse171441",
					Password = HashPassWithSHA256.HashWithSHA256("Kien12345!"),
					Fullname = "Tran Kien",
					Phone = "0345678910",
					Gender = false,
					Dob = new DateOnly(2003, 1, 11),
					ProfilePicture = string.Empty,
					IsActive = true,
					IsAitested = false,
					CreatedDate = DateTime.Now,
					RoleId = 2,
				},
				new()
				{
					Email = "trankien214@gmail.com",
					Username = "trankien214",
					Password = HashPassWithSHA256.HashWithSHA256("Kien12345!"),
					Fullname = "Tran Kien",
					Phone = "0345678910",
					Gender = false,
					Dob = new DateOnly(2003, 1, 11),
					ProfilePicture = string.Empty,
					IsActive = true,
					IsAitested = false,
					CreatedDate = DateTime.Now,
					RoleId = 2,
				},
				new()
				{
					Email = "leeetrong203@gmail.com",
					Username = "leeetrong203",
					Password = HashPassWithSHA256.HashWithSHA256("Trong12345!"),
					Fullname = "Le Duc Trong",
					Phone = "0345678914",
					Gender = false,
					Dob = new DateOnly(2003, 3, 3),
					ProfilePicture = string.Empty,
					IsActive = true,
					IsAitested = false,
					CreatedDate = DateTime.Now,
					RoleId = 3,
					Expert = new Expert
					{
						Nickname = "Besxius",
						Specialization = "Personal color expert",
						Bio = "Hi, I’m Besxius, your personal color decoder and confidence booster.\r\n\r\nI specialize in moving beyond the basic Seasonal Analysis (Spring, Summer, Autumn, Winter) to pinpoint your precise Undertone and optimal color palette. Simply put:\r\n\r\nI help you stop buying clothes that make you look like you haven't slept in three days.\r\n\r\nMy Philosophy: The right colors don't just highlight your beauty—they elevate your self-assurance. Stop guessing and start glowing.\r\n\r\nCore Services: In-depth personal color analysis, wardrobe consultations, and color-matched makeup guidance.\r\n\r\nThe Result: A bespoke color palette and the cheat codes to make you look effortlessly radiant, every single time.\r\n\r\nConnect: leeetrong203@gmail.com",
						YearsOfExperience = 2,
						Languages = "Vietnamese, English",
						Rating = (decimal?)5.0,
						Certification = "Certified Personal Color expert, Certified Image Consultant, Makeup Artistry Certification",
						CreatedDate = DateTime.Now,
						Introduction = "\"Please welcome a guest who is about to save you time, money, and your complexion. She is not just an analyst; she is a confidence architect who builds dazzling looks starting with the perfect color palette.\r\n\r\nHe is Besxius, and she has successfully guided hundreds of clients to discover their precise Undertone and optimal color 'Season,' transforming chaotic wardrobes into radiant collections. Today, she will share the secrets to ensure you always glow instead of just... get dressed.\r\n\r\nPlease join me in giving a warm welcome to Personal Color Expert, Besxius!\"",
						FacebookAccount = "https://www.facebook.com/trong.le.26298/"
					}
				},
				//new()
				//{
				//	Email = "ouroborus.free@gmail.com",
				//	Username = "ouroborus.free",
				//	Password = "PerHueDefaultPassword",
				//	Fullname = "Oroborus",
				//	Phone = "0345678917",
				//	Gender = true,
				//	Dob = new DateOnly(1999, 11, 11),
				//	ProfilePicture = string.Empty,
				//	IsActive = true,
				//	IsAitested = false,
				//	RoleId = 4,
				//}
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
        // 1. Các màu: #AEBE89 (Fresh Guacamole), #DAE3BB (Aloe Cream), #FFF7EC (White Lace), #A8EAD5 (Brook Green)
        // Phân tích: Có màu nhạt, sáng, xanh (#A8EAD5), sắc xám nhẹ (#AEBE89). Phù hợp với "Cool Winter" hoặc "Light Summer".
        // Chọn: Cool Winter (1) - vì có cả tông xanh/xám nhạt
        new(){
			ColorTypeId = 1, // Cool Winter
            Colors = allColors.Where(c => c.HexCode == "#AEBE89" ||
										c.HexCode == "#DAE3BB" ||
										c.HexCode == "#FFF7EC" ||
										c.HexCode == "#A8EAD5").ToList()
		},
        // 2. Các màu: #C5B08B (Earthy Cane), #DED5BC (Caraway Seeds), #F8F3E6 (Swan White), #C6CEBE (Frosty Pine)
        // Phân tích: Màu sắc nhẹ nhàng, mờ, có tông ấm nhẹ nhàng của nâu be, và tông xám xanh nhẹ. Phù hợp với Soft Summer hoặc Cool Summer.
        // Chọn: Soft Summer (5) - vì tính chất nhẹ nhàng, mờ và không quá nổi bật.
        new(){
			ColorTypeId = 5, // Soft Summer
            Colors = allColors.Where(c => c.HexCode == "#C5B08B" ||
										c.HexCode == "#DED5BC" ||
										c.HexCode == "#F8F3E6" ||
										c.HexCode == "#C6CEBE").ToList()
		},
        // 3. Các màu: #850121 (Aurora Red), #B1002A (Incarnadine), #EC7A49 (Sizzling Sunset), #800733 (Dark Scarlet Red)
        // Phân tích: Màu sắc đậm, mạnh mẽ, chủ yếu là các tông đỏ sẫm và cam đậm. Phù hợp với Deep Winter hoặc Warm Autumn.
        // Chọn: Deep Winter (2) - do sự đậm và mạnh mẽ của các tông đỏ sẫm.
        new(){
			ColorTypeId = 2, // Deep Winter
            Colors = allColors.Where(c => c.HexCode == "#850121" ||
										c.HexCode == "#B1002A" ||
										c.HexCode == "#EC7A49" ||
										c.HexCode == "#800733").ToList()
		},
        // 4. Các màu: #52A7CC (Frozen Wave), #FBC7C3 (Gossamer Pink), #F6EFE3 (Rustic Cream), #FABB7C (Rich Honey)
        // Phân tích: Màu sắc sáng, tinh khiết, có xanh sáng và hồng đào. Phù hợp với Clear Winter hoặc Light Spring.
        // Chọn: Clear Winter (3) - do tính chất sáng và rõ ràng.
        new(){
			ColorTypeId = 3, // Clear Winter
            Colors = allColors.Where(c => c.HexCode == "#52A7CC" ||
										c.HexCode == "#FBC7C3" ||
										c.HexCode == "#F6EFE3" ||
										c.HexCode == "#FABB7C").ToList()
		},
        // 5. Các màu: #15192F (Christmas Eve), #B11E31 (Christmas Red), #FAF2D1 (Christmas Vanilla), #096344 (Cake)
        // Phân tích: Màu sắc đậm, có tông tối như xanh đen và đỏ đậm, kết hợp với màu sáng ấm. Phù hợp với Deep Winter hoặc Deep Autumn.
        // Chọn: Deep Winter (2) - do sự hiện diện của các màu rất đậm, tối và mạnh mẽ.
        new(){
			ColorTypeId = 2, // Deep Winter
            Colors = allColors.Where(c => c.HexCode == "#15192F" ||
										c.HexCode == "#B11E31" ||
										c.HexCode == "#FAF2D1" ||
										c.HexCode == "#096344").ToList()
		},
        // 6. Các màu: #FCC4C9 (Christmas Tree), #FDF6F0 (Crystal Rose), #F8E2CF (Backlight), #F5C6AA (Sandy Beach)
        // Phân tích: Màu sắc nhẹ nhàng, mờ, chủ yếu là tông hồng nhạt và be. Phù hợp với Soft Summer.
        // Chọn: Soft Summer (5) - vì tính chất dịu dàng, mờ và không quá nổi bật.
        new(){
			ColorTypeId = 5, // Soft Summer
            Colors = allColors.Where(c => c.HexCode == "#FCC4C9" ||
										c.HexCode == "#FDF6F0" ||
										c.HexCode == "#F8E2CF" ||
										c.HexCode == "#F5C6AA").ToList()
		},
        // 7. Các màu: #415A80 (Desert Sand), #A5D4DC (Deep Azure), #F2F4F8 (Midwinter Mist), #D7E2E9 (Snowbelt)
        // Phân tích: Màu sắc lạnh, nhạt, sáng, có sắc xám hoặc xanh. Phù hợp với Cool Winter.
        // Chọn: Cool Winter (1) - do sự kết hợp của xanh đậm/nhạt và xám/trắng lạnh.
        new(){
			ColorTypeId = 1, // Cool Winter
            Colors = allColors.Where(c => c.HexCode == "#415A80" ||
										c.HexCode == "#A5D4DC" ||
										c.HexCode == "#F2F4F8" ||
										c.HexCode == "#D7E2E9").ToList()
		},
        // 8. Các màu: #A9A9C4 (Early Frost), #D0D1E1 (Cosmic Sky), #EBECEF (Hailstorm), #908DB9 (Bright Grey)
        // Phân tích: Màu sắc lạnh, nhạt, sáng, có sắc xám hoặc xanh/tím nhạt. Phù hợp với Cool Winter.
        // Chọn: Cool Winter (1) - tông màu xám tím lạnh và nhạt.
        new(){
			ColorTypeId = 1, // Cool Winter
            Colors = allColors.Where(c => c.HexCode == "#A9A9C4" ||
										c.HexCode == "#D0D1E1" ||
										c.HexCode == "#EBECEF" ||
										c.HexCode == "#908DB9").ToList()
		},
        // 9. Các màu: #E0A39C (Purple Amethyst), #FBC7C3 (Gossamer Pink), #F6EFE3 (Rustic Cream), #FABB7C (Rich Honey)
        // Phân tích: Có sự pha trộn giữa hồng đào/cam ấm và các màu trung tính. Một số màu có vẻ ấm, một số lại trung tính.
        // Chọn: Warm Autumn (7) - do sự hiện diện của các tông hồng/cam ấm và nâu trung tính.
        new(){
			ColorTypeId = 7, // Warm Autumn
            Colors = allColors.Where(c => c.HexCode == "#E0A39C" ||
										c.HexCode == "#FBC7C3" ||
										c.HexCode == "#F6EFE3" ||
										c.HexCode == "#FABB7C").ToList()
		},
        // 10. Các màu: #D4A6D1 (Berrie Popsicle), #F7D9E1 (Pink Frosting), #FBF8F6 (Soft Breeze), #D3DDE6 (Nordic Breeze)
        // Phân tích: Màu sắc dịu nhẹ, tươi sáng nhưng có sắc lạnh nhẹ. Phù hợp với Cool Summer.
        // Chọn: Cool Summer (4) - tông màu hồng tím và xanh xám nhạt, tươi sáng nhưng lạnh.
        new(){
			ColorTypeId = 4, // Cool Summer
            Colors = allColors.Where(c => c.HexCode == "#D4A6D1" ||
										c.HexCode == "#F7D9E1" ||
										c.HexCode == "#FBF8F6" ||
										c.HexCode == "#D3DDE6").ToList()
		},
        // 11. Các màu: #52ADA2 (Emerald Wave), #ADDCCA (Baby Powder), #F7F8F3 (Stem Green), #AADE87 (Wormwood Green)
        // Phân tích: Màu sắc sáng, tinh khiết, rõ ràng, với tông xanh lá cây tươi mát. Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự tươi sáng, rõ ràng và trong trẻo của các màu xanh.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#52ADA2" ||
										c.HexCode == "#ADDCCA" ||
										c.HexCode == "#F7F8F3" ||
										c.HexCode == "#AADE87").ToList()
		},
        // 12. Các màu: #9DB09C (Snowflake), #EEF0F0 (Wayward Willow), #D6D9D0 (Hazel Gaze), #B7BDB0 (Glacial Green)
        // Phân tích: Màu sắc dịu nhẹ, tươi sáng nhưng có sắc lạnh nhẹ, thiên về tông xám xanh. Phù hợp với Cool Summer.
        // Chọn: Cool Summer (4) - tông xám xanh nhẹ nhàng.
        new(){
			ColorTypeId = 4, // Cool Summer
            Colors = allColors.Where(c => c.HexCode == "#9DB09C" ||
										c.HexCode == "#EEF0F0" ||
										c.HexCode == "#D6D9D0" ||
										c.HexCode == "#B7BDB0").ToList()
		},
        // 13. Các màu: #6EB5A5 (Lychee Pulp), #F9F4DB (Caramelized Pears), #E7D6AC (Vintage Red), #A13842 (Hurricane Haze)
        // Phân tích: Màu sắc ấm áp, pha trộn giữa xanh mint, be vàng và đỏ gạch. Phù hợp với Warm Autumn hoặc Warm Spring.
        // Chọn: Warm Autumn (7) - do tông màu đất, đỏ gạch và ấm áp tổng thể.
        new(){
			ColorTypeId = 7, // Warm Autumn
            Colors = allColors.Where(c => c.HexCode == "#6EB5A5" ||
										c.HexCode == "#F9F4DB" ||
										c.HexCode == "#E7D6AC" ||
										c.HexCode == "#A13842").ToList()
		},
        // 14. Các màu: #BDBBAD (Alpine Frost), #E0DED2 (Milk Grass), #FAF8F0 (Winter Frost), #E0DED2 (Milk Grass)
        // Phân tích: Màu sắc nhẹ nhàng, mờ, gần như trung tính, có chút sắc xám. Phù hợp với Soft Summer hoặc Soft Autumn.
        // Chọn: Soft Summer (5) - tông màu trung tính và nhẹ nhàng.
        new(){
			ColorTypeId = 5, // Soft Summer
            Colors = allColors.Where(c => c.HexCode == "#BDBBAD" ||
										c.HexCode == "#E0DED2" ||
										c.HexCode == "#FAF8F0" ||
										c.HexCode == "#E0DED2").ToList()
		},
        // 15. Các màu: #BDBBAD (Alpine Frost), #E0DED2 (Milk Grass), #FAF8F0 (Winter Frost), #E0DED2 (Milk Grass)
        // Phân tích: Tương tự nhóm 14. Màu sắc nhẹ nhàng, mờ, gần như trung tính, có chút sắc xám. Phù hợp với Soft Summer hoặc Soft Autumn.
        // Chọn: Soft Summer (5) - tông màu trung tính và nhẹ nhàng.
        new(){
			ColorTypeId = 5, // Soft Summer
            Colors = allColors.Where(c => c.HexCode == "#BDBBAD" ||
										c.HexCode == "#E0DED2" ||
										c.HexCode == "#FAF8F0" ||
										c.HexCode == "#E0DED2").ToList()
		},
        // 16. Các màu: #D55F8F (Purple Kiss), #F9C4BA (Dark Desire Rose), #FAF8F0 (Winter Frost), #E0DED2 (Milk Grass)
        // Phân tích: Pha trộn giữa tông tím hồng đậm, hồng đào và các màu trung tính. Màu tím hồng có vẻ khá đậm.
        // Chọn: Deep Winter (2) - do sự nổi bật của màu tím hồng đậm và sự đối lập với các màu nhạt.
        new(){
			ColorTypeId = 2, // Deep Winter
            Colors = allColors.Where(c => c.HexCode == "#D55F8F" ||
										c.HexCode == "#F9C4BA" ||
										c.HexCode == "#FAF8F0" ||
										c.HexCode == "#E0DED2").ToList()
		},
        // 17. Các màu: #35063E (Dark Purple), #D11C5B (Bright Rose), #FADFDE (Snowpink), #F1C5C2 (Cupcake Rose)
        // Phân tích: Màu sắc đậm, mạnh mẽ như tím đen và hồng rực, kết hợp với hồng nhạt. Phù hợp với Deep Winter.
        // Chọn: Deep Winter (2) - do sự tương phản mạnh mẽ giữa màu đậm và sáng.
        new(){
			ColorTypeId = 2, // Deep Winter
            Colors = allColors.Where(c => c.HexCode == "#35063E" ||
										c.HexCode == "#D11C5B" ||
										c.HexCode == "#FADFDE" ||
										c.HexCode == "#F1C5C2").ToList()
		},
        // 18. Các màu: #D51D48 (Lusty Lips), #FBC7C3 (Gossamer Pink), #F47983 (Momo Peach), #D55F8F (Purple Kiss)
        // Phân tích: Màu sắc tươi sáng, rực rỡ, chủ yếu là các tông hồng và đỏ. Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự rõ ràng và sống động của các màu.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#D51D48" ||
										c.HexCode == "#FBC7C3" ||
										c.HexCode == "#F47983" ||
										c.HexCode == "#D55F8F").ToList()
		},
        // 19. Các màu: #550D0E (Maroon Oak), #B1002A (Incarnadine), #850121 (Aurora Red), #850121 (Aurora Red)
        // Phân tích: Các màu đậm, mạnh mẽ, tông đỏ sẫm. Phù hợp với Deep Winter.
        // Chọn: Deep Winter (2) - rõ ràng là các tông màu đậm và lạnh của mùa đông.
        new(){
			ColorTypeId = 2, // Deep Winter
            Colors = allColors.Where(c => c.HexCode == "#550D0E" ||
										c.HexCode == "#B1002A" ||
										c.HexCode == "#850121" ||
										c.HexCode == "#850121").ToList()
		},
        // 20. Các màu: #4A2619 (Wild Brown), #C28C2E (Gothic Gold), #D8CDB9 (Vintage Ephemera), #721B29 (Red Berry)
        // Phân tích: Màu sắc đậm, ấm áp, tông nâu, vàng đất và đỏ sẫm. Phù hợp với Deep Autumn.
        // Chọn: Deep Autumn (9) - tông màu đậm của lá thu.
        new(){
			ColorTypeId = 9, // Deep Autumn
            Colors = allColors.Where(c => c.HexCode == "#4A2619" ||
										c.HexCode == "#C28C2E" ||
										c.HexCode == "#D8CDB9" ||
										c.HexCode == "#721B29").ToList()
		},
        // 21. Các màu: #F7D4C8 (Rose Sangria), #FDFAF3 (Floral White), #CFECF5 (Frosty Day), #A9E2F5 (Azure Sky)
        // Phân tích: Màu sắc sáng, tinh khiết, có xanh và hồng nhạt. Phù hợp với Clear Winter hoặc Light Spring.
        // Chọn: Clear Winter (3) - do tính chất sáng và rõ ràng.
        new(){
			ColorTypeId = 3, // Clear Winter
            Colors = allColors.Where(c => c.HexCode == "#F7D4C8" ||
										c.HexCode == "#FDFAF3" ||
										c.HexCode == "#CFECF5" ||
										c.HexCode == "#A9E2F5").ToList()
		},
        // 22. Các màu: #8E6C90 (Dusty Purple), #FCD8CC (Coral Mantle), #FFF7EC (White Lace), #A8EAD5 (Brook Green)
        // Phân tích: Màu sắc dịu nhẹ, tươi sáng nhưng có sắc lạnh nhẹ (tím, xanh). Phù hợp với Cool Summer.
        // Chọn: Cool Summer (4) - do sự kết hợp của tông tím xám, hồng cam nhạt và xanh nhạt.
        new(){
			ColorTypeId = 6, // Cool Summer
            Colors = allColors.Where(c => c.HexCode == "#8E6C90" ||
										c.HexCode == "#FCD8CC" ||
										c.HexCode == "#FFF7EC" ||
										c.HexCode == "#A8EAD5").ToList()
		},
        // 23. Các màu: #550D0E (Maroon Oak), #A13842 (Hurricane Haze), #FFF7EC (White Lace), #DFD7CC (Majestic Beige)
        // Phân tích: Màu sắc đậm (đỏ sẫm) kết hợp với màu trung tính sáng. Có thể là Deep Winter (nếu màu đậm chiếm ưu thế) hoặc Warm Autumn (nếu màu trung tính và ấm).
        // Chọn: Deep Winter (2) - sự mạnh mẽ của hai màu đỏ sẫm nổi bật hơn.
        new(){
			ColorTypeId = 2, // Deep Winter
            Colors = allColors.Where(c => c.HexCode == "#550D0E" ||
										c.HexCode == "#A13842" ||
										c.HexCode == "#FFF7EC" ||
										c.HexCode == "#DFD7CC").ToList()
		},
        // 24. Các màu: #CE8884 (Almond Rose), #FBD3C4 (Coral Dune), #FDF8E8 (Cosmic Latte), #BED5AE (Meadow Grass)
        // Phân tích: Màu sắc ấm áp, pha trộn giữa hồng đào, be kem và xanh lá nhạt. Phù hợp với Warm Spring.
        // Chọn: Warm Spring (10) - do tính chất sáng và ấm áp tổng thể.
        new(){
			ColorTypeId = 10, // Warm Spring
            Colors = allColors.Where(c => c.HexCode == "#CE8884" ||
										c.HexCode == "#FBD3C4" ||
										c.HexCode == "#FDF8E8" ||
										c.HexCode == "#BED5AE").ToList()
		},
        // 25. Các màu: #7A7D68 (Dusty Olive), #F6F3EE (Pearl Sugar), #E0DED2 (Milk Grass), #BDBBAD (Alpine Frost)
        // Phân tích: Màu sắc nhẹ nhàng, mờ, chủ yếu là tông xám xanh và trung tính. Phù hợp với Soft Summer hoặc Soft Autumn.
        // Chọn: Soft Summer (5) - tông màu xám, trung tính và dịu nhẹ.
        new(){
			ColorTypeId = 5, // Soft Summer
            Colors = allColors.Where(c => c.HexCode == "#7A7D68" ||
										c.HexCode == "#F6F3EE" ||
										c.HexCode == "#E0DED2" ||
										c.HexCode == "#BDBBAD").ToList()
		},
        // 26. Các màu: #E0457F (Pink Pickled Turnips), #F9F4DB (Caramelized Pears), #E7D6AC (Vintage Red), #3CAD99 (Green Jelly)
        // Phân tích: Màu sắc tươi sáng, rõ ràng với hồng đậm, vàng be và xanh lá cây. Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự rõ ràng và sống động của các màu.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#E0457F" ||
										c.HexCode == "#F9F4DB" ||
										c.HexCode == "#E7D6AC" ||
										c.HexCode == "#3CAD99").ToList()
		},
        // 27. Các màu: #872657 (Dark Raspberry), #F47983 (Momo Peach), #FFFCEB (Glamour White), #35063E (Dark Purple)
        // Phân tích: Màu sắc đậm và mạnh mẽ, với tông tím/hồng đậm và tím đen. Phù hợp với Deep Winter.
        // Chọn: Deep Winter (2) - do sự đậm, mạnh mẽ và độ tương phản cao.
        new(){
			ColorTypeId = 2, // Deep Winter
            Colors = allColors.Where(c => c.HexCode == "#872657" ||
										c.HexCode == "#F47983" ||
										c.HexCode == "#FFFCEB" ||
										c.HexCode == "#35063E").ToList()
		},
        // 28. Các màu: #89687D (Imaginary Mauve), #FCD8CC (Coral Mantle), #FFF7EC (White Lace), #A7E0E9 (Luminescent Blue)
        // Phân tích: Màu sắc dịu nhẹ, tươi sáng nhưng có sắc lạnh nhẹ (tím, xanh). Phù hợp với Cool Summer.
        // Chọn: Cool Summer (4) - do sự kết hợp của tím xám, hồng cam nhạt và xanh nhạt.
        new(){
			ColorTypeId = 4, // Cool Summer
            Colors = allColors.Where(c => c.HexCode == "#89687D" ||
										c.HexCode == "#FCD8CC" ||
										c.HexCode == "#FFF7EC" ||
										c.HexCode == "#A7E0E9").ToList()
		},
        // 29. Các màu: #F01159 (Red Wrath), #DFF8FE (Winter’s Day), #82CDE5 (Sky Of The Ocean), #003458 (Night Dive)
        // Phân tích: Màu sắc sáng, tinh khiết, không bị pha tông xám, rất rõ ràng (đỏ tươi, xanh trời). Phù hợp với Clear Winter.
        // Chọn: Clear Winter (3) - do sự rõ ràng và tươi sáng, đặc biệt với các màu xanh.
        new(){
			ColorTypeId = 3, // Clear Winter
            Colors = allColors.Where(c => c.HexCode == "#F01159" ||
										c.HexCode == "#DFF8FE" ||
										c.HexCode == "#82CDE5" ||
										c.HexCode == "#003458").ToList()
		},
        // 30. Các màu: #52A7CC (Frozen Wave), #A5D4DC (Deep Azure), #F2F4F8 (Midwinter Mist), #DEE0E0 (Dreamy Cloud)
        // Phân tích: Màu sắc lạnh, nhạt, sáng, chủ yếu là các tông xanh và xám nhạt. Phù hợp với Cool Winter.
        // Chọn: Cool Winter (1) - do sự hiện diện của tông xanh/xám nhạt.
        new(){
			ColorTypeId = 1, // Cool Winter
            Colors = allColors.Where(c => c.HexCode == "#52A7CC" ||
										c.HexCode == "#A5D4DC" ||
										c.HexCode == "#F2F4F8" ||
										c.HexCode == "#DEE0E0").ToList()
		},
        // 31. Các màu: #D8ABB7 (Pink Nectar), #EDD5D8 (Pale Rose), #FAF8F0 (Winter Frost), #E0DED2 (Milk Grass)
        // Phân tích: Màu sắc nhẹ nhàng, mờ, chủ yếu là tông hồng và trung tính. Phù hợp với Soft Summer.
        // Chọn: Soft Summer (5) - tính chất nhẹ nhàng, mờ.
        new(){
			ColorTypeId = 5, // Soft Summer
            Colors = allColors.Where(c => c.HexCode == "#D8ABB7" ||
										c.HexCode == "#EDD5D8" ||
										c.HexCode == "#FAF8F0" ||
										c.HexCode == "#E0DED2").ToList()
		},
        // 32. Các màu: #F57B51 (Bonfire), #FBBC58 (Golden Rambler), #F7F8E2 (Apple White), #DFDECA (Moorland Mist)
        // Phân tích: Màu sắc sáng và ấm áp, chủ yếu là cam, vàng và các tông be ấm. Phù hợp với Warm Spring.
        // Chọn: Warm Spring (10) - do tính chất sáng và ấm áp.
        new(){
			ColorTypeId = 10, // Warm Spring
            Colors = allColors.Where(c => c.HexCode == "#F57B51" ||
										c.HexCode == "#FBBC58" ||
										c.HexCode == "#F7F8E2" ||
										c.HexCode == "#DFDECA").ToList()
		},
        // 33. Các màu: #F4D59A (Honey Butter), #DE8626 (French Toast), #C93D21 (Tomato Soup), #8E1A02 (Tomato Sauce)
        // Phân tích: Màu sắc ấm áp, đậm dần từ vàng sang cam và đỏ nâu đậm. Phù hợp với Warm Autumn hoặc Deep Autumn.
        // Chọn: Warm Autumn (7) - do sự chuyển tông từ sáng ấm sang đậm ấm.
        new(){
			ColorTypeId = 7, // Warm Autumn
            Colors = allColors.Where(c => c.HexCode == "#F4D59A" ||
										c.HexCode == "#DE8626" ||
										c.HexCode == "#C93D21" ||
										c.HexCode == "#8E1A02").ToList()
		},
        // 34. Các màu: #563A3D (Chocolate Eclair), #9A4D36 (Dark Marmalade), #E0D6C3 (Toasted Barley Flakes), #B4977B (Gingerbread Latte)
        // Phân tích: Màu sắc đậm, ấm áp, tông nâu sô cô la và đất. Phù hợp với Deep Autumn.
        // Chọn: Deep Autumn (9) - do sự đậm và ấm áp của tông nâu.
        new(){
			ColorTypeId = 9, // Deep Autumn
            Colors = allColors.Where(c => c.HexCode == "#563A3D" ||
										c.HexCode == "#9A4D36" ||
										c.HexCode == "#E0D6C3" ||
										c.HexCode == "#B4977B").ToList()
		},
        // 35. Các màu: #C6ADAD (Pinkish Grey), #F6F0F3 (Grim White), #DFD7DB (Shy Pink), #CABBBE (Blush Grey Rose)
        // Phân tích: Màu sắc nhẹ nhàng, mờ, tông xám hồng và trung tính. Phù hợp với Soft Summer.
        // Chọn: Soft Summer (5) - tính chất nhẹ nhàng, mờ và có sắc xám.
        new(){
			ColorTypeId = 5, // Soft Summer
            Colors = allColors.Where(c => c.HexCode == "#C6ADAD" ||
										c.HexCode == "#F6F0F3" ||
										c.HexCode == "#DFD7DB" ||
										c.HexCode == "#CABBBE").ToList()
		},
        // 36. Các màu: #C62644 (Pepper Jelly), #F3AC61 (Blazing Autumn), #F6EFE3 (Rustic Cream), #8AD2C6 (Green Daze)
        // Phân tích: Pha trộn giữa đỏ tươi, cam ấm và xanh mint nhẹ. Màu đỏ khá rực rỡ. Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự rõ ràng và tương phản.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#C62644" ||
										c.HexCode == "#F3AC61" ||
										c.HexCode == "#F6EFE3" ||
										c.HexCode == "#8AD2C6").ToList()
		},
        // 37. Các màu: #9CA1B3 (Majestic Mist), #CFD1DA (Naturally Calm), #EBECEF (Hailstorm), #9593A4 (Ash Violet)
        // Phân tích: Màu sắc lạnh, nhạt, sáng, chủ yếu là tông xám và tím nhạt. Phù hợp với Cool Winter.
        // Chọn: Cool Winter (1) - do sự hiện diện của tông xám tím lạnh.
        new(){
			ColorTypeId = 1, // Cool Winter
            Colors = allColors.Where(c => c.HexCode == "#9CA1B3" ||
										c.HexCode == "#CFD1DA" ||
										c.HexCode == "#EBECEF" ||
										c.HexCode == "#9593A4").ToList()
		},
        // 38. Các màu: #B8223C (Maple Red), #F3B062 (Pumpkin Latte), #90772F (Dying Leaves), #402320 (Dark Oak)
        // Phân tích: Màu sắc đậm, ấm áp, tông đỏ, cam, vàng và nâu sẫm. Phù hợp với Deep Autumn.
        // Chọn: Deep Autumn (9) - do tính chất đậm và màu lá thu.
        new(){
			ColorTypeId = 9, // Deep Autumn
            Colors = allColors.Where(c => c.HexCode == "#B8223C" ||
										c.HexCode == "#F3B062" ||
										c.HexCode == "#90772F" ||
										c.HexCode == "#402320").ToList()
		},
        // 39. Các màu: #CAA6DB (Wisteria), #FBC7C3 (Gossamer Pink), #F7F4E7 (Risotto), #B6DF82 (Green Chalk)
        // Phân tích: Màu sắc sáng, nhẹ nhàng, với tông tím, hồng và xanh lá cây. Phù hợp với Light Spring.
        // Chọn: Light Spring (11) - do tính chất nhẹ nhàng, sáng và thanh thoát.
        new(){
			ColorTypeId = 11, // Light Spring
            Colors = allColors.Where(c => c.HexCode == "#CAA6DB" ||
										c.HexCode == "#FBC7C3" ||
										c.HexCode == "#F7F4E7" ||
										c.HexCode == "#B6DF82").ToList()
		},
        // 40. Các màu: #721B29 (Red Berry), #9D653F (Cinnamon Baked), #DFD8CB (Powder Cake), #22314A (Blueberry Bark)
        // Phân tích: Màu sắc đậm, ấm áp (đỏ, nâu) kết hợp với màu trung tính và xanh đen. Phù hợp với Deep Autumn hoặc Deep Winter.
        // Chọn: Deep Autumn (9) - sự ấm áp của nâu và đỏ đậm là nổi bật.
        new(){
			ColorTypeId = 8, // Soft Autumn
            Colors = allColors.Where(c => c.HexCode == "#721B29" ||
										c.HexCode == "#9D653F" ||
										c.HexCode == "#DFD8CB" ||
										c.HexCode == "#22314A").ToList()
		},
        // 41. Các màu: #52A7CC (Frozen Wave), #FFF7EC (White Lace), #A8EAD5 (Brook Green), #51C9C2 (Jazzy Jade)
        // Phân tích: Màu sắc sáng, tinh khiết, chủ yếu là các tông xanh và trắng. Phù hợp với Clear Winter.
        // Chọn: Clear Winter (3) - do tính chất sáng và rõ ràng của các màu xanh.
        new(){
			ColorTypeId = 3, // Clear Winter
            Colors = allColors.Where(c => c.HexCode == "#52A7CC" ||
										c.HexCode == "#FFF7EC" ||
										c.HexCode == "#A8EAD5" ||
										c.HexCode == "#51C9C2").ToList()
		},
        // 42. Các màu: #302621 (Wood Bark), #CBBDAD (Creamy Mushroom), #984F0E (Dry Maple Leaf), #4A2619 (Wild Brown)
        // Phân tích: Màu sắc đậm, ấm áp, tông nâu gỗ và nâu đất. Phù hợp với Deep Autumn.
        // Chọn: Deep Autumn (9) - do tính chất đậm và màu đất.
        new(){
			ColorTypeId = 9, // Deep Autumn
            Colors = allColors.Where(c => c.HexCode == "#302621" ||
										c.HexCode == "#CBBDAD" ||
										c.HexCode == "#984F0E" ||
										c.HexCode == "#4A2619").ToList()
		},
        // 43. Các màu: #8D5F5B (Rose Taupe), #D8A194 (Auburn Wave), #F9E7D8 (Aria Ivory), #ECC0A5 (Veronese Peach)
        // Phân tích: Màu sắc ấm áp, dịu nhẹ, tông hồng đất và cam đào. Phù hợp với Warm Autumn hoặc Soft Autumn.
        // Chọn: Warm Autumn (7) - do sự ấm áp và màu đất.
        new(){
			ColorTypeId = 7, // Warm Autumn
            Colors = allColors.Where(c => c.HexCode == "#8D5F5B" ||
										c.HexCode == "#D8A194" ||
										c.HexCode == "#F9E7D8" ||
										c.HexCode == "#ECC0A5").ToList()
		},
        // 44. Các màu: #080E2C (Nyctophile Blue), #44D6E9 (Blue Dacnis), #ECF4F1 (Chilly White), #D1E0DB (Morning Breeze)
        // Phân tích: Màu sắc lạnh, nhạt, sáng, chủ yếu là các tông xanh và trắng. Phù hợp với Cool Winter.
        // Chọn: Cool Winter (1) - do sự hiện diện của tông xanh/trắng lạnh.
        new(){
			ColorTypeId = 1, // Cool Winter
            Colors = allColors.Where(c => c.HexCode == "#080E2C" ||
										c.HexCode == "#44D6E9" ||
										c.HexCode == "#ECF4F1" ||
										c.HexCode == "#D1E0DB").ToList()
		},
        // 45. Các màu: #342E37 (Black Safflower), #DEA81C (Chanterelle), #ECDCC4 (Wild Oats), #8E1533 (Sweet Cherry Red)
        // Phân tích: Màu sắc đậm, có sự pha trộn giữa đen/tím than, vàng đất và đỏ sẫm. Phù hợp với Deep Autumn.
        // Chọn: Deep Autumn (9) - do sự đậm và tính chất màu đất.
        new(){
			ColorTypeId = 9, // Deep Autumn
            Colors = allColors.Where(c => c.HexCode == "#342E37" ||
										c.HexCode == "#DEA81C" ||
										c.HexCode == "#ECDCC4" ||
										c.HexCode == "#8E1533").ToList()
		},
        // 46. Các màu: #762014 (Rosewood), #D23520 (Raw Sunset), #F9EFE5 (Anemone White), #E57B30 (Novelle Peach)
        // Phân tích: Màu sắc ấm áp, tông đỏ gạch, cam và trắng ngà. Phù hợp với Warm Autumn.
        // Chọn: Warm Autumn (7) - do tính chất ấm áp và màu đất.
        new(){
			ColorTypeId = 7, // Warm Autumn
            Colors = allColors.Where(c => c.HexCode == "#762014" ||
										c.HexCode == "#D23520" ||
										c.HexCode == "#F9EFE5" ||
										c.HexCode == "#E57B30").ToList()
		},
        // 47. Các màu: #550D0E (Maroon Oak), #EC7A49 (Sizzling Sunset), #F9EFE5 (Anemone White), #EBCCB9 (Scotchtone)
        // Phân tích: Sự kết hợp giữa màu đỏ sẫm mạnh mẽ, cam tươi và các tông be trung tính. Phù hợp với Warm Autumn hoặc Deep Winter.
        // Chọn: Warm Autumn (7) - do sự hiện diện của cam tươi và be ấm.
        new(){
			ColorTypeId = 7, // Warm Autumn
            Colors = allColors.Where(c => c.HexCode == "#550D0E" ||
										c.HexCode == "#EC7A49" ||
										c.HexCode == "#F9EFE5" ||
										c.HexCode == "#EBCCB9").ToList()
		},
        // 48. Các màu: #D186A0 (Heirloom Rose), #FDC2B1 (Tropical Peach), #FDF8E8 (Cosmic Latte), #AAF0D1 (Magic Mint)
        // Phân tích: Màu sắc sáng, nhẹ nhàng, với tông hồng, cam đào, be và xanh mint. Phù hợp với Light Spring.
        // Chọn: Warm Spring (10) - do tính chất nhẹ nhàng, sáng và thanh thoát.
        new(){
			ColorTypeId = 10, // Warm Spring
            Colors = allColors.Where(c => c.HexCode == "#D186A0" ||
										c.HexCode == "#FDC2B1" ||
										c.HexCode == "#FDF8E8" ||
										c.HexCode == "#AAF0D1").ToList()
		},
        // 49. Các màu: #9CADB3 (Calestra Grey), #CFDADA (Offshore Mint), #EBF2F2 (Frosted Glass), #BBD5D8 (Rhythmic Blue)
        // Phân tích: Màu sắc lạnh, nhạt, sáng, chủ yếu là tông xám và xanh nhạt. Phù hợp với Cool Winter.
        // Chọn: Cool Winter (1) - do sự hiện diện của tông xám và xanh nhạt lạnh.
        new(){
			ColorTypeId = 1, // Cool Winter
            Colors = allColors.Where(c => c.HexCode == "#9CADB3" ||
										c.HexCode == "#CFDADA" ||
										c.HexCode == "#EBF2F2" ||
										c.HexCode == "#BBD5D8").ToList()
		},
        // 50. Các màu: #CB6347 (Great Canyon), #DFCBB2 (Cashew Delight), #CC9972 (Dulce De Leche), #8C533D (Cocoa Dust)
        // Phân tích: Màu sắc ấm áp, tông cam đất và nâu. Phù hợp với Warm Autumn.
        // Chọn: Warm Autumn (7) - do tính chất ấm áp và màu đất.
        new(){
			ColorTypeId = 7, // Warm Autumn
            Colors = allColors.Where(c => c.HexCode == "#CB6347" ||
										c.HexCode == "#DFCBB2" ||
										c.HexCode == "#CC9972" ||
										c.HexCode == "#8C533D").ToList()
		},
        // 51. Các màu: #721B29 (Red Berry), #B33B44 (Cherry Bomb), #F1E7D6 (Rice Bowl), #DABE85 (Barley Seeds)
        // Phân tích: Màu sắc đậm (đỏ berry) kết hợp với màu trung tính ấm. Phù hợp với Deep Autumn.
        // Chọn: Deep Autumn (9) - do sự kết hợp của màu đỏ đậm và tông ấm.
        new(){
			ColorTypeId = 9, // Deep Autumn
            Colors = allColors.Where(c => c.HexCode == "#721B29" ||
										c.HexCode == "#B33B44" ||
										c.HexCode == "#F1E7D6" ||
										c.HexCode == "#DABE85").ToList()
		},
        // 52. Các màu: #462B45 (Shadow Purple), #884A6F (Twilight Lavender), #F5EAEF (Snow Plum), #D0BCC4 (Rose Marble)
        // Phân tích: Màu sắc dịu nhẹ, tươi sáng nhưng có sắc lạnh nhẹ, tông tím hồng xám. Phù hợp với Cool Summer.
        // Chọn: Cool Summer (4) - do sự mềm mại và sắc lạnh.
        new(){
			ColorTypeId = 4, // Cool Summer
            Colors = allColors.Where(c => c.HexCode == "#462B45" ||
										c.HexCode == "#884A6F" ||
										c.HexCode == "#F5EAEF" ||
										c.HexCode == "#D0BCC4").ToList()
		},
        // 53. Các màu: #B7DF69 (Archipelago Green), #F4F1EC (Shallow White), #9EEBE2 (Shallow Blue), #1FD8D8 (Beachy Blue)
        // Phân tích: Màu sắc sáng, tinh khiết, rõ ràng, với tông xanh lá và xanh biển tươi mát. Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự rõ ràng và sống động.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#B7DF69" ||
										c.HexCode == "#F4F1EC" ||
										c.HexCode == "#9EEBE2" ||
										c.HexCode == "#1FD8D8").ToList()
		},
        // 54. Các màu: #51C9C2 (Jazzy Jade), #EEE9CF (Ambrosia Cake), #FCFFF5 (Whitewash), #FEBAC6 (Cherry Blossom Pink)
        // Phân tích: Màu sắc sáng, nhẹ nhàng, với tông xanh mint, be vàng và hồng nhạt. Phù hợp với Light Spring.
        // Chọn: Warm Spring (10) - do tính chất nhẹ nhàng, sáng và thanh thoát.
        new(){
			ColorTypeId = 6, // Warm Spring
            Colors = allColors.Where(c => c.HexCode == "#51C9C2" ||
										c.HexCode == "#EEE9CF" ||
										c.HexCode == "#FCFFF5" ||
										c.HexCode == "#FEBAC6").ToList()
		},
        // 55. Các màu: #182F53 (Storm Blue), #F9EEE2 (Alpaca Wool), #F57A4D (Coral Rose), #AEBE89 (Fresh Guacamole)
        // Phân tích: Màu sắc đậm (xanh biển) kết hợp với tông ấm (hồng cam) và màu trung tính. Khó phân loại rõ ràng.
        // Chọn: Warm Autumn (7) - do có màu cam san hô và xanh rêu ấm áp.
        new(){
			ColorTypeId = 7, // Warm Autumn
            Colors = allColors.Where(c => c.HexCode == "#182F53" ||
										c.HexCode == "#F9EEE2" ||
										c.HexCode == "#F57A4D" ||
										c.HexCode == "#AEBE89").ToList()
		},
        // 56. Các màu: #444251 (Smoked Purple), #8D89A3 (Lavender Haze), #E4E3E5 (Violet Hush), #CBCBCF (Ghostly Grey)
        // Phân tích: Màu sắc lạnh, nhạt, sáng, chủ yếu là tông xám và tím nhạt. Phù hợp với Cool Winter.
        // Chọn: Cool Winter (1) - do sự hiện diện của tông xám và tím nhạt lạnh.
        new(){
			ColorTypeId = 1, // Cool Winter
            Colors = allColors.Where(c => c.HexCode == "#444251" ||
										c.HexCode == "#8D89A3" ||
										c.HexCode == "#E4E3E5" ||
										c.HexCode == "#CBCBCF").ToList()
		},
        // 57. Các màu: #F5CDE6 (Sparkling Pink), #FAF8F0 (Winter Frost), #CFF5EA (Snow Mint), #A7E9E1 (Mint Macaroon)
        // Phân tích: Màu sắc sáng, tinh khiết, với tông hồng nhạt và xanh mint. Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự rõ ràng và tươi sáng.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#F5CDE6" ||
										c.HexCode == "#FAF8F0" ||
										c.HexCode == "#CFF5EA" ||
										c.HexCode == "#A7E9E1").ToList()
		},
        // 58. Các màu: #E3BCBC (Soft Blush), #F4F4F4 (White Smoke), #E0E0E0 (Silver Lake), #C4C6C8 (Glacier Grey)
        // Phân tích: Màu sắc nhẹ nhàng, mờ, tông hồng nhạt và các tông xám trung tính. Phù hợp với Soft Summer.
        // Chọn: Soft Summer (5) - tính chất nhẹ nhàng, mờ.
        new(){
			ColorTypeId = 5, // Soft Summer
            Colors = allColors.Where(c => c.HexCode == "#E3BCBC" ||
										c.HexCode == "#F4F4F4" ||
										c.HexCode == "#E0E0E0" ||
										c.HexCode == "#C4C6C8").ToList()
		},
        // 59. Các màu: #BEBCCB (Blue Haze), #F1EFF2 (White Lilac), #DDD7DC (Lilac Frost), #A994A7 (Heliotrope Grey)
        // Phân tích: Màu sắc lạnh, nhạt, sáng, chủ yếu là tông xám và tím nhạt. Phù hợp với Cool Winter.
        // Chọn: Cool Winter (1) - do sự hiện diện của tông xám tím lạnh.
        new(){
			ColorTypeId = 1, // Cool Winter
            Colors = allColors.Where(c => c.HexCode == "#BEBCCB" ||
										c.HexCode == "#F1EFF2" ||
										c.HexCode == "#DDD7DC" ||
										c.HexCode == "#A994A7").ToList()
		},
        // 60. Các màu: #F1E1C9 (Brem Cake), #FAF8F0 (Winter Frost), #CFECF5 (Frosty Day), #A9E2F5 (Azure Sky)
        // Phân tích: Màu sắc sáng, tinh khiết, với tông be, trắng và xanh trời. Phù hợp với Clear Winter hoặc Light Spring.
        // Chọn: Clear Winter (3) - do tính chất sáng và rõ ràng.
        new(){
			ColorTypeId = 3, // Clear Winter
            Colors = allColors.Where(c => c.HexCode == "#F1E1C9" ||
										c.HexCode == "#FAF8F0" ||
										c.HexCode == "#CFECF5" ||
										c.HexCode == "#A9E2F5").ToList()
		},
        // 61. Các màu: #3A3C42 (Antarctic Deep), #9CCD62 (Woodland), #F7F8E2 (Apple White), #DFDECA (Moorland Mist)
        // Phân tích: Sự kết hợp giữa màu xám than đậm, xanh lá cây tươi và các tông be ấm. Phù hợp với Deep Autumn hoặc Warm Autumn.
        // Chọn: Warm Autumn (7) - do sự cân bằng giữa màu đậm và các tông ấm áp tự nhiên.
        new(){
			ColorTypeId = 7, // Warm Autumn
            Colors = allColors.Where(c => c.HexCode == "#3A3C42" ||
										c.HexCode == "#9CCD62" ||
										c.HexCode == "#F7F8E2" ||
										c.HexCode == "#DFDECA").ToList()
		},
        // 62. Các màu: #F57B51 (Bonfire), #FDF6F0 (Crystal Rose), #FBBC58 (Golden Rambler), #095D6A (Nocturnal Sea)
        // Phân tích: Màu sắc rực rỡ, ấm áp (cam, vàng) kết hợp với xanh biển đậm. Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự tươi sáng và độ rõ nét của các màu.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#F57B51" ||
										c.HexCode == "#FDF6F0" ||
										c.HexCode == "#FBBC58" ||
										c.HexCode == "#095D6A").ToList()
		},
        // 63. Các màu: #72D2E3 (Aquamarine Blue), #A7E0E9 (Luminescent Blue), #FAF8ED (White Desert), #CAAAF3 (Lavender Cream)
        // Phân tích: Màu sắc sáng, nhẹ nhàng, tinh khiết, chủ yếu là tông xanh và tím nhạt. Phù hợp với Light Spring.
        // Chọn: Light Spring (11) - do tính chất nhẹ nhàng, sáng và thanh thoát.
        new(){
			ColorTypeId = 11, // Light Spring
            Colors = allColors.Where(c => c.HexCode == "#72D2E3" ||
										c.HexCode == "#A7E0E9" ||
										c.HexCode == "#FAF8ED" ||
										c.HexCode == "#CAAAF3").ToList()
		},
        // 64. Các màu: #56514B (Heavy Charcoal), #E7E5DD (Mystic Mist), #BDBBAD (Alpine Frost), #999990 (Concrete Jungle)
        // Phân tích: Màu sắc nhẹ nhàng, mờ, chủ yếu là tông xám trung tính và xanh xám nhạt. Phù hợp với Soft Summer.
        // Chọn: Soft Summer (5) - tính chất nhẹ nhàng, mờ và sắc xám.
        new(){
			ColorTypeId = 5, // Soft Summer
            Colors = allColors.Where(c => c.HexCode == "#56514B" ||
										c.HexCode == "#E7E5DD" ||
										c.HexCode == "#BDBBAD" ||
										c.HexCode == "#999990").ToList()
		},
        // 65. Các màu: #AA6890 (Purple Starburst), #FCACC7 (Taffy Pink), #F9EDED (Girly Nursery), #BDADAE (Antique Mauve)
        // Phân tích: Màu sắc dịu nhẹ, tươi sáng nhưng có sắc lạnh nhẹ, tông hồng tím và hồng nhạt. Phù hợp với Cool Summer.
        // Chọn: Cool Summer (4) - do sự mềm mại và sắc lạnh.
        new(){
			ColorTypeId = 6, // Cool Summer
            Colors = allColors.Where(c => c.HexCode == "#AA6890" ||
										c.HexCode == "#FCACC7" ||
										c.HexCode == "#F9EDED" ||
										c.HexCode == "#BDADAE").ToList()
		},
        // 66. Các màu: #2F1A35 (Blackcurrant), #FF5976 (Watermelon Candy), #FFF6FA (Lavender Blush), #FDDBDB (Aquarelle Red)
        // Phân tích: Màu sắc đậm (tím đen) kết hợp với màu sáng rực (hồng, đỏ). Phù hợp với Deep Winter.
        // Chọn: Deep Winter (2) - do sự tương phản mạnh mẽ giữa màu đậm và sáng.
        new(){
			ColorTypeId = 2, // Deep Winter
            Colors = allColors.Where(c => c.HexCode == "#2F1A35" ||
										c.HexCode == "#FF5976" ||
										c.HexCode == "#FFF6FA" ||
										c.HexCode == "#FDDBDB").ToList()
		},
        // 67. Các màu: #CDD5DC (Illusion Blue), #F4F4F4 (White Smoke), #E0E0E0 (Silver Lake), #B2B6BB (Pearl Grey)
        // Phân tích: Màu sắc lạnh, nhạt, sáng, chủ yếu là tông xám và xanh nhạt. Phù hợp với Cool Winter.
        // Chọn: Cool Winter (1) - do sự hiện diện của tông xám và xanh nhạt lạnh.
        new(){
			ColorTypeId = 1, // Cool Winter
            Colors = allColors.Where(c => c.HexCode == "#CDD5DC" ||
										c.HexCode == "#F4F4F4" ||
										c.HexCode == "#E0E0E0" ||
										c.HexCode == "#B2B6BB").ToList()
		},
        // 68. Các màu: #E9687E (Ambitious Rose), #FDC2B1 (Tropical Peach), #FDFAF3 (Floral White), #F7E298 (Lemon Meringue)
        // Phân tích: Màu sắc sáng, nhẹ nhàng, với tông hồng, cam đào, trắng và vàng nhạt. Phù hợp với Light Spring.
        // Chọn: Light Spring (11) - do tính chất nhẹ nhàng, sáng và thanh thoát.
        new(){
			ColorTypeId = 11, // Light Spring
            Colors = allColors.Where(c => c.HexCode == "#E9687E" ||
										c.HexCode == "#FDC2B1" ||
										c.HexCode == "#FDFAF3" ||
										c.HexCode == "#F7E298").ToList()
		},
        // 69. Các màu: #222220 (Coal Black), #C28C2E (Gothic Gold), #D8CDB9 (Vintage Ephemera), #721B29 (Red Berry)
        // Phân tích: Màu sắc đậm, ấm áp, tông đen, vàng đất và đỏ sẫm. Phù hợp với Deep Autumn.
        // Chọn: Deep Autumn (9) - do tính chất đậm và màu đất.
        new(){
			ColorTypeId = 9, // Deep Autumn
            Colors = allColors.Where(c => c.HexCode == "#222220" ||
										c.HexCode == "#C28C2E" ||
										c.HexCode == "#D8CDB9" ||
										c.HexCode == "#721B29").ToList()
		},
        // 70. Các màu: #F3B8D9 (Soft Cashmere), #F7F4E7 (Risotto), #F7D4C8 (Rose Sangria), #64D7EB (Turquoise Blue)
        // Phân tích: Màu sắc sáng, nhẹ nhàng, với tông hồng nhạt, be và xanh ngọc. Phù hợp với Light Spring.
        // Chọn: Light Spring (11) - do tính chất nhẹ nhàng, sáng và thanh thoát.
        new(){
			ColorTypeId = 11, // Light Spring
            Colors = allColors.Where(c => c.HexCode == "#F3B8D9" ||
										c.HexCode == "#F7F4E7" ||
										c.HexCode == "#F7D4C8" ||
										c.HexCode == "#64D7EB").ToList()
		},
        // 71. Các màu: #9C7627 (Luxor Gold), #C9BA86 (Olive Hint), #E8E3CC (Dirty White), #9C4A29 (Cinnamon Stick)
        // Phân tích: Màu sắc ấm áp, tông vàng đất, xanh olive và nâu quế. Phù hợp với Warm Autumn.
        // Chọn: Warm Autumn (7) - do tính chất ấm áp và màu đất.
        new(){
			ColorTypeId = 8, // Soft Autumn
            Colors = allColors.Where(c => c.HexCode == "#9C7627" ||
										c.HexCode == "#C9BA86" ||
										c.HexCode == "#E8E3CC" ||
										c.HexCode == "#9C4A29").ToList()
		},
        // 72. Các màu: #F340AF (Royal Fuchsia), #FBDB14 (Bright Sun), #FFFCEB (Glamour White), #01F9C6 (Bright Teal)
        // Phân tích: Màu sắc sáng, tinh khiết, không bị pha tông xám, rất rõ ràng (hồng rực, vàng sáng, xanh ngọc). Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự rực rỡ và rõ nét.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#F340AF" ||
										c.HexCode == "#FBDB14" ||
										c.HexCode == "#FFFCEB" ||
										c.HexCode == "#01F9C6").ToList()
		},
        // 73. Các màu: #DD84A1 (Shimmering Blush), #FFBBBE (Matte Pink), #FDF8E8 (Cosmic Latte), #CBE3B3 (Beryl Green)
        // Phân tích: Màu sắc sáng, nhẹ nhàng, với tông hồng nhạt, be và xanh lá nhạt. Phù hợp với Light Spring.
        // Chọn: Light Spring (11) - do tính chất nhẹ nhàng, sáng và thanh thoát.
        new(){
			ColorTypeId = 11, // Light Spring
            Colors = allColors.Where(c => c.HexCode == "#DD84A1" ||
										c.HexCode == "#FFBBBE" ||
										c.HexCode == "#FDF8E8" ||
										c.HexCode == "#CBE3B3").ToList()
		},
        // 74. Các màu: #4F3B38 (Cocoa Bean), #C9C1B0 (Malibu Beige), #E5E1D8 (Foggy Day), #9C6F69 (Burlwood)
        // Phân tích: Màu sắc đậm, ấm áp, tông nâu đất và be. Phù hợp với Warm Autumn.
        // Chọn: Warm Autumn (7) - do tính chất ấm áp và màu đất.
        new(){
			ColorTypeId = 7, // Warm Autumn
            Colors = allColors.Where(c => c.HexCode == "#4F3B38" ||
										c.HexCode == "#C9C1B0" ||
										c.HexCode == "#E5E1D8" ||
										c.HexCode == "#9C6F69").ToList()
		},
        // 75. Các màu: #171D4B (Dark Indigo), #2ED3C6 (Turquoise Gemstone), #F8F5E6 (Diamond Dust), #AFE19F (Green Gooseberry)
        // Phân tích: Màu sắc sáng, tinh khiết, không bị pha tông xám, rất rõ ràng (xanh indigo, xanh ngọc, xanh lá). Phù hợp với Clear Winter.
        // Chọn: Clear Winter (3) - do tính chất rõ ràng và lạnh của các màu.
        new(){
			ColorTypeId = 3, // Clear Winter
            Colors = allColors.Where(c => c.HexCode == "#171D4B" ||
										c.HexCode == "#2ED3C6" ||
										c.HexCode == "#F8F5E6" ||
										c.HexCode == "#AFE19F").ToList()
		},
        // 76. Các màu: #5FDED7 (Light Turquoise), #FFFDF8 (Vanilla Shake), #FFDC8E (Midday Sun), #E22A77 (Pink Sapphire)
        // Phân tích: Màu sắc sáng, tinh khiết, không bị pha tông xám, rất rõ ràng (xanh ngọc, vàng, hồng). Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự rực rỡ và rõ nét.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#5FDED7" ||
										c.HexCode == "#FFFDF8" ||
										c.HexCode == "#FFDC8E" ||
										c.HexCode == "#E22A77").ToList()
		},
        // 77. Các màu: #575965 (Midnight Badger), #C3C4C8 (Quiet Grey), #F8F8F6 (Classic Chalk), #939498 (Wolf Grey)
        // Phân tích: Màu sắc lạnh, nhạt, sáng, chủ yếu là tông xám. Phù hợp với Cool Winter.
        // Chọn: Cool Winter (1) - do sự hiện diện của tông xám lạnh.
        new(){
			ColorTypeId = 1, // Cool Winter
            Colors = allColors.Where(c => c.HexCode == "#575965" ||
										c.HexCode == "#C3C4C8" ||
										c.HexCode == "#F8F8F6" ||
										c.HexCode == "#939498").ToList()
		},
        // 78. Các màu: #FA376C (Neon Fuchsia), #FFA780 (Peach Fizz), #FAF8E0 (Clear Moon), #9BEEC1 (Nightly Aurora)
        // Phân tích: Màu sắc sáng, tinh khiết, không bị pha tông xám, rất rõ ràng (hồng neon, cam, vàng nhạt, xanh mint). Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự rực rỡ và rõ nét.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#FA376C" ||
										c.HexCode == "#FFA780" ||
										c.HexCode == "#FAF8E0" ||
										c.HexCode == "#9BEEC1").ToList()
		},
        // 79. Các màu: #F03E9E (Majestic Magenta), #C0E876 (Pisco Sour), #F7F4E7 (Risotto), #FAD2AD (Satin Latour)
        // Phân tích: Màu sắc sáng, tinh khiết, không bị pha tông xám, rất rõ ràng (hồng magenta, xanh lá, be). Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự rực rỡ và rõ nét.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#F03E9E" ||
										c.HexCode == "#C0E876" ||
										c.HexCode == "#F7F4E7" ||
										c.HexCode == "#FAD2AD").ToList()
		},
        // 80. Các màu: #A4647F (Strawberry Mousse), #F8ADB5 (Pink Blush), #FFF6FA (Lavender Blush), #F8D8D8 (Pink Dogwood)
        // Phân tích: Màu sắc nhẹ nhàng, mờ, chủ yếu là tông hồng và tím nhạt. Phù hợp với Soft Summer.
        // Chọn: Soft Summer (5) - tính chất nhẹ nhàng, mờ.
        new(){
			ColorTypeId = 5, // Soft Summer
            Colors = allColors.Where(c => c.HexCode == "#A4647F" ||
										c.HexCode == "#F8ADB5" ||
										c.HexCode == "#FFF6FA" ||
										c.HexCode == "#F8D8D8").ToList()
		},
        // 81. Các màu: #452B30 (Aubergine), #74404C (Brownish Purple), #F3ECD8 (Parchment), #C7C79E (Watercress Pesto)
        // Phân tích: Màu sắc đậm, ấm áp (tím nâu) kết hợp với màu trung tính ấm (be, xanh lá). Phù hợp với Deep Autumn.
        // Chọn: Deep Autumn (9) - do tính chất đậm và màu đất.
        new(){
			ColorTypeId = 8, // Soft Autumn
            Colors = allColors.Where(c => c.HexCode == "#452B30" ||
										c.HexCode == "#74404C" ||
										c.HexCode == "#F3ECD8" ||
										c.HexCode == "#C7C79E").ToList()
		},
        // 82. Các màu: #631647 (Plum Purple), #FF8B0D (Autumn Glory), #FDFFF0 (Milk Soda), #E3E8CD (Frozen Margarita)
        // Phân tích: Màu sắc đậm (tím mận) kết hợp với màu sáng ấm (cam, vàng kem, xanh lá nhạt). Phù hợp với Deep Autumn.
        // Chọn: Deep Autumn (9) - do sự hiện diện của tím mận và cam đất.
        new(){
			ColorTypeId = 9, // Deep Autumn
            Colors = allColors.Where(c => c.HexCode == "#631647" ||
										c.HexCode == "#FF8B0D" ||
										c.HexCode == "#FDFFF0" ||
										c.HexCode == "#E3E8CD").ToList()
		},
        // 83. Các màu: #97D7CB (Tropical Freeze), #FCF2C7 (Spring Lily), #DBC797 (Chamomile Tea), #9C765D (Chamoisee)
        // Phân tích: Màu sắc sáng, nhẹ nhàng, với tông xanh mint, vàng kem và nâu đất nhẹ. Phù hợp với Light Spring.
        // Chọn: Warm Spring (10) - do tính chất nhẹ nhàng, sáng và thanh thoát.
        new(){
			ColorTypeId = 10, // Warm Spring
            Colors = allColors.Where(c => c.HexCode == "#97D7CB" ||
										c.HexCode == "#FCF2C7" ||
										c.HexCode == "#DBC797" ||
										c.HexCode == "#9C765D").ToList()
		},
        // 84. Các màu: #DDC35D (Cream Gold), #F0EC73 (Sunny), #C53B3D (Red Safflower), #86213D (Red Claret)
        // Phân tích: Màu sắc sáng và ấm áp (vàng kem, vàng tươi) kết hợp với đỏ đậm. Phù hợp với Warm Spring.
        // Chọn: Warm Spring (10) - do sự nổi bật của màu vàng tươi và ấm áp.
        new(){
			ColorTypeId = 10, // Warm Spring
            Colors = allColors.Where(c => c.HexCode == "#DDC35D" ||
										c.HexCode == "#F0EC73" ||
										c.HexCode == "#C53B3D" ||
										c.HexCode == "#86213D").ToList()
		},
        // 85. Các màu: #FFA286 (Tangerine Cream), #FCDCC8 (Sweet Peach), #FFF4F2 (Rose Sugar), #A5EACF (Sweet Aqua)
        // Phân tích: Màu sắc sáng, nhẹ nhàng, tinh khiết, với tông cam đào, hồng nhạt và xanh mint. Phù hợp với Light Spring.
        // Chọn: Light Spring (11) - do tính chất nhẹ nhàng, sáng và thanh thoát.
        new(){
			ColorTypeId = 11, // Light Spring
            Colors = allColors.Where(c => c.HexCode == "#FFA286" ||
										c.HexCode == "#FCDCC8" ||
										c.HexCode == "#FFF4F2" ||
										c.HexCode == "#A5EACF").ToList()
		},
        // 86. Các màu: #B33534 (Molten Lava), #E3E8CD (Frozen Margarita), #FFFDF8 (Vanilla Shake), #A1DD70 (Tropical Light)
        // Phân tích: Màu sắc rực rỡ (đỏ cam) kết hợp với màu sáng và tươi (xanh lá). Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự rõ ràng và sống động của các màu.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#B33534" ||
										c.HexCode == "#E3E8CD" ||
										c.HexCode == "#FFFDF8" ||
										c.HexCode == "#A1DD70").ToList()
		},
        // 87. Các màu: #464646 (Wood Charcoal), #C4C4BC (Mist Grey), #F4F4F4 (White Smoke), #DEDAD1 (Shy Beige)
        // Phân tích: Màu sắc nhẹ nhàng, mờ, chủ yếu là tông xám và be trung tính. Phù hợp với Soft Summer.
        // Chọn: Soft Summer (5) - tính chất nhẹ nhàng, mờ và sắc xám.
        new(){
			ColorTypeId = 5, // Soft Summer
            Colors = allColors.Where(c => c.HexCode == "#464646" ||
										c.HexCode == "#C4C4BC" ||
										c.HexCode == "#F4F4F4" ||
										c.HexCode == "#DEDAD1").ToList()
		},
        // 88. Các màu: #13A699 (Stormy Strait Green), #FFD708 (Sunflower Yellow), #FFF7EC (White Lace), #AAF0D1 (Magic Mint)
        // Phân tích: Màu sắc sáng, tinh khiết, không bị pha tông xám, rất rõ ràng (xanh, vàng, trắng). Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự rực rỡ và rõ nét.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#13A699" ||
										c.HexCode == "#FFD708" ||
										c.HexCode == "#FFF7EC" ||
										c.HexCode == "#AAF0D1").ToList()
		},
        // 89. Các màu: #C7C6AB (Tamy Thyme), #F9F5F0 (Bright Winter Cloud), #F7E1D8 (Cameo Rose), #EBCCB9 (Scotchtone)
        // Phân tích: Màu sắc nhẹ nhàng, mờ, tông xanh xám, trắng và be hồng. Phù hợp với Soft Summer.
        // Chọn: Soft Summer (5) - tính chất nhẹ nhàng, mờ.
        new(){
			ColorTypeId = 5, // Soft Summer
            Colors = allColors.Where(c => c.HexCode == "#C7C6AB" ||
										c.HexCode == "#F9F5F0" ||
										c.HexCode == "#F7E1D8" ||
										c.HexCode == "#EBCCB9").ToList()
		},
        // 90. Các màu: #D11C5B (Bright Rose), #F6EED5 (Moonlight), #FAC600 (Yellow Jelly), #EA4B1D (Nasturtium Flower)
        // Phân tích: Màu sắc sáng, tinh khiết, không bị pha tông xám, rất rõ ràng (hồng, vàng, cam). Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự rực rỡ và rõ nét.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#D11C5B" ||
										c.HexCode == "#F6EED5" ||
										c.HexCode == "#FAC600" ||
										c.HexCode == "#EA4B1D").ToList()
		},
        // 91. Các màu: #D51D48 (Lusty Lips), #F24463 (Smudged Lips), #FAEDE7 (Rose White), #F7D3C3 (Dreamsicle)
        // Phân tích: Màu sắc sáng, tinh khiết, không bị pha tông xám, rất rõ ràng (đỏ, hồng, trắng, cam đào). Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự rực rỡ và rõ nét.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#D51D48" ||
										c.HexCode == "#F24463" ||
										c.HexCode == "#FAEDE7" ||
										c.HexCode == "#F7D3C3").ToList()
		},
        // 92. Các màu: #242121 (Blackout), #F8C25C (Saffron Mango), #F8F3E3 (Daisy White), #FF2047 (Red Flag)
        // Phân tích: Màu sắc đậm (đen) kết hợp với màu sáng rực (cam, trắng, đỏ tươi). Phù hợp với Deep Winter hoặc Clear Spring.
        // Chọn: Deep Winter (2) - do sự tương phản mạnh mẽ với màu đen.
        new(){
			ColorTypeId = 2, // Deep Winter
            Colors = allColors.Where(c => c.HexCode == "#242121" ||
										c.HexCode == "#F8C25C" ||
										c.HexCode == "#F8F3E3" ||
										c.HexCode == "#FF2047").ToList()
		},
        // 93. Các màu: #D63447 (Safflower Red), #F57B51 (Bonfire), #F6EEDF (Birch White), #D1CEBD (Foggy Gray)
        // Phân tích: Màu sắc ấm áp, tông đỏ, cam và be. Phù hợp với Warm Autumn.
        // Chọn: Warm Autumn (7) - do tính chất ấm áp.
        new(){
			ColorTypeId = 7, // Warm Autumn
            Colors = allColors.Where(c => c.HexCode == "#D63447" ||
										c.HexCode == "#F57B51" ||
										c.HexCode == "#F6EEDF" ||
										c.HexCode == "#D1CEBD").ToList()
		},
        // 94. Các màu: #333333 (Carbon), #9BA8A8 (Oily Steel), #EBEBE7 (Arctic Fox), #C8CEC4 (Foggy Dew)
        // Phân tích: Màu sắc lạnh, nhạt, sáng, chủ yếu là tông xám. Phù hợp với Cool Winter.
        // Chọn: Cool Winter (1) - do sự hiện diện của tông xám lạnh.
        new(){
			ColorTypeId = 1, // Cool Winter
            Colors = allColors.Where(c => c.HexCode == "#333333" ||
										c.HexCode == "#9BA8A8" ||
										c.HexCode == "#EBEBE7" ||
										c.HexCode == "#C8CEC4").ToList()
		},
        // 95. Các màu: #95CE67 (Bright Lettuce), #DAE3BB (Aloe Cream), #FAF8F0 (Winter Frost), #95DDDA (Pale Beryl)
        // Phân tích: Màu sắc sáng, nhẹ nhàng, tinh khiết, với tông xanh lá và xanh mint. Phù hợp với Light Spring.
        // Chọn: Light Spring (11) - do tính chất nhẹ nhàng, sáng và thanh thoát.
        new(){
			ColorTypeId = 11, // Light Spring
            Colors = allColors.Where(c => c.HexCode == "#95CE67" ||
										c.HexCode == "#DAE3BB" ||
										c.HexCode == "#FAF8F0" ||
										c.HexCode == "#95DDDA").ToList()
		},
        // 96. Các màu: #C74375 (Fuchsia Rose), #EE778A (Bubblegum), #FAF3E3 (Whitewashed Fence), #FFF7EC (White Lace)
        // Phân tích: Màu sắc sáng, tinh khiết, không bị pha tông xám, rất rõ ràng (hồng, trắng). Phù hợp với Clear Spring.
        // Chọn: Clear Spring (12) - do sự tươi sáng và rõ nét.
        new(){
			ColorTypeId = 12, // Clear Spring
            Colors = allColors.Where(c => c.HexCode == "#C74375" ||
										c.HexCode == "#EE778A" ||
										c.HexCode == "#FAF3E3" ||
										c.HexCode == "#FFF7EC").ToList()
		},
        // 97. Các màu: #E04255 (Celestial Coral), #FAF8E0 (Clear Moon), #FEE698 (Pale Daffodil), #FDB15D (Soft Orange)
        // Phân tích: Màu sắc sáng, ấm áp, với tông đỏ san hô, vàng nhạt và cam. Phù hợp với Warm Spring.
        // Chọn: Warm Spring (10) - do tính chất sáng và ấm áp.
        new(){
			ColorTypeId = 10, // Warm Spring
            Colors = allColors.Where(c => c.HexCode == "#E04255" ||
										c.HexCode == "#FAF8E0" ||
										c.HexCode == "#FEE698" ||
										c.HexCode == "#FDB15D").ToList()
		},
        // 98. Các màu: #7B3638 (Red Pear), #E04255 (Celestial Coral), #FCEEDD (Mango Cheesecake), #A1DD70 (Tropical Light)
        // Phân tích: Sự pha trộn giữa đỏ đậm, đỏ san hô, be kem và xanh lá. Phù hợp với Deep Autumn.
        // Chọn: Deep Autumn (9) - do sự kết hợp của màu đậm và tông ấm.
        new(){
			ColorTypeId = 9, // Deep Autumn
            Colors = allColors.Where(c => c.HexCode == "#7B3638" ||
										c.HexCode == "#E04255" ||
										c.HexCode == "#FCEEDD" ||
										c.HexCode == "#A1DD70").ToList()
		},
        // 99. Các màu: #F49FB6 (Rose Harmony), #F4D2D2 (Lady Pink), #FAF8F0 (Winter Frost), #A6E0DE (Aqua Island)
        // Phân tích: Màu sắc nhẹ nhàng, mờ, chủ yếu là tông hồng và xanh nhạt. Phù hợp với Soft Summer.
        // Chọn: Soft Summer (5) - tính chất nhẹ nhàng, mờ.
        new(){
			ColorTypeId = 5, // Soft Summer
            Colors = allColors.Where(c => c.HexCode == "#F49FB6" ||
										c.HexCode == "#F4D2D2" ||
										c.HexCode == "#FAF8F0" ||
										c.HexCode == "#A6E0DE").ToList()
		},
        // 100. Các màu: #2C2627 (Velvet Black), #BC2C3D (Cherry Lush), #F8F3E6 (Swan White), #EFD2BC (Peach Puree)
        // Phân tích: Màu sắc đậm (đen) kết hợp với màu đỏ đậm và các màu sáng trung tính. Phù hợp với Deep Winter.
        // Chọn: Deep Winter (2) - do sự tương phản mạnh mẽ với màu đen.
        new(){
			ColorTypeId = 2, // Deep Winter
            Colors = allColors.Where(c => c.HexCode == "#2C2627" ||
										c.HexCode == "#BC2C3D" ||
										c.HexCode == "#F8F3E6" ||
										c.HexCode == "#EFD2BC").ToList()
		}
	};
		}
	}
}