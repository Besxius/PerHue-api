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
			CreateMap<UserAccount, UpdateUserModel>().ReverseMap();
			CreateMap<UserAccount, ChangePasswordModel>()
				.ReverseMap()
				.ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.NewPassword));

			CreateMap<Payment, PaymentModel>().ReverseMap();
			CreateMap<ServicePackage, ServicePackageModel>().ReverseMap();
			CreateMap<UserSubscription, UserSubscriptionModel>().ReverseMap();
			CreateMap<UserSubscription, CreateUserSubscriptionModel>().ReverseMap();
			//CreateMap<User, User>()
			//	.ForMember(dest => dest.Id, opt => opt.Ignore())
			//	.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
			//	.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
			//	.ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
			//	.ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
		}
	}
}
