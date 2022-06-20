using Moonstorm;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using On.RoR2;
using On.RoR2.UI;
using TMPro;
using UnityEngine.UI;

namespace Slipstream.Items
{
    public class CriticalShield
    {
        private static Sprite lowShieldNormal;
        private static Sprite lowShieldVoid;
        private static float threshold;

        public void Init()
        {
            lowShieldNormal = SlipAssets.Instance.MainAssetBundle.LoadAsset<Sprite>("texCriticalShieldIndi");
            lowShieldVoid = SlipAssets.Instance.MainAssetBundle.LoadAsset<Sprite>("texCriticalVoidShiIndi");
            threshold = PepperSpray.threshold;
            On.RoR2.UI.HealthBar.UpdateHealthbar += new On.RoR2.UI.HealthBar.hook_UpdateHealthbar(this.HealthBar_UpdateHealthbar);
        }
        private void HealthBar_UpdateHealthbar(On.RoR2.UI.HealthBar.orig_UpdateHealthbar orig, RoR2.UI.HealthBar self, float deltaTime)
        {
            //invoke orig before the UpdateBar call so that the bar is set above other HealthBar elements
            orig.Invoke(self, deltaTime);
            CriticalShield.CriticalShieldBehavior component = null;
            if (self.source)
            {
                component = self.source.GetComponent<CriticalShield.CriticalShieldBehavior>();
                if (component)
                    component.UpdateBar(self);
            }
            if (component)
                component.ApplyBar(self);
        }
        public abstract class CriticalShieldBehavior : BaseItemBodyBehavior
        {
            private Image image;

            private bool shouldTrigger = false;

            public virtual void FixedUpdate()
            {
                //Checks if the body is at full shield. shouldTrigger is just a switch to make sure that the effect doesn't trigger more than once below the shield threshold.
                if (body.healthComponent.shield == body.healthComponent.fullShield && !shouldTrigger)
                    shouldTrigger = true;

                //Checks if the body is lower than the shield threshold percentage.
                if (body.healthComponent.shield < body.healthComponent.fullShield * threshold && shouldTrigger)
                {
                    shouldTrigger = false;
                    Trigger();
                }
            }

            public abstract void Trigger();
            public virtual void OnDestroy()
            {
                //PepperSpray.PepperSprayBehavior component = body.GetComponent<PepperSpray.PepperSprayBehavior>();
                if (image)
                {
                    if (Application.isPlaying)
                        Destroy(image.gameObject);
                    else
                        DestroyImmediate(image.gameObject);
                    image = null;
                }
            }

            //thanks groove
            public void UpdateBar(RoR2.UI.HealthBar healthBar)
            {
                if (!healthBar.barAllocator.containerTransform.gameObject.scene.IsValid())
                {
                    return;
                }
                CriticalShield.CriticalShieldBehavior component = healthBar.source.GetComponent<CriticalShield.CriticalShieldBehavior>();
                if (!shouldTrigger)
                {
                    //destroy image gameObject if the item triggers.
                    if (image)
                    {
                        if (Application.isPlaying)
                            Destroy(image.gameObject);
                        else
                            DestroyImmediate(image.gameObject);
                        image = null;
                    }
                    return;
                }
                if (!image)
                {
                    //reinstantiate image if the image doesn't exist (which it should)
                    image = Instantiate(healthBar.barAllocator.elementPrefab, healthBar.barAllocator.containerTransform).GetComponent<Image>();
                    GameObject gameObject = image.gameObject;
                    if (healthBar.barAllocator.markElementsUnsavable)
                        gameObject.hideFlags |= (HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild);
                    gameObject.SetActive(true);
                    int length = healthBar.barAllocator.elements.Count;
                    healthBar.barAllocator.AllocateElements(length - 1);
                }
            }

            public void ApplyBar(RoR2.UI.HealthBar healthBar)
            {
                if (image && healthBar.source)
                {
                    //image.type = healthBar.style.
                    RoR2.HealthComponent.HealthBarValues healthBarValues = healthBar.source.GetHealthBarValues();
                    image.sprite = healthBarValues.hasVoidShields ? lowShieldVoid : lowShieldNormal;
                    image.color = new Color(1f, 1f, 1f, 1f);

                    //starts at health end
                    float minPos = healthBarValues.healthFraction;

                    //starts at shield bar threshold
                    //can't use healthBarValues.shieldFraction because that only takes account to your current shield, not full shield
                    float fullShieldFraction = ((body.healthComponent.fullShield * threshold) / (body.healthComponent.fullShield + body.healthComponent.fullHealth)) * (1f - healthBarValues.curseFraction);
                    float maxPos = healthBarValues.healthFraction + (fullShieldFraction);

                    SetRectPosition((RectTransform)image.transform, minPos, maxPos, 1f);
                }
            }

            public static void SetRectPosition(RectTransform rectTransform, float xMin, float xMax, float sizeDelta)
            {
                rectTransform.anchorMin = new Vector2(xMin, 0f);
                rectTransform.anchorMax = new Vector2(xMax, 1f);
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = new Vector2(sizeDelta * 0.5f + 1f, sizeDelta + 1f);
            }
        }
    }
    
}
