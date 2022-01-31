using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace SysloggerPlus
{
    class Program
    {
        static SysloggerPlus syslogger;

        static bool exitFlg = false;

        static string settingFilename = "config.xml";
        static string nodelistFilename = "transfernodelist.txt";
        static string ignorelistFilename = "ignorenodelist.txt";

        static void Main(string[] args)
        {

            ProgramSetting setting;

            var configfile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), settingFilename);
            XmlSerializer serializer = new XmlSerializer(typeof(ProgramSetting));

            if (System.IO.File.Exists(configfile))
            {
                using(System.IO.StreamReader sr = new System.IO.StreamReader(configfile))
                {
                    setting = (ProgramSetting)serializer.Deserialize(sr);
                }
            }
            else
            {
                Console.WriteLine("The configuration file does not exist.");
                setting = new ProgramSetting();
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(configfile, false, new System.Text.UTF8Encoding(false)))
                {
                    serializer.Serialize(sw, setting);
                }
                Console.WriteLine("New configuration file has been created.");
                Console.WriteLine("Modify it and restart program.");
                return;
            }

            var nodeList = new List<string>();

            var nodelistfile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), nodelistFilename);
            var ignorelistfile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), ignorelistFilename);


            syslogger = new SysloggerPlus(setting, nodelistfile, ignorelistfile);

            while (!exitFlg)
            {
                System.Threading.Thread.Sleep(50);
                if(!syslogger.IsConnected)
                {
                    return;
                }
            }
            return;
        }
    }
}
