using AutoMapper;
using DeviceManagement.Application.DTOs;
using DeviceManagement.Domain.Entities;

namespace DeviceManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Device, DeviceDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.IsAvailable, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentUserName, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentLocation, opt => opt.Ignore());

        CreateMap<DeviceRequestDto, Device>();
        
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.CurrentDeviceName, opt => opt.Ignore());

        CreateMap<UserRequestDto, User>();
        
        CreateMap<DeviceAssignment, DeviceAssignmentDto>()
            .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.Device != null ? src.Device.Name : string.Empty))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : string.Empty))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.ReturnedDate == null));

        CreateMap<DeviceAssignmentRequestDto, DeviceAssignment>();
    }
}
