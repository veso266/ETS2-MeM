using System;

namespace ETS2_MeM
{
    class Variables
    {
        public static string time = String.Empty; //Game Time
        public static bool GameRunning = false;
        public static string HappyText = FileUtils.ReadSetting("HappyText");
        public static string BirthdayText = FileUtils.ReadSetting("BirthdayText");
        public static string CakeLocation = FileUtils.ReadSetting("CakeImage");
    }
}
