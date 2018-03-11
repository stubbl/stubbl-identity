﻿namespace System.Security.Claims
{
    using Collections.Generic;

    public static class ClaimExtensions
    {
        public static void AddIfValueNotNull(this List<Claim> extended, string type, string value)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (value == null)
            {
                return;
            }

            extended.Add(new Claim(type, value));
        }
    }
}
