using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Users.UpdateProfile;

internal sealed class UpdateUserProfileCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateUserProfileCommand>
{
    public async Task<Result> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
    {
        var users = unitOfWork.GetDbSet<User>();

        var user = await users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound(command.UserId));
        }

        if (!string.IsNullOrWhiteSpace(command.Email) &&
            await users.AnyAsync(u => u.Email == command.Email && u.Id != command.UserId, cancellationToken))
        {
            return Result.Failure(UserErrors.EmailNotUnique);
        }

        user.Name = command.Name;
        user.LastName = command.LastName;
        user.Email = command.Email;
        user.PhoneNumber = command.PhoneNumber;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

