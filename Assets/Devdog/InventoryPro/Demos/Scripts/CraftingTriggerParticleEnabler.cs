using UnityEngine;
using System.Collections;
using Devdog.InventoryPro;


namespace Devdog.InventoryPro.Demo
{
    public class CraftingTriggerParticleEnabler : MonoBehaviour
    {

        public ParticleSystem[] emitters = new ParticleSystem[0];

        protected void Start()
        {
            var layout = GetComponent<CraftingLayoutTrigger>();
            var standard = GetComponent<CraftingStandardTrigger>();

            if (layout != null)
            {
                layout.OnCraftStart += CraftStart;
                layout.OnCraftSuccess += CraftSuccess;
                layout.OnCraftCancelled += CraftCancelled;
                layout.OnCraftFailed += CraftFailed;
            }
            else if (standard != null)
            {
                standard.OnCraftStart += CraftStart;
                standard.OnCraftSuccess += CraftSuccess;
                standard.OnCraftCancelled += CraftCancelled;
                standard.OnCraftFailed += CraftFailed;
            }
        }

        private void CraftCancelled(CraftingProgressContainer.CraftInfo craftInfo, float progress)
        {
            HideParticles();
        }

        private void CraftFailed(CraftingProgressContainer.CraftInfo craftInfo)
        {
            HideParticles();
        }

        private void CraftSuccess(CraftingProgressContainer.CraftInfo craftInfo)
        {
            HideParticles();
        }

        private void CraftStart(CraftingProgressContainer.CraftInfo craftInfo)
        {
            ShowParticles();
        }

        private void HideParticles()
        {
            foreach (var emitter in emitters)
            {
                emitter.Stop();
            }
        }

        private void ShowParticles()
        {
            foreach (var emitter in emitters)
            {
                emitter.Play();
            }
        }
    }
}