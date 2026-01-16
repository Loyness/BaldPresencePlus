using System;
using HarmonyLib;
using UnityEngine;

namespace BaldPresencePlus.ActivityPatches
{

    //INITIALIZE MAIN MENU
    [HarmonyPatch(typeof(MainMenu), "Start")]
    public class InitMainMenu
    {
        static void Postfix()
        {
            BasePlugin.plugin.UpdateActivity("Main Menu", null, "mainmenu", "Main menu", null, null);
        }
    }

    //QUIT TO MAIN MENU
    [HarmonyPatch(typeof(CoreGameManager), "Quit")]
    public class QuitToMainMenu
    {
        static void Postfix()
        {
            BasePlugin.plugin.UpdateActivity("Main Menu", "", "mainmenu", "Main menu", null, null);
        }
    }

    //INITIALIZE OPTIONS MENU
    [HarmonyPatch(typeof(OptionsMenu), "Awake")]
    public class InitOptionsMenu
    {
        static void Postfix()
        {
            if (Singleton<CoreGameManager>.Instance != null) return; //only change text if not playing game
            BasePlugin.plugin.UpdateActivity("Options", null, "options", "Options", null, null);
        }
    }

    //CLOSE OPTIONS MENU
    [HarmonyPatch(typeof(OptionsMenu), "Close")]
    public class CloseOptionsMenu
    {
        static void Postfix()
        {
            if (Singleton<CoreGameManager>.Instance != null) return; //only change text if not playing game
            BasePlugin.plugin.UpdateActivity("Main Menu", "", "mainmenu", "Main Menu", null, null);
        }
    }

    //FILE SELECT
    [HarmonyPatch(typeof(NameManager), "Awake")]
    public class FileSelectActivity
    {
        static void Postfix()
        {
            BasePlugin.plugin.UpdateActivity("File Select", null, "fileselect", "File Select", null, null);
        }
    }

    //START ELEVATOR
    [HarmonyPatch(typeof(ElevatorScreen), "Initialize")]
    class OnChangeToElevator  
    {
        static void Postfix()
        {
            BasePlugin.plugin.UpdateActivity("Elevator", Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, "elevator", "Elevator", null, null);
        }
    }

    //COLLECT NOTEBOOK
    [HarmonyPatch(typeof(BaseGameManager), "CollectNotebooks")]
    class OnCollectNotebook
    {
        static void Postfix(BaseGameManager __instance, int count)
        {
            if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "END")
            {
                BasePlugin.plugin.UpdateActivity(null, count.ToString() + " Notebooks", null, null, null, null);
            }
            else
            {
                BasePlugin.plugin.UpdateActivity(null, count.ToString() + "/" + __instance.NotebookTotal.ToString() + " Notebooks", null, null, null, null);
            }
        }
    }

    //BEGI
    [HarmonyPatch(typeof(BaseGameManager), "BeginPlay")]
    class OnChangeToFloor
    {
        public static string[] gameplayIcons = { "gameplay1", "gameplay2", "gameplay3", "gameplay4", "gameplay5", "gameplay6", "gameplay7", "gameplay8", "gameplay9", "gameplay10", "gameplay11", "gameplay12", "gameplay13" };
        static void Postfix()
        {
            BasePlugin.logsblablabla.LogInfo(gameplayIcons[UnityEngine.Random.Range(0, gameplayIcons.Length)]);
            if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "END")
            {
                BasePlugin.plugin.UpdateActivity(Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, Singleton<BaseGameManager>.Instance.FoundNotebooks.ToString() + " Notebooks", gameplayIcons[UnityEngine.Random.Range(0, gameplayIcons.Length)], Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, null, null);
            }
            else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "PIT")
            {
                BasePlugin.plugin.UpdateActivity(Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, Singleton<CoreGameManager>.Instance.GetPoints(0).ToString() + " YTPs", "pitstop", Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, null, null);
            }
            else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "FT")
            {
                BasePlugin.plugin.UpdateActivity(Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, "Exploring around", "fieldtrip", Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, null, null);
            }
            else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "FARM")
            {
                BasePlugin.plugin.UpdateActivity(Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, "Something is on the works.. - Baldi", "gameplay_", Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, null, null);
            }
            else
            {
                BasePlugin.plugin.UpdateActivity(Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, Singleton<BaseGameManager>.Instance.FoundNotebooks.ToString() + "/" + Singleton<BaseGameManager>.Instance.NotebookTotal.ToString() + " Notebooks", gameplayIcons[UnityEngine.Random.Range(0, gameplayIcons.Length)], Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, null, null);
            }
        }
    }

    //PAUSE FLOOR
    [HarmonyPatch(typeof(CoreGameManager), "Pause")]
    class OnPause
    {
        static void Postfix(CoreGameManager __instance)
        {
            if (__instance.Paused)
            {
                BasePlugin.plugin.UpdateActivity(Singleton<CoreGameManager>.Instance.sceneObject.levelTitle + " (Paused)", null, null, null, null, null);
            }
            else
            {
                BasePlugin.plugin.UpdateActivity(Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, null, null, null, null, null);
            }
        }
    }
}