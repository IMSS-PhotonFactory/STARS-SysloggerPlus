# STARS-SysloggerPlus

STARS client for logging all STARS message and transfer to InfluxDB.

# Usage

dotnet SysloggerPlus.dll

config.xml, transfernodelist.txt and ignorenodelist.txt will be created at the first boot time.

# config.xml

- <STARS_IP> IP address of STARS server
- <STARS_Port> TCP port of STARS server
- <STARS_Node> Node name for STARS server connection (usually Debugger)
- <STARS_Keyword> Password for STARS server connection
- <STARS_Keyfile> Password filepath for STARS server connection
- <Use_Logfile> Use file logging function (true/false)
- <Logfile_Path> Path for loglile folder
- <Use_Influxdb> Use Influxdb transfer function (true/false)
- <Influxdb_IP> IP address of Influxdb server
- <Influxdb_Port> TCP port of Influxdb server
- <Influxdb_Database> Database name of Influxdb
- <Influxdb_Table> Datatable name of Influxdb
- <Influxdb_MaxConnection> Max simultaneous connections to　Influxdb server
- <Ope_Mode> Operating mode of this program (usually Normal)
- <MaxLogLengthLimit> Max log word length (0 is unlimit)

# transfernodelist.txt
  
nodename list for Influxdb transfer target. (one node in one row)

eg.
term2
term3.sub
term4.*
  
# ignorenodelist.txt
  
nodename list for log ignorling. (one node in one row)
