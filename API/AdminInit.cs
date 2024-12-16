using System.Security.Cryptography;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI
{
    public class AdminInit
    {
        private static readonly DbService _dbService = new DbService(
            new SqlConnection(Configuration.GetConnectionString())
        );
        private readonly IStringLocalizer<General> _str;

        public AdminInit(IStringLocalizer<General> str)
        {
            _str = str;
        }

        public async Task StartupBase()
        {
            string? admins = "1";

            try
            {
                admins = await _dbService.Get<string>(
                    @"
                        SELECT 
                        [Role]
                        FROM [Auth]
                        WHERE [Role] = 'Admin'; ",
                    ""
                );
            }
            catch (Exception)
            {
                throw new ApiErrorException([new ErrorObject(_str["connectionError"])]);
            }

            if (admins == null)
            {
                Console.Write(_str["alpimiLogo"]);
                Console.WriteLine(_str["welcomeMsg"]);
                ActionResult<User?> user;
                string? login;
                try
                {
                    do
                    {
                        System.Console.WriteLine(_str["login"]);
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

                    string? password = null;

                    do
                    {
                        if (password != null)
                        {
                            Console.WriteLine(_str["passwordsDontMatch"]);
                        }
                        Console.WriteLine(_str["password"]);
                        password = Console.ReadLine();
                        Console.WriteLine(_str["repeatPassword"]);
                    } while (password != Console.ReadLine() || password == "");

                    var userId = await _dbService.Post<Guid>(
                        $@"
                            INSERT INTO [User] 
                            ([Id], [Login], [CustomURL])
                            OUTPUT 
                            INSERTED.Id                    
                            VALUES (
                            '{Guid.NewGuid()}',
                            '{login}',
                            NULL); ",
                        ""
                    );
                    byte[] salt = RandomNumberGenerator.GetBytes(16);
                    byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                        password!,
                        salt,
                        Configuration.GetHashIterations(),
                        Configuration.GetHashAlgorithm(),
                        Configuration.GetKeySize()
                    );

                    await _dbService.Post<Guid>(
                        $@"
                            INSERT INTO [Auth] 
                            ([Id], [Password], [Salt], [Role], [UserId])
                            OUTPUT 
                            INSERTED.UserId                    
                            VALUES (
                            '{Guid.NewGuid()}',
                            '{Convert.ToBase64String(hash)}',
                            '{Convert.ToBase64String(salt)}',
                            'Admin',
                            '{userId}'); ",
                        ""
                    );
                    Console.WriteLine(_str["success"]);
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            }
        }
    }
}
