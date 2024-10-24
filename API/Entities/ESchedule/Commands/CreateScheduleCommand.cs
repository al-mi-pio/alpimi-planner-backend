﻿using System.Security.Cryptography;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.EUser.Queries;
using AlpimiAPI.Settings;
using AlpimiAPI.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ESchedule.Commands
{
    public record CreateScheduleCommand(Guid Id, Guid UserID, string Name, int SchoolHour)
        : IRequest<Guid>;

    public class CreateScheduleHandler : IRequestHandler<CreateScheduleCommand, Guid>
    {
        private readonly IDbService _dbService;

        public CreateScheduleHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<Guid> Handle(
            CreateScheduleCommand request,
            CancellationToken cancellationToken
        )
        {
            GetScheduleByNameHandler getScheduleByNameHandler = new GetScheduleByNameHandler(
                _dbService
            );
            GetScheduleByNameQuery getScheduleByNameQuery = new GetScheduleByNameQuery(
                request.Name,
                new Guid(),
                "Admin"
            );
            ActionResult<Schedule?> scheduleName = await getScheduleByNameHandler.Handle(
                getScheduleByNameQuery,
                cancellationToken
            );

            if (scheduleName.Value != null)
            {
                throw new BadHttpRequestException("Name already taken");
            }

            var insertedId = await _dbService.Post<Guid>(
                @"
                    INSERT INTO [Schedule] ([Id],[Name],[SchoolHour],[UserID])
                    OUTPUT INSERTED.Id                    
                    VALUES (@Id,@Name,@SchoolHour,@UserID);",
                request
            );

            return insertedId;
        }
    }
}
