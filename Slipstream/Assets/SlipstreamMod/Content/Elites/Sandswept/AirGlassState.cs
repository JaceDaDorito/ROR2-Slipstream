using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using Slipstream;
using Slipstream.Buffs;
using AK;
namespace EntityStates.Sandswept
{
    public class AirGlassState : GlassState
    {

        ICharacterGravityParameterProvider characterGravityParameterProvider;
        ICharacterFlightParameterProvider characterFlightParameterProvider;

        RaycastHit hit;
        bool raycastToGround;
        bool antiGravity = true;
        float differenceFeet;
        float differenceCore;
        float setMass = 40f;
        float setAcc = 10f;

        float bonusRadius;

        DieOnCollision doc;
        float BonusRadius
        {
            get { return this.bonusRadius; }
            set
            {
                if (characterBody.hullClassification == HullClassification.Human)
                    bonusRadius = 2f;
                else if (characterBody.hullClassification == HullClassification.Golem)
                    bonusRadius = 10f;
                else
                    bonusRadius = 20f;
            }
        }

        //Collider collider;

        //private bool died;
        public override void OnEnter()
        {
            base.OnEnter();
            /*if (sfxLocator && sfxLocator.barkSound != "")
                Util.PlaySound(base.sfxLocator.barkSound, base.gameObject);*/
            //characterBody.gameObject.layer = LayerIndex.noCollision.mask;



            /*AffixSandswept.DisableColliders(characterBody);

            SphereCollider setCollider = characterBody.gameObject.AddComponent<SphereCollider>();
            setCollider.enabled = true;
            setCollider.center = new Vector3(0f, 0f, 0f);
            setCollider.radius = characterBody.bestFitRadius;
            setCollider.isTrigger = true;*/

            doc = characterBody.gameObject.AddComponent<DieOnCollision>();
            if(attackerBody)
                doc.attackerBody = attackerBody;

            characterGravityParameterProvider = characterBody.GetComponent<ICharacterGravityParameterProvider>();
            characterFlightParameterProvider = characterBody.GetComponent<ICharacterFlightParameterProvider>();

            if (characterGravityParameterProvider != null)
            {
                CharacterGravityParameters gravityParameters = characterGravityParameterProvider.gravityParameters;
                gravityParameters.channeledAntiGravityGranterCount++;
                characterGravityParameterProvider.gravityParameters = gravityParameters;
            }
            if(characterFlightParameterProvider != null)
            {
                CharacterFlightParameters flightParameters = characterFlightParameterProvider.flightParameters;
                flightParameters.channeledFlightGranterCount++;
                this.characterFlightParameterProvider.flightParameters = flightParameters;
            }

        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active)
            {

                if(fixedAge >= invulnDuration)
                {
                    /*if (antiGravity)
                    {
                        antiGravity = false;
                        if (characterGravityParameterProvider != null)
                        {
                            CharacterGravityParameters gravityParameters = characterGravityParameterProvider.gravityParameters;
                            gravityParameters.channeledAntiGravityGranterCount--;
                            characterGravityParameterProvider.gravityParameters = gravityParameters;
                        }
                        if (characterFlightParameterProvider != null)
                        {
                            CharacterFlightParameters flightParameters = characterFlightParameterProvider.flightParameters;
                            flightParameters.channeledFlightGranterCount--;
                            this.characterFlightParameterProvider.flightParameters = flightParameters;
                        }
                        characterBody.rigidbody.mass = setMass;
                    }*/

                    if (antiGravity)
                    {
                        antiGravity = false;
                        characterBody.rigidbody.mass = setMass;
                        characterBody.rigidbody.drag = 0f;
                        characterBody.acceleration = setAcc;
                    }

                    if(characterMotor)
                        characterMotor.velocity.y += Physics.gravity.y * Time.deltaTime;
                    else if (rigidbody && !rigidbody.useGravity)
                        rigidbody.useGravity = true;

                    raycastToGround = Physics.Raycast(characterBody.footPosition, Vector3.down, out hit, Mathf.Infinity, LayerIndex.world.mask);
                    differenceFeet = characterBody.footPosition.y - hit.point.y;
                    if (raycastToGround && differenceFeet < 1f)
                    {
                        SlipLog.Debug(characterBody + " glass statue died to Raycast.");
                        doc.collided = true;
                        CommitSuicide();
                    }
                    
                }
            }
        }
    }
}