using AutoMapper;
using PerHue.Application.Models;
using PerHue.Domain.Entities;

namespace PerHue.Infrastructure.Utils
{
	public class AutoMapperConfiguration : Profile
	{
		public AutoMapperConfiguration()
		{
			CreateMap<UserAccount, UserModel>().ReverseMap();
			CreateMap<UserAccount, CreateUserModel>().ReverseMap();
			CreateMap<UserAccount, CreateUserByEmailModel>().ReverseMap();
			CreateMap<UserAccount, ChangePasswordModel>()
				.ReverseMap()
				.ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.NewPassword));

			CreateMap<Payment, PaymentModel>().ReverseMap();
			CreateMap<ServicePackage, ServicePackageModel>().ReverseMap();
			CreateMap<UserSubscription, UserSubscriptionModel>().ReverseMap();
			CreateMap<UserSubscription, CreateUserSubscriptionModel>().ReverseMap();
			CreateMap<PaymentLog, PaymentLogModel>().ReverseMap();
			CreateMap<Role, RoleModel>().ReverseMap();
			CreateMap<Color, ColorModel>().ReverseMap();
			CreateMap<ColorType, ColorTypeModel>().ReverseMap();
			CreateMap<CapsulePalette, CapsulePaletteModel>().ReverseMap();
			CreateMap<TestResult, TestResultModel>().ReverseMap();
			CreateMap<Expert, ExpertModel>()
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.IdNavigation.Email))
				.ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.IdNavigation.Username));
			CreateMap<Notification, NotificationModel>()
				.ForMember(dest => dest.ReceiverUsername, opt => opt.MapFrom(src => src.ReceiverNavigation.Username));

			CreateMap<AiPicture, AiPictureModel>().ReverseMap();
			CreateMap<TestRequest, TestRequestModel>()
				.ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.UserAccount.Email));

			CreateMap<TestResponse, TestResponseModel>()
				.ForMember(dest => dest.ColorTypeName, opt => opt.MapFrom(src => src.ColorType.Name));
			CreateMap<CreateTestResponseModel, TestResponse>();
		}
	}
}


//CreateMap<User, User>()
//	.ForMember(dest => dest.Id, opt => opt.Ignore())
//	.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
//	.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
//	.ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
//	.ForMember(dest => dest.IsDeleted, opt => opt.Ignore());