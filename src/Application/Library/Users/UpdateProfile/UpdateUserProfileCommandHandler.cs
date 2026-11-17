using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Users;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Users.UpdateProfile;

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

