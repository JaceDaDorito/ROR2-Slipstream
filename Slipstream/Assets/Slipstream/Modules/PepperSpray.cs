using R2API;
using RoR2;
using BepInEx;
using UnityEngine;
using static On.RoR2.HealthComponent;
using static R2API.RecalculateStatsAPI;

namespace Slipstream.Modules
{
    class PepperSpray
    {
        public ItemDef itemDef { get; set; } = ContentPackProvider.contentPack.itemDefs.Find("PepperSpray");

        private static float threshhold = 0.5f;

        public void Init()
        {
            AddLanguageTokens();
            Hooks();
        }

        private void AddLanguageTokens()
        {
            LanguageAPI.Add("PEPPERSPRAY_NAME", "Pepper Spray");
            LanguageAPI.Add("PEPPERSPRAYE_PICKUP", "When going below a certain shield threshold, you gain a speed boost and stun enemies around you.");
            LanguageAPI.Add("PEPPERSPRAY_DESC", "PEPPERSPRAY_DESC");
            LanguageAPI.Add("PEPPERSPRAY_LORE", "Do you sometimes wake up at the side of the road unable to move your broken worthless body? Does your sibling sometimes go missing for several days? Is it often that someone pulls up on the curb and shanks you with a knife? Then you need F.O.A. high grade Pepper Spray! The only only thing saving your beautiful *censored*. F.O.A.'s Pepper spray works on space pirates, stray dogs, and even your husband! Just press this button and WOW look at it go!\n\n\"It's just so easy!", "I've never felt more safe before!\"\n\nThe secret behind our powerful technology is the compatibility with your very own personal shield generator! The shield generator will prevent this powerful repelent from damaging your ugly face! And if you dont have your own generator, you can make an extra purchase of [Risk of rain currency system im not sure its called] and we will send you your very own F.O.A. pocket generator!\n\nIf you sign up now you can get another pack of F.O.A.'s signature Pepper Spray free, and if you win our raffle, you can have a chance to win our F.O.A. car sticker too! Don't wait! Contact us today! And you know what they say: *censored* OFF *censored*HOLE!\n\nFor more information visit www.fuahole.com, more interplanetary domain names will be available in the future.");
        }

        private void Hooks()
        {
            GetStatCoefficients += GrantBaseShield;
            TakeDamage += Trigger;
        }

        private void GrantBaseShield(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender.inventory != null && sender.inventory.GetItemCount(itemDef) > 0)
            {
                HealthComponent healthC = sender.GetComponent<HealthComponent>();
                args.baseShieldAdd += healthC.fullHealth * 0.05f;
            }
        }
        /*
        private void Validate(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
        {
            orig(self);

            var shieldComponent = self.GetComponent<IsShieldedComponent>();
            if(!shieldComponent) { shieldComponent = self.gameObject.AddComponent<IsShieldedComponent>(); }

            var isAtMax = self.healthComponent.shield == self.healthComponent.fullShield;

            bool IsDifferent = false;



        } 

        public class IsShieldedComponent : MonoBehaviour
        {
            public bool cachedTrigger = false;
        }*/
        
        //Originally this was going to recharge the item when it went back to the threshold, but getting the ratio of shield before the last hit is the most pain in the ass thing I ever had to sift through. If anyone knows how to do this let me know.

        private void Trigger(orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            CharacterBody hurtBody = self.body;

            if (hurtBody.inventory != null && hurtBody.inventory.GetItemCount(itemDef) > 0) 
            {
                if (self.shield < self.fullShield * threshhold)
                {
                    Chat.AddMessage("Triggered");
                }
            }
            orig(self, damageInfo);
        }

    }
}
