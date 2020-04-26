using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
using SCSSdkClient;
using SCSSdkClient.Object;
using ETS2_MeM.Hooks;
using System.IO;
using System.Drawing;

namespace ETS2_MeM
{
    class Program
    {
        public SCSSdkTelemetry Telemetry; //To get Games Time
        private System.Threading.Timer currentGameTimer; //Game Timer
        public static object ets2data;

        public static string simulatorNotRunning = "Simulator not yet running";
        public static string simulatorNotDriving = "Simulator running, let's get driving!";
        public static string simulatorRunning = "Simulator running!";
        public static string currentGame = "none";
		public static bool GameRunning = false;

        static int BirthdayAactive = 0; //Use to tell when Birthday starts

        #region Text

        #endregion
        #region TextColors
        Brush TextColor = Brushes.White;
        Brush ColoredTextColor = new SolidBrush(Color.FromArgb(255, 174, 0)); //Yellow Color like in ETS2
        #endregion
        private void Telemetry_Data(SCSTelemetry data, bool updated)
        {
            try
            {
                ets2data = data;
                
                if (data.GameVersion.Major == 0)
                {
                    Console.Title = simulatorNotRunning;
					GameRunning = false;
                }
                else if (data.Timestamp == 0)
                {
                    Console.Title = simulatorNotDriving;
					GameRunning = true;
                }
                else
                {
                    Console.Title = simulatorRunning;
					GameRunning = true

                    //Get game time, with ugly code but it works
                    string[] uraArr = data.CommonValues.GameTime.Date.ToUniversalTime().ToString().Split(' ')[1].Split(':');
                    Variables.time = string.Join(":", uraArr.Reverse().Skip(1).Reverse());

                    //Game status (true=running, false=paused)
                    Variables.GameRunning = !data.Paused;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        //Every second we check if we are attach to the process of the game
        //We need to be attached to game process, because we hook into patched dll and we can draw
        private void currentGameTimer_Tick(object sender)
        {
            bool ets2Found = false;
            bool atsFound = false;

            if (Process.GetProcessesByName("eurotrucks2").Length > 0)
            {
                if (currentGame != "ets2")
                {
                    currentGame = "ets2";
                    Console.Title = "Euro Truck Simulator 2";
					if (GameRunning)
						MemWritter.AttachProcess("eurotrucks2");
                }
                ets2Found = true;
            }
            if (Process.GetProcessesByName("amtrucks").Length > 0)
            {
                if (currentGame != "ats")
                {
                    currentGame = "ats";
                    Console.Title = "American Truck Simulator";
					if (GameRunning)
						MemWritter.AttachProcess("amtrucks");
                }
                atsFound = true;
            }

            if (!ets2Found && !atsFound)
            {
                currentGame = "none";
            }
            if (ets2Found && Variables.GameRunning)
            {
                if (Variables.time == "0:00") //to set time in ETS2/ATS type in console: g_set_time XX [YY] eg: g_set_time 23 59 to set time to 23:59
                {
                    MemWritter.DrawMEM(Variables.HappyText, TextColor, Variables.BirthdayText, ColoredTextColor, "center", Directory.GetCurrentDirectory() + "\\" + Variables.CakeLocation, 5000);
                    BirthdayAactive++;
                }
                if (BirthdayAactive > 0)
                    Console.WriteLine("Vse najboljše Nika!!, zdej lohka pa tud v amerik alkohol piješ :) ");
                    
            }
        }
        static void Main(string[] args)
        {
            Program prg = new Program();
            //Start game timer
            prg.currentGameTimer = new System.Threading.Timer(prg.currentGameTimer_Tick, 5, 0, 1000); //Gane Timer

            //Start telemetry grabbing:
            prg.Telemetry = new SCSSdkTelemetry(250);
            prg.Telemetry.Data += prg.Telemetry_Data;

            if (prg.Telemetry.Error != null)
            {
                System.Windows.Forms.MessageBox.Show(
                    "General info:\r\nFailed to open memory map " + prg.Telemetry.Map +
                        " - on some systems you need to run the client (this app) with elevated permissions, because e.g. you're running Steam/ETS2 with elevated permissions as well. .NET reported the following Exception:\r\n" +
                        prg.Telemetry.Error.Message + "\r\n\r\nStacktrace:\r\n" + prg.Telemetry.Error.StackTrace);
            }
            Console.ReadLine();
        }
    }
}
