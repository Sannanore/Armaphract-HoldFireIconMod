using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using MelonLoader;
using HarmonyLib;
using Il2CppSystem.IO;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime;

namespace BetterHUD
{
    public class HoldFireModClass : MelonMod
    {
        private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);

        private static DLoadImage _iCallLoadImage;

        public static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            if (_iCallLoadImage == null)
            {
                _iCallLoadImage = IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
            }

            var il2CPPArray = (Il2CppStructArray<byte>)data;

            return _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
        }
        //public override void OnUpdate()
        //{
        //    if (Input.GetKeyDown(KeyCode.J))
        //    {
        //        Il2Cpp.Unit k75a = GameObject.Find("UNITS/Player/playersquad/K-75A").GetComponent<Il2Cpp.Unit>();
        //        GameObject unitIcon = GameObject.Find("UNITS/Player/playersquad/K-75A/unitRect/unitIcon");
        //        if (k75a.fireAtWill == true)
        //        {
        //            unitIcon.SetActive(true);
        //        }
        //        else
        //        {
        //            unitIcon.SetActive(false);
        //        }
        //    }
        //}
    }

    [HarmonyPatch]
    public static class Il2Cpp_Unit_Patch
    {
        //[HarmonyPatch(typeof(Il2Cpp.Unit), nameof(Il2Cpp.Unit.ToggleROE))]
        //[HarmonyPostfix]
        //public static void ToggleROE_Postfix()
        //{
        //    Il2Cpp.Unit k75a = GameObject.Find("UNITS/Player/playersquad/K-75A").GetComponent<Il2Cpp.Unit>();
        //    GameObject unitIcon = GameObject.Find("UNITS/Player/playersquad/K-75A/unitRect/unitIcon");
        //    if (k75a.fireAtWill == true)
        //    {
        //        unitIcon.SetActive(true);
        //    }
        //    else
        //    {
        //        unitIcon.SetActive(false);
        //    }
        //}

        [HarmonyPatch(typeof(Il2Cpp.Unit), "ToggleROE")]
        private static class Patch
        {
            //public static bool already_created = false;
            private static void Postfix(Il2Cpp.Unit __instance) // UNITS/Player/playersquad/K-75A
            {
                //Il2Cpp.Unit k75a = GameObject.Find("UNITS/Player/playersquad/K-75A").GetComponent<Il2Cpp.Unit>();
                Il2Cpp.Unit k75a = __instance.GetComponent<Il2Cpp.Unit>();
                //GameObject holdFireIcon_existing = GameObject.Find("UNITS/Player/playersquad/K-75A/unitRect/holdFireIcon");
                GameObject holdFireIcon_existing = __instance.transform.GetChild(10).gameObject; // UNITS/Player/playersquad/K-75A/unitRect/holdFireIcon
                if (holdFireIcon_existing.transform.childCount >= 15)
                {
                    holdFireIcon_existing = holdFireIcon_existing.transform.GetChild(14).gameObject;
                }
                else
                {
                    holdFireIcon_existing = null;
                }

                if (holdFireIcon_existing == null)
                {
                    GameObject unitIcon = __instance.transform.GetChild(10).GetChild(9).gameObject; // UNITS/Player/playersquad/K-75A/unitRect/unitIcon
                    GameObject holdFireIcon = GameObject.Instantiate(unitIcon);
                    holdFireIcon.transform.SetParent(__instance.transform.GetChild(10).transform);
                    holdFireIcon.name = "holdFireIcon";
                    holdFireIcon.transform.position = unitIcon.transform.position;
                    holdFireIcon.transform.rotation = unitIcon.transform.rotation;
                    holdFireIcon.transform.localPosition = new Vector3(holdFireIcon.transform.localPosition.x, holdFireIcon.transform.localPosition.y - 0.3f, holdFireIcon.transform.localPosition.z);
                    holdFireIcon.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    holdFireIcon.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                    //already_created = true;
                    string iconpath = Path.Combine(MelonLoader.Utils.MelonEnvironment.ModsDirectory, "holdfireicon.png");
                    byte[] pngBytes = File.ReadAllBytes(iconpath);
                    Texture2D tex = new Texture2D(2, 2);
                    //tex.LoadImage(pngBytes);
                    HoldFireModClass.LoadImage(tex, pngBytes, false);
                    Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    holdFireIcon.GetComponent<SpriteRenderer>().sprite = newSprite;
                }
                else
                {
                    if (k75a.fireAtWill == true)
                    {
                        holdFireIcon_existing.SetActive(false);
                    }
                    else
                    {
                        holdFireIcon_existing.SetActive(true);
                    }
                }
            }
        }
    }
}
