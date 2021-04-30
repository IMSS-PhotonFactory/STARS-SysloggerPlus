using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysloggerPlus
{
    public class InfluxDB_Transfer
    {
        private protected static System.Net.Http.HttpClient wc;

        private protected Uri serverUri;
        private protected string tableName;

        public InfluxDB_Transfer(string ServerIP, int ServerPort, string DbName, string TableName, int MaxConnection = 10)
        {
            wc = new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler() { MaxConnectionsPerServer = MaxConnection });
            serverUri = new Uri($"http://{ServerIP}:{ServerPort}/write?db={DbName}");
            tableName = TableName;
        }


        public virtual bool Write_StarsLog(string raw_str, DateTime timestamp)
        {
            try
            {
                string[] str;
                str = raw_str.Split(' ');
                if (str.Length <= 1)
                {
                }
                else if (str[1] == "@GetValue" || str[1] == "_ChangedIsBusy" || str[1] == "@IsBusy" || str[1] == "_changed" || str[1] == "_Changed" || str[1] == "_ChangedValue")
                {
                    if (!str[2].StartsWith("Er:"))
                    {
                        var content = new System.Net.Http.StringContent(Convert_StarsToInflux(str, timestamp, tableName));
                        wc.PostAsync(serverUri, content);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private protected string Convert_StarsToInflux(string[] param, DateTime timestamp, string table_name)
        {
            var tag_from = param[0].Substring(0, param[0].IndexOf('>'));
            var tag_snd = tag_from;
            if (tag_from.IndexOf('.') >= 0)
            {
                tag_snd = tag_from.Substring(tag_from.IndexOf('.') + 1);
            }
            var tag_to = param[0].Substring(param[0].IndexOf('>') + 1);
            var tag_rcv = tag_to;
            if (tag_to.IndexOf('.') >= 0)
            {
                tag_rcv = tag_to.Substring(tag_to.IndexOf('.') + 1);
            }

            var tag_data = $"command={param[1]},from={tag_from},sender={tag_snd},to={tag_to},reciever={tag_rcv}";

            var field_data = "";

            string[] field_value = param[2].Split(',');
            if (field_value.Length == 1)
            {
                field_data = "value=" + field_value[0];
            }
            else
            {
                for (int i = 0; i < field_value.Length; i++)
                {
                    var temp_str = "ch" + i.ToString("00") + "=" + field_value[i];
                    if (i != field_value.Length - 1)
                    {
                        temp_str += ",";
                    }
                    field_data += temp_str;
                }
            }
            var time = new DateTimeOffset(timestamp.ToUniversalTime());

            var res_str = $"{table_name},{tag_data} {field_data} {time.ToUnixTimeMilliseconds()}000000";

            return res_str;
        }
    }

    public class InfluxDB_Transfer_Vac : InfluxDB_Transfer
    {
        public InfluxDB_Transfer_Vac(string ServerIP, int ServerPort, string DbName, string TableName, int MaxConnection = 10) : base(ServerIP, ServerPort, DbName, TableName, MaxConnection)
        {
        }

        public override bool Write_StarsLog(string raw_str, DateTime timestamp)
        {
            try
            {
                string[] str;
                str = raw_str.Replace("  ", " ").Split(' ');

                if (str.Length > 1 && str[1] == "@GetValue")
                {
                    if (!str[2].StartsWith("Er:"))
                    {
                        var content = new System.Net.Http.StringContent(Convert_StarsToInflux(str, timestamp, tableName));
                        wc.PostAsync(serverUri, content);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }

}
