using inConcert.iMS.DataAccess.Repositories.Interfaces;
using inConcert.iMS.Domain.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace inConcert.iMS.DataAccess.Repositories
{
    public class StoreProcedureRepository : IStoreProcedureRepository
    {
        public int SaveCallRecord(CallsRecords callsrecords, string ConnString)
        {
            int rpta = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spSaveCallsRecords", con))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@CallId", SqlDbType.VarChar).Value = callsrecords.CallId;
                        cmd.Parameters.Add("@Record", SqlDbType.Bit).Value = callsrecords.Record;
                        cmd.Parameters.Add("@CallPartNumber", SqlDbType.VarChar).Value = callsrecords.CallPartNumber;

                        rpta = cmd.ExecuteNonQuery();
                    }
                }

                return rpta;
            }
            catch (Exception e)
            {
                return 0;
            }

        }

    }
}
