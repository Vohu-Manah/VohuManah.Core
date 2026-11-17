using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VohuManah.Application.Abstractions.Data;

namespace VohuManah.Infrastructure.Database;

internal sealed class SqlServerExceptionClassifier : IDatabaseExceptionClassifier
{
    public DatabaseFailureKind Classify(Exception exception)
    {
        if (exception is DbUpdateException dbUpdateException)
        {
            var sqlException = dbUpdateException.InnerException as SqlException;

            if (sqlException != null)
            {
                if (sqlException.Number == 547)
                {
                    return DatabaseFailureKind.FkReferenced;
                }

                if (sqlException.Number is 2601 or 2627)
                {
                    return DatabaseFailureKind.DuplicateKey;
                }
            }
        }

        if (exception.InnerException is SqlException sqlEx && sqlEx.Number == 547)
        {
            return DatabaseFailureKind.FkReferenced;
        }

        return DatabaseFailureKind.None;
    }
}
