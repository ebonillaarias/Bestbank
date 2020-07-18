using inConcert.iMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.DataAccess.Repositories.Interfaces
{
    public interface IStoreProcedureRepository
    {
        int SaveCallRecord(CallsRecords callsrecords, string ConnString);
    }
}
