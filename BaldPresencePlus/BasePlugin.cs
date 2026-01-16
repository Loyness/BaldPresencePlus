using BepInEx;
using System;
using MTM101BaldAPI.OptionsAPI;
using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;
using MTM101BaldAPI;
using System.Threading;
using Discord;


namespace BaldiRPC
{
    [BepInPlugin(ModInfo.GUID, ModInfo.Name, ModInfo.Version)]
    [BepInProcess("BALDI.exe")]
    public class BaldiRPC : BaseUnityPlugin
    {
        //DISCORD VARIABLES
        public static Discord.Discord BB_Discord = null;
        public static ActivityManager BB_ActivityManager = null;
        public static Discord.Activity BB_Activity = new Discord.Activity { Instance = true };

        public static ConfigEntry<bool> RichPresenceEnabled;
        public static ConfigEntry<long> client_id_config;
        public static ConfigEntry<int> WhichIcon;

        public static AdjustmentBars iconChangeBar;
        public static TextLocalizer iconNameText;

        public string state_string;
        public string detail_string;
        public string extra_state_string;
        public string extra_detail_string;

        public static BaldiRPC plugin;

        internal string[] iconNames = {
                "Placeholder",
                "Refined",
                "Plus"
            };

        internal static class ModInfo
        {
            internal const string GUID = "imloyness.baldpresenceplus";
            internal const string Name = "BaldPresence+";
            internal const string Version = "1.0";
        }

        internal static class RPInfo
        {
            internal const long client_id = 1461650504143605790;
        }

        void Awake()
        {
            Harmony harmony = new Harmony(ModInfo.GUID);
            harmony.PatchAllConditionals();
            plugin = this;

            var discord = new Discord.Discord(RPInfo.client_id, (UInt64)Discord.CreateFlags.Default);
            //CustomOptionsCore.OnMenuInitialize += AddRPCOptions;

            RichPresenceEnabled = plugin.Config.Bind("DiscordRPC", "enabled", true, "If enabled, stats about your Baldi's Basics Plus gameplay will appear on your Discord profile.");
            client_id_config = plugin.Config.Bind("DiscordRPC", "ClientID", 1461650504143605790, "This is client id to the application (If you don't know, please don't touch it)");
            WhichIcon = plugin.Config.Bind("DiscordRPC", "which_icon", 0, "Which icon should be used for the large image of the Rich Presence visualizer. 0 = Placeholder, 1 = Refined, 2 = Plus");

            //only start if it's enabled
            if (RichPresenceEnabled.Value)
            {
                Thread BB_THREAD = new Thread(DiscordGo);
                BB_THREAD.Start();
            }
        }
        private static void DiscordGo()
        {
            BB_Discord = new Discord.Discord(client_id_config.Value, (UInt64)CreateFlags.NoRequireDiscord);
            BB_ActivityManager = BB_Discord.GetActivityManager();
            if (BB_ActivityManager == null) return;
            BB_ActivityManager.RegisterSteam(1275890);

            BB_Activity.Name = "Baldi's Basics Plus";

            plugin.UpdateActivity("Warning Screen", null, "warning", "Warning", "logo_plus_icon", Application.productName + " v." + Application.version + " | Powered by " + ModInfo.Name + " v." + ModInfo.Version);

            try
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(500);  
                        BB_Discord.RunCallbacks();
                    }
                    catch (ResultException e)
                    {
                        UnityEngine.Debug.LogError("Discord throws a ResultException: " + e.Message);
                    }
                }
            }
            finally
            {
                BB_Discord.Dispose();
            }
        }
        public void UpdateActivity(string D, string S, string Li, string Lt, string Si, string St)
        {
            if (BB_ActivityManager != null && RichPresenceEnabled.Value)
            {
                if (D != null) BB_Activity.Details = D;
                if (S != null) BB_Activity.State = S;
                if (Li != null) BB_Activity.Assets.LargeImage = Li;
                if (Lt != null) BB_Activity.Assets.LargeText = Lt;
                if (Si != null) BB_Activity.Assets.SmallImage = Si;
                if (St != null) BB_Activity.Assets.SmallText = St;

            }
            try
            {
                BB_ActivityManager.UpdateActivity(BB_Activity, result => { });
            }
            catch (Exception e) { UnityEngine.Debug.LogError("Discord::UpdateActivity throws a " + e.GetType() + ":\n" + e.Message); }
        }
    }
}
