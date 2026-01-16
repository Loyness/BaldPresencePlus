using HarmonyLib;

namespace BaldiRPC.ActivityPatches
{

    //INITIALIZE MAIN MENU
    [HarmonyPatch(typeof(MainMenu), "Start")]
    public class InitMainMenu
    {
        static void Postfix()
        {
            BaldiRPC.plugin.UpdateActivity("Main Menu", null, "mainmenu", "Main menu", null, null);
        }
    }

    //QUIT TO MAIN MENU
    [HarmonyPatch(typeof(CoreGameManager), "Quit")]
    public class QuitToMainMenu
    {
        static void Postfix()
        {
            BaldiRPC.plugin.UpdateActivity("Main Menu", "", "mainmenu", "Main menu", null, null);
        }
    }

    //INITIALIZE OPTIONS MENU
    [HarmonyPatch(typeof(OptionsMenu), "Awake")]
    public class InitOptionsMenu
    {
        static void Postfix()
        {
            //if (Singleton<CoreGameManager>.Instance != null) return; //only change text if not playing game
            BaldiRPC.plugin.UpdateActivity("Options", null, "options", "Options", null, null);
        }
    }

    //CLOSE OPTIONS MENU
    [HarmonyPatch(typeof(OptionsMenu), "Close")]
    public class CloseOptionsMenu
    {
        static void Postfix()
        {
            //if (Singleton<CoreGameManager>.Instance != null) return; //only change text if not playing game
            BaldiRPC.plugin.UpdateActivity(null, "", "mainmenu", "Main Menu", null, null);
        }
    }

    //FILE SELECT
    [HarmonyPatch(typeof(NameManager), "Awake")]
    public class FileSelectActivity
    {
        static void Postfix()
        {
            BaldiRPC.plugin.UpdateActivity("File Select", null, "fileselect", "File Select", null, null);
        }
    }

    //START ELEVATOR
    [HarmonyPatch(typeof(ElevatorScreen), "Initialize")]
    class OnChangeToElevator  
    {
        static void Postfix()
        {
            BaldiRPC.plugin.UpdateActivity("Elevator", Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, "elevator", "Elevator", null, null);
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
                BaldiRPC.plugin.UpdateActivity(null, count.ToString() + " Notebooks", null, null, null, null);
            }
            else
            {
                BaldiRPC.plugin.UpdateActivity(null, count.ToString() + "/" + __instance.NotebookTotal.ToString() + " Notebooks", null, null, null, null);
            }
        }
    }

    //BEGI
    [HarmonyPatch(typeof(BaseGameManager), "BeginPlay")]
    class OnChangeToFloor
    {
        static void Postfix()
        {
            if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "END")
            {
                BaldiRPC.plugin.UpdateActivity(Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, Singleton<BaseGameManager>.Instance.FoundNotebooks.ToString() + " Notebooks", "gameplay1", Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, null, null);
            }
            else
            {
                BaldiRPC.plugin.UpdateActivity(Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, Singleton<BaseGameManager>.Instance.FoundNotebooks.ToString() + "/" + Singleton<BaseGameManager>.Instance.NotebookTotal.ToString() + " Notebooks", "gameplay1", Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, null, null);
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
                BaldiRPC.plugin.UpdateActivity(Singleton<CoreGameManager>.Instance.sceneObject.levelTitle + " (Paused)", null, null, null, null, null);
            }
            else
            {
                BaldiRPC.plugin.UpdateActivity(Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, null, null, null, null, null);
            }
        }
    }
}