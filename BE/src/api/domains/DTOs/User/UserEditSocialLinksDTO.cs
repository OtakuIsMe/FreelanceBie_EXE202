using BE.src.api.domains.Enum;

namespace BE.src.api.domains.DTOs.User
{
    public class UserEditSocialLinksDTO
    {
        public string? FacebookLink { get; set; }
        public string? InstagramLink { get; set; }
        public string? TwitterLink { get; set; }
        public string? GetLinkByType(TypeSocialEnum type)
        {
            return type switch
            {
                TypeSocialEnum.Facebook => FacebookLink,
                TypeSocialEnum.Instagram => InstagramLink,
                TypeSocialEnum.Twitter => TwitterLink,
                _ => null
            };
        }
    }
}