using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InternetSpeedMeter
{
    class Program
    {
        private  Object _lockObj = new Object();
        public double downloadBytesPerSecond = 0;
        public double uploadBytesPerSecond = 0;
        private Int64 downloadedData = 0;
        private System.Timers.Timer aTimer;
        static NetworkInterface ni = null;
        private System.Timers.Timer dbTimer;
        private DbHandler dbHander = null;
        private string sqlFormattedDate = DateTime.Today.ToString("yyyy-MM-dd");
        private int todayID = 0;
        private System.Timers.Timer niTimer;

        public  void init()
        {
            
                //dbHander = new DbHandler();
                //dbHander.CreateDatabaseAndTable();
               // todayID = dbHander.SearchDataByDate(sqlFormattedDate);
            
            ni = GetNetworkInterface();
            setTimer();
            DbTimer();

        }
        private  void setTimer()
        {
            aTimer = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            getSpeed();
        }
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            long beginValueUpload = ni.GetIPv4Statistics().BytesSent;
            long beginValueDownload = ni.GetIPv4Statistics().BytesReceived;
            DateTime beginTime = DateTime.Now;

            Thread.Sleep(1000);

            long endValueDownload = ni.GetIPv4Statistics().BytesReceived;
            long endValueUpload = ni.GetIPv4Statistics().BytesSent;
            DateTime endTime = DateTime.Now;

            long recievedBytes = endValueDownload - beginValueDownload;
            long sentedBytes = endValueUpload - beginValueUpload;
            double totalSeconds = (endTime - beginTime).TotalSeconds;
            lock (_lockObj)
            {
                downloadedData += recievedBytes + sentedBytes;
                downloadBytesPerSecond = recievedBytes / totalSeconds;
                uploadBytesPerSecond = sentedBytes / totalSeconds;
            }
        }


        public void getSpeed()

        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerAsync();

        }
        public void DbTimer()
        {
            dbTimer = new System.Timers.Timer(60000);
            // Hook up the Elapsed event for the timer. 
            dbTimer.Elapsed += OnDbTimedEvent;
            dbTimer.AutoReset = true;
            dbTimer.Enabled = true;
        }
        public void RefreshNI()
        {
            niTimer = new System.Timers.Timer(60000);
            // Hook up the Elapsed event for the timer. 
            niTimer.Elapsed += RefrashEvent;
            niTimer.AutoReset = true;
            niTimer.Enabled = true;
        }
        private void RefrashEvent(object sender, EventArgs e)
        {
            ni = GetNetworkInterface();
        }
        private void OnDbTimedEvent(object sender, EventArgs e)
        {
            doDbOperation();
        }
        private void doDbOperation()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += bw_doDbOperations;
            bw.RunWorkerAsync();
        }

        private void bw_doDbOperations(object sender, DoWorkEventArgs e)
        {
            
            if (todayID == 0)
            {
                dbHander.AddData(sqlFormattedDate, downloadedData.ToString());
                todayID = dbHander.SearchDataByDate(sqlFormattedDate);
            }
            if (todayID != 0)
            {
                string data = dbHander.GetTodayDownloadedData(todayID);
                Int64 bytes = Int64.Parse(data);
                bytes += downloadedData;
                dbHander.UpdateData(todayID, bytes.ToString());
            }
        }

        public static NetworkInterface GetNetworkInterface()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            var connectedNetworkInterfaces = networkInterfaces.Where(
            counter => counter.OperationalStatus == OperationalStatus.Up &&
            (counter.NetworkInterfaceType != NetworkInterfaceType.Loopback || counter.NetworkInterfaceType != NetworkInterfaceType.Tunnel));
            if (connectedNetworkInterfaces.Count() < 1)
            {
                throw new Exception("The computer does not seem to be connected to the internet.");
            }
            var ni = connectedNetworkInterfaces.First();

            for (int i = 0; i < connectedNetworkInterfaces.Count(); i++)
            {
                ni = connectedNetworkInterfaces.ElementAt(i);
                if (!ni.Name.Contains("VirtualBox") && !ni.Name.Contains("Loopback"))
                {
                    //Console.WriteLine(ni.Name);
                    //Console.WriteLine(ni.Description);
                    //Console.WriteLine(ni.Speed);
                    //Console.WriteLine(ni.NetworkInterfaceType);
                    //Console.WriteLine(ni.IsReceiveOnly);
                    //Console.WriteLine("<=======================>");
                    ni = connectedNetworkInterfaces.ElementAt(i);
                    break;
                }
            }
            return ni;
        }
    }

}