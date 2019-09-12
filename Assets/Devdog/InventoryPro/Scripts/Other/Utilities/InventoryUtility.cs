using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Reflection;
using System.Text;
using Devdog.General.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    public partial class InventoryUtility
    {
        /// <summary>
        /// Plays an audio clip, only use this for the UI, it is not pooled so performance isn't superb.
        /// TODO: Pool this
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="volume"></param>
        [Obsolete("Use the new InventoryAudioManager.AudioPlayOneShot instead.", true)]
        public static void AudioPlayOneShot(AudioClip clip, float volume = 1.0f)
        {
            Assert.IsNotNull(clip, "AudioClip is null, not allowed.");

            var obj = new GameObject("TEMP_AUDIO_SOURCE_UI");
            var source = obj.AddComponent<AudioSource>();

            source.PlayOneShot(clip, volume);
            UnityEngine.Object.Destroy(obj, clip.length + 0.1f);
        }

        [Obsolete]
        public static int FindIndex<T>(IEnumerable<T> items, Func<T, bool> predicate)
        {
            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item))
                {
                    return retVal;
                }

                retVal++;
            }

            return -1;
        }

        public static void SetLayerRecursive(GameObject obj, int layer)
        {
            Assert.IsNotNull(obj, "Cannot set layers, gameObject given is null! (or object was destroyed while setting layers)");

            obj.layer = layer;
            foreach (Transform t in obj.transform)
            {
                SetLayerRecursive(t.gameObject, layer);
            }
        }

        public static void ResetTransform(Transform transform)
        {
            UIUtility.ResetTransform(transform);
        }

        /// <summary>
        /// Regular GameObject find doesn't handle in-active objects...
        /// </summary>
        public static void FindChildTransform(Transform startObject, string path, ref Transform result)
        {
            // Early bailing if it's already found
            if (result != null)
            {
                return;
            }

            var p = path.Split('/');
            Assert.IsTrue(p.Length > 0, "Not a valid path given...");

            foreach (Transform child in startObject.transform)
            {
                if (child.name == p[p.Length - 1])
                {
                    // Found name of object, check parent names
                    if (p.Length == 1)
                    {
                        result = child;
                        return;
                    }

                    bool isMatch = true;
                    var parent = child.parent;
                    for (int i = p.Length - 2; i >= 0; i--)
                    {
                        if (parent.name != p[i])
                        {
                            isMatch = false;
                            break;
                        }
                    }

                    if (isMatch)
                    {
                        result = child;
                        return;
                    }
                }

                if (child.transform.childCount > 0)
                {
                    FindChildTransform(child, path, ref result);
                }
            }
        }
    }
}
