using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public interface ITransaction
    {
        Task<IDbContextTransaction> BeginTransaction();
    }
}
