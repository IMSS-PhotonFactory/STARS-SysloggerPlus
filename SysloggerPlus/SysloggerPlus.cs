using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STARS;

namespace SysloggerPlus
{
    class SysloggerPlus : IDisposable
    {
        private StarsInterface stars;

        private bool useLog;
        private bool useDb;
        private string logpath;
        private InfluxDB_Transfer influxdb;

        private bool disposedValue;

        public bool IsConnected
        {
            get
            {
                return stars.IsConnected;
            }
        }

        public SysloggerPlus(ProgramSetting conf)
        {
            useLog = conf.Use_Logfile;
            useDb = conf.Use_Influxdb;
            
            if(conf.Logfile_Path != "")
            {
                logpath = conf.Logfile_Path;
            }
            else
            {
                logpath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }

            if(useDb)
            {
                switch (conf.Ope_Mode)
                {
                    case OpeMode.Normal:
                        influxdb = new InfluxDB_Transfer(conf.Influxdb_IP, conf.Influxdb_Port, conf.Influxdb_Database, conf.Influxdb_Table);
                        break;
                    case OpeMode.Vacuum:
                        influxdb = new InfluxDB_Transfer_Vac(conf.Influxdb_IP, conf.Influxdb_Port, conf.Influxdb_Database, conf.Influxdb_Table);
                        break;
                    default:
                        break;
                }
            }

            stars = new StarsInterface(conf.STARS_Node, conf.STARS_IP, conf.STARS_Keyfile, conf.STARS_Port);
            if (conf.STARS_Keyword != "")
            {
                stars.KeyWord = conf.STARS_Keyword;
            }

            stars.DataReceived += Stars_DataReceived;

            try
            {
                stars.Connect(true);
            }
            catch (Exception)
            {
                return;
            }
        }

        private void Stars_DataReceived(object sender, StarsCbArgs e)
        {
            var rcvtime = DateTime.Now;


            if (useLog)
            {
                write_file(e.allMessage, rcvtime);
            }

            if(useDb)
            {
                influxdb.Write_StarsLog(e.allMessage, rcvtime);
            }
        }

        private void write_file(string str, DateTime time)
        {
            var fileName = "slog" + time.ToString("yyyy-MM-dd") + ".txt";
            var fullPath = System.IO.Path.Combine(logpath, fileName);

            var log_str = time.ToString("yyyy-MM-dd HH:mm:ss.fff ") + str;

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullPath, true))
            {
                sw.WriteLine(log_str);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    stars.DataReceived -= Stars_DataReceived;
                    stars.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class ProgramSetting
    {
        public string STARS_IP;
        public string STARS_Node;
        public int STARS_Port;
        public string STARS_Keyword;
        public string STARS_Keyfile;

        public bool Use_Logfile;
        public string Logfile_Path;

        public bool Use_Influxdb;
        public string Influxdb_IP;
        public int Influxdb_Port;
        public string Influxdb_Database;
        public string Influxdb_Table;

        public OpeMode Ope_Mode;

        public ProgramSetting()
        {
            STARS_IP = "127.0.0.1";
            STARS_Node = "Debugger";
            STARS_Port = 6057;
            STARS_Keyword = "stars";
            STARS_Keyfile = "";

            Use_Logfile = true;
            Logfile_Path = "";

            Use_Influxdb = true;
            Influxdb_IP = "127.0.0.1";
            Influxdb_Port = 8086;
            Influxdb_Database = "test";
            Influxdb_Table = "debug";
            Ope_Mode = OpeMode.Normal;
        }
    }

    public enum OpeMode
    {
        Normal,
        Vacuum
    }

}
