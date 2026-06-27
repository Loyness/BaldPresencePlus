using BepInEx;
using System;

using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;
using System.Threading;
using Discord;

namespace BaldPresencePlus
{
    [BepInPlugin(ModInfo.GUID, ModInfo.Name, ModInfo.Version)]
    [BepInProcess("BALDI.exe")]
    public class BasePlugin : BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource logsblablabla = BepInEx.Logging.Logger.CreateLogSource("BaldPresencePlus");

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

        public static BasePlugin plugin;

        // Attempt to preload the Discord native library by full path so Mono's P/Invoke
        // finds it regardless of LD_LIBRARY_PATH search order.
        // Falls through gracefully if libdl itself is unavailable — the dllmap config
        // in BaldPresencePlus.dll.config acts as the reliable fallback.
        [DllImport("libdl.so.2", EntryPoint = "dlopen")]
        private static extern IntPtr dlopen_v2(string filename, int flags);
        [DllImport("libdl", EntryPoint = "dlopen")]
        private static extern IntPtr dlopen_v1(string filename, int flags);

        private static IntPtr TryDlopen(string path, int flags)
        {
            try { return dlopen_v2(path, flags); } catch { }
            try { return dlopen_v1(path, flags); } catch { }
            return IntPtr.Zero;
        }

        private static void PreloadDiscordNativeLib()
        {
            if (Environment.OSVersion.Platform != PlatformID.Unix)
                return;

            const int RTLD_NOW = 0x0002;
            const int RTLD_GLOBAL = 0x0100;

            string[] candidates = {
                Path.Combine(Paths.PluginPath,   "libdiscord_game_sdk.so"),
            };

            foreach (string libPath in candidates)
            {
                if (!File.Exists(libPath)) continue;

                IntPtr handle = TryDlopen(libPath, RTLD_NOW | RTLD_GLOBAL);
                if (handle != IntPtr.Zero)
                {
                    logsblablabla.LogInfo("Preloaded native library: " + libPath);
                    return;
                }
                logsblablabla.LogWarning("dlopen returned null for " + libPath);
                return;
            }

            logsblablabla.LogWarning("discord_game_sdk native library not found in BepInEx/plugins or game root.");
        }

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
            harmony.PatchAll();
            plugin = this;

            //CustomOptionsCore.OnMenuInitialize += AddRPCOptions;

            RichPresenceEnabled = plugin.Config.Bind("DiscordRPC", "enabled", true, "If enabled, stats about your Baldi's Basics Plus gameplay will appear on your Discord profile.");
            client_id_config = plugin.Config.Bind("DiscordRPC", "ClientID", 1461650504143605790, "This is client id to the application (If you don't know, please don't touch it)");
            WhichIcon = plugin.Config.Bind("DiscordRPC", "which_icon", 0, "Which icon should be used for the large image of the Rich Presence visualizer. 0 = Placeholder, 1 = Refined, 2 = Plus");

            if (RichPresenceEnabled.Value)
            {
                Thread BB_THREAD = new Thread(DiscordGo);
                BB_THREAD.IsBackground = true;
                BB_THREAD.Start();
            }
        }
        private static void DiscordGo()
        {
            PreloadDiscordNativeLib();

            const int maxAttempts = 12;
            const int retryDelayMs = 5000;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    BB_Discord = new Discord.Discord(client_id_config.Value, (UInt64)CreateFlags.NoRequireDiscord);
                    break;
                }
                catch (DllNotFoundException)
                {
                    logsblablabla.LogError("discord_game_sdk native library not found. Place discord_game_sdk.so / libdiscord_game_sdk.so in the game root or BALDI_Data/Plugins/.");
                    return;
                }
                catch (ResultException ex) when (ex.Result == Result.InternalError)
                {
                    if (attempt == 1)
                        logsblablabla.LogWarning(
                            "Discord SDK returned InternalError — Discord/Equicord may not be running yet " +
                            "or its IPC socket isn't reachable (common with Flatpak). " +
                            "Retrying every " + (retryDelayMs / 1000) + "s (" + maxAttempts + " attempts max)...");

                    if (attempt >= maxAttempts)
                    {
                        logsblablabla.LogError(
                            "Discord SDK failed after " + maxAttempts + " attempts (InternalError). " +
                            "If using Discord Flatpak, bridge the IPC socket: " +
                            "symlink $XDG_RUNTIME_DIR/app/<equicord-app-id>/discord-ipc-0 → /tmp/discord-ipc-0");
                        return;
                    }
                    Thread.Sleep(retryDelayMs);
                    continue;
                }
                catch (Exception e)
                {
                    logsblablabla.LogError("Failed to initialize Discord SDK: " + e.GetType().Name + ": " + e.Message);
                    return;
                }
            }

            if (BB_Discord == null) return;
            BB_ActivityManager = BB_Discord.GetActivityManager();
            if (BB_ActivityManager == null) { logsblablabla.LogWarning("Could not get Discord ActivityManager."); return; }
            BB_ActivityManager.RegisterSteam(1275890);

            BB_Activity.Name = "Baldi's Basics Plus";

            UpdateActivity("Warning Screen", null, "warning", "Warning", "logo_plus_icon", Application.productName + " v." + Application.version + " | Powered by " + ModInfo.Name + " v." + ModInfo.Version);

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
        public static void UpdateActivity(string D, string S, string Li, string Lt, string Si, string St)
        {
            if (plugin == null || BB_ActivityManager == null || !RichPresenceEnabled.Value) return;

            if (D != null) BB_Activity.Details = D;
            if (S != null) BB_Activity.State = S;
            if (Li != null) BB_Activity.Assets.LargeImage = Li;
            if (Lt != null) BB_Activity.Assets.LargeText = Lt;
            if (Si != null) BB_Activity.Assets.SmallImage = Si;
            if (St != null) BB_Activity.Assets.SmallText = St;

            try
            {
                BB_ActivityManager.UpdateActivity(BB_Activity, result => { });
            }
            catch (Exception e) { UnityEngine.Debug.LogError("Discord::UpdateActivity throws a " + e.GetType() + ":\n" + e.Message); }
        }
    }
}
