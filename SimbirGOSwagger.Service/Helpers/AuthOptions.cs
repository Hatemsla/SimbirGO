﻿using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SimbirGOSwagger.Service.Helpers;

public static class AuthOptions
{
    public const string Issuer = "Hatemsla"; // издатель токена
    public const string Audience = "Hatemsla"; // потребитель токена
    const string Key = "7iMdnuwf7XMMKGXGSMHKcs+qicGCinCJONLPrhGOX94=";   // ключ для шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(Key));
}