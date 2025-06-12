using AutoMapper;
using Keycloak.Net.Models.Users;

namespace keycloak_userEditor;

public class UserProfile : Profile
{
    const string Age = "Age";
    const string AvatarUrl = "AvatarUrl";

    public UserProfile()
    {
        CreateMap<UserInfo, User>()
            .ForMember(representation => representation.Id, expression => expression.Ignore())
            .ForMember(representation => representation.Attributes, expression => expression.Ignore())
            .ForMember(representation => representation.UserName, expression => expression.MapFrom(info => info.Login))
            .ForMember(representaion=>representaion.CreatedTimestamp, expression => expression.MapFrom(info => info.Created.ToUnixTimeMilliseconds()))
            .AfterMap((info, representation) =>
            {
                representation.Attributes = new Dictionary<string, IEnumerable<string>>()
                {
                    { AvatarUrl, new List<string>() { info.AvatarUrl } },
                    { Age, new List<string>() { info.Age.ToString() } }
                };
            });//.IgnoreAllSourcePropertiesWithAnInaccessibleSetter()
        CreateMap<User, UserResult>()
            .ForMember(info => info.Login, expression => expression.MapFrom(representation => representation.UserName))
            .ForMember(info => info.Age,
                expression => expression.MapFrom(representation => representation.Attributes[Age].FirstOrDefault()))
            .ForMember(info => info.AvatarUrl,
                expression =>
                    expression.MapFrom(representation => representation.Attributes[AvatarUrl].FirstOrDefault()));
        CreateMap<UserUpdate, User>()
            .ForMember(representation => representation.Attributes, expression => expression.Ignore())
            .ForMember(representation => representation.UserName, expression => expression.MapFrom(info => info.Login))
            .AfterMap((info, representation) =>
            {
                representation.Attributes = new Dictionary<string, IEnumerable<string>>()
                {
                    { AvatarUrl, new List<string>() { info.AvatarUrl } },
                    { Age, new List<string>() { info.Age.ToString() } }
                };
            });
        CreateMap<User, UserUpdate>()
            .ForMember(info => info.Login, expression => expression.MapFrom(representation => representation.UserName))
            .ForMember(info => info.Age,
                expression => expression.MapFrom(representation => representation.Attributes[Age].FirstOrDefault()))
            .ForMember(info => info.AvatarUrl,
                expression =>
                    expression.MapFrom(representation => representation.Attributes[AvatarUrl].FirstOrDefault()));

    }
}