using BepInEx;
using HarmonyLib;
using TMPro;
using UnityEngine;

[BepInPlugin("com.yourname.textspacingmod", "Text Spacing Mod", "1.0.0")]
public class TextSpacingMod : BaseUnityPlugin
{
    void Awake()
    {
        Harmony harmony = new Harmony("com.yourname.textspacingmod");
        harmony.PatchAll();
        Logger.LogInfo("Text Spacing Mod Loaded");
    }

    [HarmonyPatch(typeof(TextMeshProUGUI), "Awake")]
    public class TextMeshProSpacingPatch
    {
        static void Postfix(TextMeshProUGUI __instance)
        {
            // 줄 간격 설정 (기본값 1.0f)
            __instance.lineSpacing = 1.3f;

            // 필요 시 줄 바꿈 활성화
            __instance.enableWordWrapping = true;

            // 기본 폰트 크기 조정 (선택 사항)
            //__instance.fontSize = 24;
        }
    }
}