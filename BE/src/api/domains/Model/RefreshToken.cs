using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
    public class RefreshToken : BaseEntity
    {
        public string Token {get; set;}
        public Guid UserId {get; set;}
        public DateTime ExpiresOnUtc {get; set;}
        public User User {get; set;}
    }
}