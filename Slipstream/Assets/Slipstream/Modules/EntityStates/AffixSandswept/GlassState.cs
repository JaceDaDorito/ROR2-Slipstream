using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using Slipstream;

namespace EntityStates.Sandswept
{
    public class GlassState : BaseState
    {
        private TemporaryOverlay temporaryOverlay;
        private Material overlay;
        private Animator modelAnimator;
        private float duration;
        private float freezeDuration = 30f;

        public Material frozenOverlayMaterial = SlipAssets.Instance.MainAssetBundle.LoadAsset<Material>("matIsGlass");

        private GameObject eliteSandKnockbackIndicator;

        private bool indicatorEnabled
        {
            get
            {
                return eliteSandKnockbackIndicator;
            }
            set
            {
                if(indicatorEnabled == value)
                {
                    return;
                }
                if (value)
                {
                    GameObject original = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("EliteSandKnockbackIndicator");
                    eliteSandKnockbackIndicator = UnityEngine.Object.Instantiate<GameObject>(original, base.characterBody.corePosition, Quaternion.identity);
                    eliteSandKnockbackIndicator.transform.localScale = new Vector3(characterBody.bestFitRadius * 5, characterBody.bestFitRadius * 5, characterBody.bestFitRadius * 5);
                    eliteSandKnockbackIndicator.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(base.gameObject, null);
                    return;
                }

            }
        }

        //private bool died;

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            /*if (sfxLocator && sfxLocator.barkSound != "")
                Util.PlaySound(base.sfxLocator.barkSound, base.gameObject);*/
            Transform modelTransform = GetModelTransform();
            if (modelTransform)
            {
                CharacterModel model = modelTransform.GetComponent<CharacterModel>();

                if (model)
                {
                    temporaryOverlay = gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = freezeDuration; //doesn't matter what you put here
                    temporaryOverlay.destroyComponentOnEnd = false;
                    temporaryOverlay.destroyObjectOnEnd = false;
                    temporaryOverlay.originalMaterial = frozenOverlayMaterial;
                    temporaryOverlay.AddToCharacerModel(model);

                    //turn off all lights
                    CharacterModel.LightInfo[] lights = model.baseLightInfos;
                    for(int i = 0; i < lights.Length; i++)
                    {
                        lights[i].light.gameObject.SetActive(false);
                    }

                    //model.material
                }
            }
            modelAnimator = base.GetModelAnimator();
            if (modelAnimator)
            {
                modelAnimator.enabled = false;
                duration = freezeDuration;
            }
            if(rigidbody && !rigidbody.isKinematic)
            {
                rigidbody.velocity = Vector3.zero;
                if (rigidbodyMotor)
                    rigidbodyMotor.moveVector = Vector3.zero;
            }
            if (characterDirection)
                characterDirection.moveVector = characterDirection.forward;

            indicatorEnabled = true;
            
        }


        public override void OnExit()
        {
            /*if (temporaryOverlay)
                EntityState.Destroy(temporaryOverlay);
            gameObject.GetComponent<AffixSandswept.AffixSandsweptBehavior>().isGlass = false;
            EffectManager.SpawnEffect()*/

            CharacterBody body = base.characterBody;
            if (body)
            {
                Transform transform = body.modelLocator.modelTransform;
                if (transform)
                {
                    EntityState.Destroy(transform.gameObject);
                    transform = null;
                }
            }

            indicatorEnabled = false;

            Util.PlayAttackSpeedSound("Play_char_glass_death", gameObject, 2f);

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active)
            {
                if (base.fixedAge >= duration)
                {
                    base.characterBody.healthComponent.Suicide();
                }
                    
            }

        }
    }
}