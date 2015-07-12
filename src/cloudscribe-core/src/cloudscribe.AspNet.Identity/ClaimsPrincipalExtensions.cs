﻿// Copyright (c) Source Tree Solutions, LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Author:					Joe Audette
// Created:					2015-07-11
// Last Modified:			2015-07-11
// 

using System.Security.Claims;

namespace cloudscribe.AspNet.Identity
{
    public static class ClaimsPrincipalExtensions
    {

        public static bool IsInRoles(this ClaimsPrincipal principal, string allowedRolesCsv)
        {
            if(string.IsNullOrEmpty(allowedRolesCsv)) { return true; } // empty indicates no role filtering
            string[] roles = allowedRolesCsv.Split(',');
            if(roles.Length == 0) { return true; }

            if (!principal.IsSignedIn()) { return false; } 

            foreach(string role in roles)
            {
                if (role.Length == 0) continue;
                if(role == "All Users") { return true; }
                if(principal.IsInRole(role)) { return true; }
            }


            return false;

        }
    }
}
