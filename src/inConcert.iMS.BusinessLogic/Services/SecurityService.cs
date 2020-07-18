using System;
using System.Security.Cryptography;
using System.Text;
using inConcert.iMS.BusinessLogic.Services.Interfaces;
using inConcert.iMS.DataAccess.Repositories.Interfaces;
using inConcert.iMS.Domain.Entities;

namespace inConcert.iMS.BusinessLogic.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IGenericRepository<LogsGenerals> _logsRepository;
        public SecurityService(IGenericRepository<LogsGenerals> logsRepository) 
        {
            _logsRepository = logsRepository;
        }

        public string HashData(string str)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(str));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public string InsertLog(LogsGenerals data)
        {            
            _logsRepository.Insert(data);
            _logsRepository.Save();

            return "";
        }

        public string RequestLog(string msg)
        {
            DateTime dtNow = DateTime.UtcNow;

            LogsGenerals LogPetition = new LogsGenerals
            {
                TypeLog = "Log del Sistema",
                Description = msg,
                HourLog = dtNow,
                UserId = 0,
                CallsId = 0

            };

            _logsRepository.Insert(LogPetition);
            _logsRepository.Save();

            return "";
        }

    }
}
