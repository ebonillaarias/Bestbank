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

        public List<CallsRecords> spGetListCallsRecords(string CallId, string ConnString)
        {
            List<CallsRecords> list = new List<CallsRecords>();
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("spGetListCallsRecords", con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pCallId", SqlDbType.VarChar).Value = CallId;

                    var read = cmd.ExecuteReader();

                    while (read.Read())
                    {
                        CallsRecords tmp = new CallsRecords();
                        if (read["Id"] != DBNull.Value)
                            tmp.Id = read.GetInt32(read.GetOrdinal("Id"));

                        if (read["Record"] != DBNull.Value)
                            tmp.Record = read.GetBoolean(read.GetOrdinal("Record"));

                        if (read["CallPartNumber"] != DBNull.Value)
                            tmp.CallPartNumber = read.GetString(read.GetOrdinal("CallPartNumber"));

                        if (read["StartDate"] != DBNull.Value)
                            tmp.StartDate = read.GetDateTime(read.GetOrdinal("StartDate"));

                        if (read["CallId"] != DBNull.Value)
                            tmp.CallId = Int32.Parse(read.GetString(read.GetOrdinal("CallId")));
                        
                        list.Add(tmp);
                    }
                }
            }

            return list;
        }

    }
}
