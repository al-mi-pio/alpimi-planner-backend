﻿using System.Data;
using alpimi_planner_backend.API;
using alpimi_planner_backend.API.Utilities;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;

namespace AlpimiAPI.User.Commands
{
    public record DeleteUserCommand(Guid Id) : IRequest;

    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IDbService _dbService;

        public DeleteUserHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await _dbService.Delete("DELETE [User] WHERE [Id] = @Id;", request);
        }
    }
}
