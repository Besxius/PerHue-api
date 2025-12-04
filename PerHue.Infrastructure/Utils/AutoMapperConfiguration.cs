using AutoMapper;
using PerHue.Application.Models;
using PerHue.Application.Models.Authentication;
using PerHue.Application.Models.CapsulePalette;
using PerHue.Application.Models.Color;
using PerHue.Application.Models.ColorType;
using PerHue.Application.Models.Expert;
using PerHue.Application.Models.ManualTest;
using PerHue.Application.Models.Notification;
using PerHue.Application.Models.Payment;
using PerHue.Application.Models.PaymentLog;
using PerHue.Application.Models.Role;
using PerHue.Application.Models.ServicePackage;
using PerHue.Application.Models.User;
using PerHue.Application.Models.UserSubscription;
using PerHue.Domain.Entities;

namespace PerHue.Infrastructure.Utils
{
	public class AutoMapperConfiguration : Profile
	{
		public AutoMapperConfiguration()
		{
			CreateMap<UserAccount, UserModel>().ReverseMap();
			CreateMap<UserAccount, CreateUserRequestModel>().ReverseMap();
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
				.ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.IdNavigation.Username))
				.ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.IdNavigation.ProfilePicture));
			CreateMap<Notification, NotificationModel>()
				.ForMember(dest => dest.ReceiverUsername, opt => opt.MapFrom(src => src.ReceiverNavigation.Username))
				.ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.ReceivedTime));

			CreateMap<AiPicture, AiPictureModel>().ReverseMap();
			CreateMap<Picture, PictureModel>().ReverseMap();
			CreateMap<TestRequest, TestRequestModel>().ReverseMap();

			CreateMap<TestRequest, ExpertAssignmentModel>()
				.IncludeBase<TestRequest, TestRequestModel>();

			CreateMap<TestResponse, TestResponseModel>()
				.ForMember(dest => dest.ColorTypeName, opt => opt.MapFrom(src => src.ColorType.Name))
				.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));
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