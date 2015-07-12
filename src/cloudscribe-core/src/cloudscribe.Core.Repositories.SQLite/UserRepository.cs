﻿// Copyright (c) Source Tree Solutions, LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Author:					Joe Audette
// Created:					2014-08-18
// Last Modified:			2015-07-04
// 


using cloudscribe.Configuration;
using cloudscribe.Core.Models;
using cloudscribe.Core.Models.DataExtensions;
using cloudscribe.DbHelpers.SQLite;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace cloudscribe.Core.Repositories.SQLite
{
    //disable warning about not really being async
    // we know it is not, and for Sqlite there is probably no benefit to making it really async
#pragma warning disable 1998

    public sealed class UserRepository : IUserRepository
    {
        public UserRepository(
            SQLiteConnectionstringResolver connectionStringResolver,
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            if (connectionStringResolver == null) { throw new ArgumentNullException(nameof(connectionStringResolver)); }
            if (loggerFactory == null) { throw new ArgumentNullException(nameof(loggerFactory)); }

            config = configuration;
            logFactory = loggerFactory;
            log = loggerFactory.CreateLogger(typeof(UserRepository).FullName);
            connectionString = connectionStringResolver.Resolve();

            dbSiteUser = new DBSiteUser(connectionString, logFactory);
            dbUserLogins = new DBUserLogins(connectionString, logFactory);
            dbUserClaims = new DBUserClaims(connectionString, logFactory);
            dbUserLocation = new DBUserLocation(connectionString, logFactory);
            dbRoles = new DBRoles(connectionString, logFactory);
        }

        private ILoggerFactory logFactory;
        private ILogger log;
        private IConfiguration config;
        private string connectionString;
        private DBSiteUser dbSiteUser;
        private DBUserLogins dbUserLogins;
        private DBUserClaims dbUserClaims;
        private DBUserLocation dbUserLocation;
        private DBRoles dbRoles;

        #region User 

        public async Task<bool> Save(ISiteUser user)
        {
            if (user.SiteId == -1) { throw new ArgumentException("user must have a siteid"); }
            if (user.SiteGuid == Guid.Empty) { throw new ArgumentException("user must have a siteguid"); }

            if (user.UserId == -1)
            {
                user.UserGuid = Guid.NewGuid();
                user.CreatedUtc = DateTime.UtcNow;

                user.UserId = dbSiteUser.AddUser(
                    user.SiteGuid,
                    user.SiteId,
                    user.DisplayName,
                    user.UserName,
                    user.Email,
                    user.Password,
                    user.PasswordSalt,
                    user.UserGuid,
                    user.CreatedUtc,
                    user.MustChangePwd,
                    user.FirstName,
                    user.LastName,
                    user.TimeZoneId,
                    user.DateOfBirth,
                    user.EmailConfirmed,
                    user.PasswordFormat,
                    user.PasswordHash,
                    user.SecurityStamp,
                    user.PhoneNumber,
                    user.PhoneNumberConfirmed,
                    user.TwoFactorEnabled,
                    user.LockoutEndDateUtc);



                //user.LoweredEmail,
                //user.PasswordQuestion,
                //user.PasswordAnswer,
                //user.Gender,
                //user.ProfileApproved,
                //user.RegisterConfirmGuid,
                //user.ApprovedForForums,
                //user.Trusted,
                //user.DisplayInMemberList,
                //user.WebSiteURL,
                //user.Country,
                //user.State,
                //user.Occupation,
                //user.Interests,
                //user.MSN,
                //user.Yahoo,
                //user.AIM,
                //user.ICQ,
                //user.TotalPosts,
                //user.AvatarUrl,
                //user.TimeOffsetHours,
                //user.Signature,
                //user.Skin,
                //user.IsDeleted,
                //user.LastActivityDate,
                //user.LastLoginDate,
                //user.LastPasswordChangedDate,
                //user.LastLockoutDate,
                //user.FailedPasswordAttemptCount,
                //user.FailedPwdAttemptWindowStart,
                //user.FailedPwdAnswerAttemptCount,
                //user.FailedPwdAnswerWindowStart,
                //user.IsLockedOut,
                //user.MobilePIN,    
                //user.Comment,
                //user.OpenIDURI,
                //user.WindowsLiveID,    
                //user.TotalRevenue,
                //user.NewEmail,
                //user.EditorPreference,
                //user.EmailChangeGuid,
                //user.PasswordResetGuid,
                //user.RolesChanged,
                //user.AuthorBio 
                //);

                // user.UserID = newId;
            }

            // not all properties are added on insert so update even if we just inserted

            return Update(user);

        }

        private bool Update(ISiteUser user)
        {
            if (string.IsNullOrEmpty(user.LoweredEmail)) { user.LoweredEmail = user.Email.ToLowerInvariant(); }

            return dbSiteUser.UpdateUser(
                    user.UserId,
                    user.DisplayName,
                    user.UserName,
                    user.Email,
                    user.Password,
                    user.PasswordSalt,
                    user.Gender,
                    user.ProfileApproved,
                    user.ApprovedForLogin,
                    user.Trusted,
                    user.DisplayInMemberList,
                    user.WebSiteUrl,
                    user.Country,
                    user.State,
                    string.Empty, // legacy user.Occupation,
                    string.Empty, // legacy user.Interests,
                    string.Empty, // legacy user.MSN,
                    string.Empty, // legacy user.Yahoo,
                    string.Empty, // legacyuser.AIM,
                    string.Empty, // legacy user.ICQ,
                    user.AvatarUrl,
                    user.Signature,
                    user.Skin,
                    user.LoweredEmail,
                    user.PasswordQuestion,
                    user.PasswordAnswer,
                    user.Comment,
                    0, // legacy timeOffsetHours
                    user.OpenIdUri,
                    string.Empty, // legacy user.WindowsLiveId,
                    user.MustChangePwd,
                    user.FirstName,
                    user.LastName,
                    user.TimeZoneId,
                    user.EditorPreference,
                    user.NewEmail,
                    user.EmailChangeGuid,
                    user.PasswordResetGuid,
                    user.RolesChanged,
                    user.AuthorBio,
                    user.DateOfBirth,
                    user.EmailConfirmed,
                    user.PasswordFormat,
                    user.PasswordHash,
                    user.SecurityStamp,
                    user.PhoneNumber,
                    user.PhoneNumberConfirmed,
                    user.TwoFactorEnabled,
                    user.LockoutEndDateUtc);


            //user.RegisterConfirmGuid,
            //user.TotalPosts,
            //user.TimeOffsetHours,
            //user.DateCreated,
            //user.UserGuid,
            //user.IsDeleted,
            //user.LastActivityDate,
            //user.LastLoginDate,
            //user.LastPasswordChangedDate,
            //user.LastLockoutDate,
            //user.FailedPasswordAttemptCount,
            //user.FailedPwdAttemptWindowStart,
            //user.FailedPwdAnswerAttemptCount,
            //user.FailedPwdAnswerWindowStart,
            //user.IsLockedOut,
            //user.MobilePIN,
            //user.SiteGuid,
            //user.TotalRevenue,

        }




        /// <summary>
        /// Deletes an instance of User. Returns true on success.
        /// </summary>
        /// <param name="userID"> userID </param>
        /// <returns>bool</returns>
        public async Task<bool> Delete(int userId)
        {
            return dbSiteUser.DeleteUser(userId);
        }

        public async Task<bool> FlagAsDeleted(int userId)
        {
            return dbSiteUser.FlagAsDeleted(userId);
        }

        public async Task<bool> FlagAsNotDeleted(int userId)
        {
            return dbSiteUser.FlagAsNotDeleted(userId);
        }

        public bool UpdatePasswordAndSalt(
            int userId,
            int passwordFormat,
            string password,
            string passwordSalt)
        {
            return dbSiteUser.UpdatePasswordAndSalt(userId, passwordFormat, password, passwordSalt);
        }

        public async Task<bool> ConfirmRegistration(Guid registrationGuid)
        {
            if (registrationGuid == Guid.Empty)
            {
                return false;
            }

            return dbSiteUser.ConfirmRegistration(Guid.Empty, registrationGuid);
        }


        public async Task<bool> LockoutAccount(Guid userGuid)
        {
            return dbSiteUser.AccountLockout(userGuid, DateTime.UtcNow);
        }

        public async Task<bool> UnLockAccount(Guid userGuid)
        {
            return dbSiteUser.AccountClearLockout(userGuid);
        }

        public async Task<bool> UpdateFailedPasswordAttemptCount(Guid userGuid, int failedPasswordAttemptCount)
        {
            return dbSiteUser.UpdateFailedPasswordAttemptCount(userGuid, failedPasswordAttemptCount);
        }

        public async Task<bool> UpdateTotalRevenue(Guid userGuid)
        {
            dbSiteUser.UpdateTotalRevenue(userGuid);
            return true;

        }

        /// <summary>
        /// updates the total revenue for all users
        /// </summary>
        public async Task<bool> UpdateTotalRevenue()
        {
            dbSiteUser.UpdateTotalRevenue();
            return true;
        }


        //public DataTable GetUserListForPasswordFormatChange(int siteId)
        //{
        //    if (AppSettings.UseRelatedSiteMode) { siteId = AppSettings.RelatedSiteId; }

        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("UserID", typeof(int));
        //    dt.Columns.Add("PasswordSalt", typeof(String));
        //    dt.Columns.Add("Pwd", typeof(String));

        //    using (IDataReader reader = DBSiteUser.GetUserList(siteId))
        //    {
        //        while (reader.Read())
        //        {
        //            DataRow row = dt.NewRow();
        //            row["UserID"] = reader["UserID"];
        //            row["PasswordSalt"] = reader["PasswordSalt"];
        //            row["Pwd"] = reader["Pwd"];
        //            dt.Rows.Add(row);

        //        }
        //    }

        //    return dt;
        //}

        public int GetCount(int siteId)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            return dbSiteUser.UserCount(siteId);
        }

        //public int UserCount(int siteId, String userNameBeginsWith)
        //{
        //    if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

        //    return DBSiteUser.UserCount(siteId, userNameBeginsWith);
        //}

        public int UsersOnlineSinceCount(int siteId, DateTime sinceTime)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            return dbSiteUser.CountOnlineSince(siteId, sinceTime);
        }


        public async Task<ISiteUser> FetchNewest(int siteId)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            int newestUserId = await GetNewestUserId(siteId);
            return await Fetch(siteId, newestUserId);
        }

        public async Task<ISiteUser> Fetch(int siteId, int userId)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            using (DbDataReader reader = dbSiteUser.GetSingleUser(userId))
            {
                if (reader.Read())
                {
                    SiteUser user = new SiteUser();

                    user.LoadFromReader(reader);

                    if (user.SiteId == siteId) { return user; }

                }
            }

            return null;
        }


        public async Task<ISiteUser> Fetch(int siteId, Guid userGuid)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            using (DbDataReader reader = dbSiteUser.GetSingleUser(userGuid))
            {
                if (reader.Read())
                {
                    SiteUser user = new SiteUser();

                    user.LoadFromReader(reader);

                    if (user.SiteId == siteId) { return user; }

                }
            }

            return null;
        }

        public async Task<ISiteUser> FetchByConfirmationGuid(int siteId, Guid confirmGuid)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            using (DbDataReader reader = dbSiteUser.GetUserByRegistrationGuid(siteId, confirmGuid))
            {
                if (reader.Read())
                {
                    SiteUser user = new SiteUser();

                    user.LoadFromReader(reader);

                    if (user.SiteId == siteId) { return user; }

                }
            }

            return null;
        }


        public async Task<ISiteUser> Fetch(int siteId, string email)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            using (DbDataReader reader = dbSiteUser.GetSingleUser(siteId, email))
            {
                if (reader.Read())
                {
                    SiteUser user = new SiteUser();

                    user.LoadFromReader(reader);

                    return user;

                }
            }

            return null;
        }

        public async Task<ISiteUser> FetchByLoginName(int siteId, string userName, bool allowEmailFallback)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            using (DbDataReader reader = dbSiteUser.GetSingleUserByLoginName(siteId, userName, allowEmailFallback))
            {
                if (reader.Read())
                {
                    SiteUser user = new SiteUser();

                    user.LoadFromReader(reader);

                    return user;

                }
            }

            return null;
        }





        public async Task<List<IUserInfo>> GetByIPAddress(Guid siteGuid, string ipv4Address)
        {
            if (config.UseRelatedSiteMode()) { siteGuid = Guid.Empty; }

            List<IUserInfo> userList = new List<IUserInfo>();

            using (DbDataReader reader = dbUserLocation.GetUsersByIPAddress(siteGuid, ipv4Address))
            {
                while (reader.Read())
                {
                    UserInfo user = new UserInfo();
                    user.LoadFromReader(reader);
                    userList.Add(user);
                }
            }

            return userList;
        }

        public async Task<List<IUserInfo>> GetCrossSiteUserListByEmail(string email)
        {
            List<IUserInfo> userList = new List<IUserInfo>();

            using (DbDataReader reader = dbSiteUser.GetCrossSiteUserListByEmail(email))
            {
                while (reader.Read())
                {
                    UserInfo user = new UserInfo();
                    user.LoadFromReader(reader);
                    userList.Add(user);
                }
            }

            return userList;
        }

        public async Task<int> CountUsers(int siteId, String userNameBeginsWith)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            return dbSiteUser.CountUsers(siteId, userNameBeginsWith);
        }

        public async Task<List<IUserInfo>> GetPage(
            int siteId,
            int pageNumber,
            int pageSize,
            string userNameBeginsWith,
            int sortMode)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            //sortMode: 0 = DisplayName asc, 1 = JoinDate desc, 2 = Last, First

            List<IUserInfo> userList = new List<IUserInfo>();

            using (DbDataReader reader = dbSiteUser.GetUserListPage(
                    siteId,
                    pageNumber,
                    pageSize,
                    userNameBeginsWith,
                    sortMode))
            {
                while (reader.Read())
                {
                    UserInfo user = new UserInfo();
                    user.LoadFromReader(reader);
                    userList.Add(user);
                }
            }

            return userList;

        }

        public async Task<int> CountUsersForSearch(int siteId, string searchInput)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            return dbSiteUser.CountUsersForSearch(siteId, searchInput);
        }

        public async Task<List<IUserInfo>> GetUserSearchPage(
            int siteId,
            int pageNumber,
            int pageSize,
            string searchInput,
            int sortMode)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            //sortMode: 0 = DisplayName asc, 1 = JoinDate desc, 2 = Last, First

            List<IUserInfo> userList = new List<IUserInfo>();

            using (DbDataReader reader = dbSiteUser.GetUserSearchPage(
                siteId,
                pageNumber,
                pageSize,
                searchInput,
                sortMode))
            {
                while (reader.Read())
                {
                    UserInfo user = new UserInfo();
                    user.LoadFromReader(reader);
                    userList.Add(user);
                }
            }

            return userList;

        }

        public async Task<int> CountUsersForAdminSearch(int siteId, string searchInput)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            return dbSiteUser.CountUsersForAdminSearch(siteId, searchInput);
        }

        public async Task<List<IUserInfo>> GetUserAdminSearchPage(
            int siteId,
            int pageNumber,
            int pageSize,
            string searchInput,
            int sortMode)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            List<IUserInfo> userList = new List<IUserInfo>();

            using (DbDataReader reader = dbSiteUser.GetUserAdminSearchPage(
                siteId,
                pageNumber,
                pageSize,
                searchInput,
                sortMode))
            {

                while (reader.Read())
                {
                    UserInfo user = new UserInfo();
                    user.LoadFromReader(reader);
                    userList.Add(user);

                }
            }

            return userList;


        }

        public async Task<int> CountLockedOutUsers(int siteId)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }
            return dbSiteUser.CountLockedOutUsers(siteId);
        }

        public async Task<List<IUserInfo>> GetPageLockedUsers(
            int siteId,
            int pageNumber,
            int pageSize)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            List<IUserInfo> userList = new List<IUserInfo>();

            using (DbDataReader reader = dbSiteUser.GetPageLockedUsers(
                siteId,
                pageNumber,
                pageSize))
            {

                while (reader.Read())
                {
                    UserInfo user = new UserInfo();
                    user.LoadFromReader(reader);
                    userList.Add(user);

                }
            }

            return userList;
        }

        public async Task<int> CountNotApprovedUsers(int siteId)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }
            return dbSiteUser.CountNotApprovedUsers(siteId);
        }

        public async Task<List<IUserInfo>> GetNotApprovedUsers(
            int siteId,
            int pageNumber,
            int pageSize)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            List<IUserInfo> userList = new List<IUserInfo>();

            using (DbDataReader reader = dbSiteUser.GetPageNotApprovedUsers(
                siteId,
                pageNumber,
                pageSize))
            {

                while (reader.Read())
                {
                    UserInfo user = new UserInfo();
                    user.LoadFromReader(reader);
                    userList.Add(user);

                }
            }

            return userList;
        }

        //public IDataReader GetSmartDropDownData(int siteId, string query, int rowsToGet)
        //{
        //    //if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

        //    return DBSiteUser.GetSmartDropDownData(siteId, query, rowsToGet);
        //}

        //public IDataReader EmailLookup(int siteId, string query, int rowsToGet)
        //{
        //    //if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

        //    return DBSiteUser.EmailLookup(siteId, query, rowsToGet);
        //}

        public async Task<bool> EmailExistsInDB(int siteId, string email)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            bool found = false;

            using (DbDataReader r = dbSiteUser.GetSingleUser(siteId, email))
            {
                while (r.Read()) { found = true; }
            }
            return found;
        }

        public async Task<bool> EmailExistsInDB(int siteId, int userId, string email)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            bool found = false;

            using (DbDataReader r = dbSiteUser.GetSingleUser(siteId, email))
            {
                while (r.Read())
                {
                    int foundId = Convert.ToInt32(r["UserID"]);
                    found = (foundId != userId);
                    if (found) { return found; }
                }
            }
            return found;
        }

        public bool LoginExistsInDB(int siteId, string loginName)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            bool found = false;

            using (DbDataReader r = dbSiteUser.GetSingleUserByLoginName(siteId, loginName, false))
            {
                while (r.Read()) { found = true; }
            }

            return found;
        }

        /// <summary>
        /// available only if the found user matches the passed in one
        /// or if not found
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="userId"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public bool LoginIsAvailable(int siteId, int userId, string loginName)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }
            bool available = true;

            using (DbDataReader r = dbSiteUser.GetSingleUserByLoginName(siteId, loginName, false))
            {
                while (r.Read())
                {
                    int foundId = Convert.ToInt32(r["UserID"]);

                    available = (foundId == userId);
                    if (!available) { return available; }
                }
            }

            return available;
        }

        public async Task<string> GetUserNameFromEmail(int siteId, String email)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            string result = String.Empty;
            if ((email != null) && (email.Length > 0) && (siteId > 0))
            {
                string comma = String.Empty;
                using (DbDataReader reader = dbSiteUser.GetSingleUser(siteId, email))
                {
                    while (reader.Read())
                    {
                        result += comma + reader["LoginName"].ToString();
                        comma = ", ";

                    }
                }
            }

            return result;

        }



        public async Task<int> GetNewestUserId(int siteId)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            return dbSiteUser.GetNewestUserId(siteId);

        }

        #endregion

        #region Roles

        /// <summary>
        /// Persists a new instance of Role. Returns true on success.
        /// when a role is created displayname corresponds to rolename
        /// but rolename can never change since it is used in a cookies and coded
        /// into security checks in some cases
        /// so subsequent changes to rolename really only effect displayname
        /// ie for localization or customization
        /// to really change a rolename you can delete the role and create a new one with the desired name
        /// some specific required system roles (Admin, Content Administrators) 
        /// are also not allowed to be deleted
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveRole(ISiteRole role)
        {
            if (role.RoleId == -1) // new role
            {
                bool exists = await RoleExists(role.SiteId, role.DisplayName);
                if (exists)
                {
                    log.LogError("attempt to create a duplicate role "
                        + role.DisplayName + " for site "
                        + role.SiteId.ToString());

                    return false;
                }

                role.RoleGuid = Guid.NewGuid();

                role.RoleId = dbRoles.RoleCreate(
                    role.RoleGuid,
                    role.SiteGuid,
                    role.SiteId,
                    role.DisplayName
                    );

                role.RoleName = role.DisplayName;

                return (role.RoleId > -1);
            }
            else
            {
                return dbRoles.Update(
                    role.RoleId,
                    role.DisplayName);

            }

        }

        public async Task<bool> DeleteRole(int roleId)
        {
            return dbRoles.Delete(roleId);
        }

        public async Task<bool> AddUserToRole(
            int roleId,
            Guid roleGuid,
            int userId,
            Guid userGuid
            )
        {
            return dbRoles.AddUser(roleId, userId, roleGuid, userGuid);
        }

        public async Task<bool> RemoveUserFromRole(int roleId, int userId)
        {
            return dbRoles.RemoveUser(roleId, userId);
        }

        public async Task<bool> AddUserToDefaultRoles(ISiteUser siteUser)
        {
            // moved this to the config setting below instead of hard coded
            //IRole role = Fetch(siteUser.SiteId, "Authenticated Users");
            //if (role.RoleID > -1)
            //{
            //    AddUser(role.RoleID, role.RoleGuid, siteUser.UserId, siteUser.UserGuid);
            //}

            ISiteRole role;
            bool result = true;
            string defaultRoles = AppSettings.DefaultRolesForNewUsers;

            if (defaultRoles.Length > 0)
            {
                if (defaultRoles.IndexOf(";") == -1)
                {
                    role = await FetchRole(siteUser.SiteId, defaultRoles);
                    if ((role != null) && (role.RoleId > -1))
                    {
                        result = await AddUserToRole(role.RoleId, role.RoleGuid, siteUser.UserId, siteUser.UserGuid);
                    }
                }
                else
                {
                    string[] roleArray = defaultRoles.Split(';');
                    foreach (string roleName in roleArray)
                    {
                        if (!string.IsNullOrEmpty(roleName))
                        {
                            role = await FetchRole(siteUser.SiteId, roleName);
                            if ((role != null) && (role.RoleId > -1))
                            {
                                result = result && await AddUserToRole(role.RoleId, role.RoleGuid, siteUser.UserId, siteUser.UserGuid);
                            }
                        }
                    }

                }

            }

            return result;
        }

        public async Task<bool> DeleteUserRoles(int userId)
        {
            return dbRoles.DeleteUserRoles(userId);
        }

        public async Task<bool> DeleteUserRolesByRole(int roleId)
        {
            return dbRoles.DeleteUserRolesByRole(roleId);
        }

        public async Task<bool> RoleExists(int siteId, String roleName)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }
            return dbRoles.Exists(siteId, roleName);
        }

        public int GetRoleMemberCount(int roleId)
        {
            // TODO: implement actual select count from db
            // this is works but is not ideal
            int count = 0;
            using (DbDataReader reader = dbRoles.GetRoleMembers(roleId))
            {
                while (reader.Read())
                {
                    count += 1;
                }
            }

            return count;

        }

        public async Task<ISiteRole> FetchRole(int roleId)
        {
            using (DbDataReader reader = dbRoles.GetById(roleId))
            {
                if (reader.Read())
                {
                    SiteRole role = new SiteRole();
                    role.LoadFromReader(reader);
                    return role;
                }
            }

            return null;
        }

        public async Task<ISiteRole> FetchRole(int siteId, string roleName)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }
            SiteRole role = null;

            using (DbDataReader reader = dbRoles.GetByName(siteId, roleName))
            {
                if (reader.Read())
                {
                    role = new SiteRole();
                    role.LoadFromReader(reader);
                }
            }

            return role;

        }

        public async Task<List<string>> GetUserRoles(int siteId, int userId)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            List<string> userRoles = new List<string>();
            using (DbDataReader reader = dbSiteUser.GetRolesByUser(siteId, userId))
            {
                while (reader.Read())
                {
                    userRoles.Add(reader["RoleName"].ToString());
                }

            }

            return userRoles;
        }

        public async Task<IList<ISiteRole>> GetRolesBySite(
            int siteId,
            string searchInput,
            int pageNumber,
            int pageSize)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            IList<ISiteRole> roles = new List<ISiteRole>();
            using (DbDataReader reader = dbRoles.GetPage(siteId, searchInput, pageNumber, pageSize))
            {
                while (reader.Read())
                {
                    SiteRole role = new SiteRole();
                    role.LoadFromReader(reader);
                    role.MemberCount = Convert.ToInt32(reader["MemberCount"]);

                    roles.Add(role);
                }
            }

            return roles;

        }

        public IList<ISiteRole> GetRolesUserIsNotIn(
            int siteId,
            int userId)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            IList<ISiteRole> roles = new List<ISiteRole>();
            using (DbDataReader reader = dbRoles.GetRolesUserIsNotIn(siteId, userId))
            {
                SiteRole role = new SiteRole();
                role.LoadFromReader(reader);

                roles.Add(role);
            }
            return roles;
        }

        public async Task<List<int>> GetRoleIds(int siteId, string roleNamesSeparatedBySemiColons)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            List<int> roleIds = new List<int>();

            List<string> roleNames = GetRolesNames(roleNamesSeparatedBySemiColons);

            foreach (string roleName in roleNames)
            {
                if (string.IsNullOrEmpty(roleName)) { continue; }
                ISiteRole r = await FetchRole(siteId, roleName);
                if (r == null)
                {
                    log.LogDebug("could not get roleid for role named " + roleName);
                    continue;
                }
                if (r.RoleId > -1) { roleIds.Add(r.RoleId); }
            }

            return roleIds;
        }

        public static List<string> GetRolesNames(string roleNamesSeparatedBySemiColons)
        {
            List<string> roleNames = new List<string>();
            string[] roles = roleNamesSeparatedBySemiColons.Split(';');
            foreach (string r in roles)
            {
                if (!roleNames.Contains(r)) { roleNames.Add(r); }
            }

            return roleNames;

        }


        public async Task<int> CountOfRoles(int siteId, string searchInput)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }
            return dbRoles.GetCountOfSiteRoles(siteId, searchInput);
        }

        public async Task<int> CountUsersInRole(int siteId, int roleId, string searchInput)
        {
            return dbRoles.GetCountOfUsersInRole(siteId, roleId, searchInput);
        }

        public async Task<IList<IUserInfo>> GetUsersInRole(
            int siteId,
            int roleId,
            string searchInput,
            int pageNumber,
            int pageSize)
        {
            IList<IUserInfo> users = new List<IUserInfo>();

            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            using (DbDataReader reader = dbRoles.GetUsersInRole(siteId, roleId, searchInput, pageNumber, pageSize))
            {
                while (reader.Read())
                {
                    UserInfo user = new UserInfo();
                    user.LoadFromReader(reader);
                    users.Add(user);

                }

            }

            return users;
        }

        public async Task<IList<ISiteUser>> GetUsersInRole(
            int siteId,
            string roleName)
        {
            IList<ISiteUser> users = new List<ISiteUser>();

            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            ISiteRole role = await FetchRole(siteId, roleName);
            int roleId = -3;
            if (role != null)
            {
                roleId = role.RoleId;
            }

            using (DbDataReader reader = dbRoles.GetUsersInRole(siteId, roleId, string.Empty, 1, 100000))
            {
                while (reader.Read())
                {
                    SiteUser user = new SiteUser();
                    user.LoadFromReader(reader);
                    users.Add(user);

                }

            }

            return users;
        }

        public async Task<int> CountUsersNotInRole(int siteId, int roleId, string searchInput)
        {
            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            return dbRoles.GetCountOfUsersNotInRole(siteId, roleId, searchInput);
        }

        public async Task<IList<IUserInfo>> GetUsersNotInRole(
            int siteId,
            int roleId,
            string searchInput,
            int pageNumber,
            int pageSize)
        {
            IList<IUserInfo> users = new List<IUserInfo>();

            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            using (DbDataReader reader = dbRoles.GetUsersNotInRole(siteId, roleId, searchInput, pageNumber, pageSize))
            {
                while (reader.Read())
                {
                    UserInfo user = new UserInfo();
                    user.LoadFromReader(reader);
                    users.Add(user);

                }

            }

            return users;
        }


        #endregion

        #region Claims

        /// <summary>
        /// Persists a new instance of UserClaim. Returns true on success.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveClaim(IUserClaim userClaim)
        {
            int newId = dbUserClaims.Create(
                userClaim.UserId,
                userClaim.ClaimType,
                userClaim.ClaimValue);

            userClaim.Id = newId;

            return (newId > -1);
        }

        public async Task<bool> DeleteClaim(int id)
        {
            return dbUserClaims.Delete(id);
        }

        public async Task<bool> DeleteClaimsByUser(string userId)
        {
            return dbUserClaims.DeleteByUser(userId);
        }

        public async Task<bool> DeleteClaimByUser(string userId, string claimType)
        {
            return dbUserClaims.DeleteByUser(userId, claimType);
        }

        public async Task<bool> DeleteClaimsBySite(Guid siteGuid)
        {
            return dbUserClaims.DeleteBySite(siteGuid);
        }

        public async Task<IList<IUserClaim>> GetClaimsByUser(string userId)
        {
            DbDataReader reader = dbUserClaims.GetByUser(userId);
            return LoadClaimListFromReader(reader);

        }

        public async Task<IList<ISiteUser>> GetUsersForClaim(
            int siteId,
            string claimType,
            string claimValue)
        {
            IList<ISiteUser> users = new List<ISiteUser>();

            if (config.UseRelatedSiteMode()) { siteId = config.RelatedSiteId(); }

            using (DbDataReader reader = dbUserClaims.GetUsersByClaim(siteId, claimType, claimValue))
            {
                while (reader.Read())
                {
                    SiteUser user = new SiteUser();
                    user.LoadFromReader(reader);
                    users.Add(user);

                }

            }

            return users;
        }


        private List<IUserClaim> LoadClaimListFromReader(DbDataReader reader)
        {
            List<IUserClaim> userClaimList = new List<IUserClaim>();

            using(reader)
            {
                while (reader.Read())
                {
                    UserClaim userClaim = new UserClaim();
                    userClaim.LoadFromReader(reader);
                    userClaimList.Add(userClaim);

                }
            }
            
            return userClaimList;

        }


        #endregion

        #region Logins

        /// <summary>
        /// Persists a new instance of UserLogin. Returns true on success.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateLogin(IUserLogin userLogin)
        {
            if (userLogin.LoginProvider.Length == -1) { return false; }
            if (userLogin.ProviderKey.Length == -1) { return false; }
            if (userLogin.UserId.Length == -1) { return false; }

            return dbUserLogins.Create(
                userLogin.LoginProvider,
                userLogin.ProviderKey,
                userLogin.UserId);


        }


        /// <param name="loginProvider"> loginProvider </param>
        /// <param name="providerKey"> providerKey </param>
        public async Task<IUserLogin> FindLogin(
            string loginProvider,
            string providerKey)
        {
            using (DbDataReader reader = dbUserLogins.Find(
                loginProvider,
                providerKey))
            {
                if (reader.Read())
                {
                    UserLogin userLogin = new UserLogin();
                    userLogin.LoadFromReader(reader);
                    return userLogin;

                }
            }

            return null;
        }


        /// <summary>
        /// Deletes an instance of UserLogin. Returns true on success.
        /// </summary>
        /// <param name="loginProvider"> loginProvider </param>
        /// <param name="providerKey"> providerKey </param>
        /// <param name="userId"> userId </param>
        /// <returns>bool</returns>
        public async Task<bool> DeleteLogin(
            string loginProvider,
            string providerKey,
            string userId)
        {
            return dbUserLogins.Delete(
                loginProvider,
                providerKey,
                userId);
        }

        public async Task<bool> DeleteLoginsByUser(string userId)
        {
            return dbUserLogins.DeleteByUser(userId);
        }

        public async Task<bool> DeleteLoginsBySite(Guid siteGuid)
        {
            return dbUserLogins.DeleteBySite(siteGuid);
        }




        /// <summary>
        /// Gets an IList with all instances of UserLogin.
        /// </summary>
        public async Task<IList<IUserLogin>> GetLoginsByUser(string userId)
        {
            List<IUserLogin> userLoginList = new List<IUserLogin>();
            using (DbDataReader reader = dbUserLogins.GetByUser(userId))
            {
                while (reader.Read())
                {
                    UserLogin userLogin = new UserLogin();
                    userLogin.LoadFromReader(reader);
                    userLoginList.Add(userLogin);

                }
            }

            return userLoginList;

        }




        #endregion

        #region IDisposable

        public void Dispose()
        {

        }

        #endregion
    }

#pragma warning restore 1998

}
