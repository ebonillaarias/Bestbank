using inConcert.iMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.BusinessLogic.Services.Interfaces
{
    public interface ISecurityService
    {
        string HashData(string str);

        string InsertLog(LogsGenerals logsGeneral);

        string RequestLog(string msg);
    }
}
