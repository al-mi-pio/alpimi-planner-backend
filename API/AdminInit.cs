﻿using System.Linq.Expressions;
using System.Security.Cryptography;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Queries;
using AlpimiAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI
{
    public static class AdminInit
    {
        private static readonly IDbService _dbService = new DbService();

        public static async Task StartupBase()
        {
            string? admins = "1";

            try
            {
                admins = await _dbService.Get<string>(
                    "SELECT [Role] FROM [Auth] WHERE [Role] = 'Admin';",
                    ""
                );
            }
            catch (Exception)
            {
                throw new Exception("Connection String");
            }

            if (admins == null)
            {
                Console.Write(
                    @"
           _       _           _            _                             
     /\   | |     (_)         (_)          | |                            
    /  \  | |_ __  _ _ __ ___  _      _ __ | | __ _ _ __  _ __   ___ _ __ 
   / /\ \ | | '_ \| | '_ ` _ \| |    | '_ \| |/ _` | '_ \| '_ \ / _ \ '__|
  / ____ \| | |_) | | | | | | | |    | |_) | | (_| | | | | | | |  __/ |   
 /_/    \_\_| .__/|_|_| |_| |_|_|    | .__/|_|\__,_|_| |_|_| |_|\___|_|   
            | |                      | |                                  
            |_|                      |_|                                  

Welcome to Alpimi Planner!
To start using the API service you need to create an Administrator account first.

"
                );
                ActionResult<User?> user;
                string? login;
                try
                {
                    do
                    {
                        System.Console.WriteLine("Login:");
                        login = Console.ReadLine();

                        GetUserByLoginHandler getUserByLoginHandler = new GetUserByLoginHandler(
                            _dbService
                        );
                        GetUserByLoginQuery getUserByLoginQuery = new GetUserByLoginQuery(
                            login!,
                            new Guid(),
                            "Admin"
                        );
                        user = await getUserByLoginHandler.Handle(
                            getUserByLoginQuery,
                            new CancellationToken()
                        );
                    } while (user.Value != null || login == "");

                    string? password1 = "temp";
                    string? password2 = "temp";
                    do
                    {
                        if (password1 != password2)
                        {
                            Console.WriteLine("Pasword don't match, try again\nPassword:");
                        }
                        Console.WriteLine("Password");
                        password1 = Console.ReadLine();
                        Console.WriteLine("Repeat Password:");
                        password2 = Console.ReadLine();
                    } while (password1 != password2 || password1 == "");

                    var userID = await _dbService.Post<Guid>(
                        @"
                    INSERT INTO [User] ([Id],[Login],[CustomURL])
                    OUTPUT INSERTED.Id                    
                    VALUES ('"
                            + Guid.NewGuid()
                            + "','"
                            + login
                            + "',NULL);",
                        ""
                    );
                    byte[] salt = RandomNumberGenerator.GetBytes(16);
                    byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                        password1!,
                        salt,
                        Configuration.GetHashIterations(),
                        Configuration.GetHashAlgorithm(),
                        Configuration.GetKeySize()
                    );

                    await _dbService.Post<Guid>(
                        @"
                    INSERT INTO [Auth] ([Id],[Password],[Salt],[Role],[UserID])
                    OUTPUT INSERTED.UserID                    
                    VALUES ('"
                            + Guid.NewGuid()
                            + "','"
                            + Convert.ToBase64String(hash)
                            + "','"
                            + Convert.ToBase64String(salt)
                            + "','Admin','"
                            + userID
                            + "');",
                        ""
                    );
                    Console.WriteLine("Success!");
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            }
        }
    }
}
