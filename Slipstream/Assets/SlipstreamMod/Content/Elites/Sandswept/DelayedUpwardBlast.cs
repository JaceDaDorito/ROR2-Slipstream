using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using Slipstream;
using Slipstream.Buffs;
using AK;
using KinematicCharacterController;

namespace Slipstream.Components
{
    public class DelayedUpwardBlast : MonoBehaviour
    {
        public float combinedRadius;
        public Vector3 position;
        public Vector3 searchDirection;
        public float outwardForce;
        public float upwardForce;
        public float time;
        public TeamIndex team;
        private Run.FixedTimeStamp timer;
        
        //private bool didntFire = true;

        private void OnEnable()
        {
           timer = Run.FixedTimeStamp.now;
        }

        private void FixedUpdate()
        {
            if( timer.timeSince >= time)
            {
                //didntFire = false;
                UpwardBlast();

                Destroy(gameObject);
            }
        }
        private void UpwardBlast()
        {
            //Util.PlayAttackSpeedSound("Play_char_glass_death", body.gameObject, 2f);
            //Vector3 corePosition = RoR2.Util.GetCorePosition(body.gameObject);

            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.teamMaskFilter = TeamMask.GetEnemyTeams(team);
            bullseyeSearch.filterByDistinctEntity = true;
            bullseyeSearch.filterByLoS = false;
            bullseyeSearch.maxDistanceFilter = combinedRadius;
            bullseyeSearch.searchOrigin = position;
            bullseyeSearch.searchDirection = searchDirection;
            bullseyeSearch.RefreshCandidates();

            SlipLogger.LogD("Sandswept Bullseye Search Creation");
            IEnumerable<HurtBox> list = bullseyeSearch.GetResults();
            SlipLogger.LogD("Sandswept Bullseye Search Results");
            foreach (HurtBox hurtbox in list)
            {
                CharacterBody indexBody = hurtbox.healthComponent.body;
                
                if (indexBody && indexBody.characterMotor && indexBody.master && !AffixSandswept.BlacklistedBodyIndices.Contains(indexBody.bodyIndex) /*&& indexBody.teamComponent.teamIndex != team &&*/)
                {
                    KinematicCharacterMotor indexKCC = indexBody.GetComponent<KinematicCharacterMotor>();
                    if (indexKCC)
                        indexKCC.ForceUnground();

                    Vector3 result = indexBody.corePosition - position;
                    result = result.normalized;
                    PhysForceInfo physInfo = new PhysForceInfo()
                    {
                        force = new Vector3(result.x * outwardForce, upwardForce, result.z * outwardForce),
                        ignoreGroundStick = true,
                        disableAirControlUntilCollision = false,
                        massIsOne = false //MassIsOne just means if mass will have a factor in the force. I set it to false because I want the mass to contribute.
                    };
                    indexBody.characterMotor.ApplyForceImpulse(physInfo);
                }
            }

            //Util.CharacterSpherecast

            //Both of these knockbacks were previous iterations of the knockback. Keeping them here for archival purposes.
            #region BlastAttackKB
            //RaycastHit hit;
            //bool isRaycastExplosion = Physics.Raycast(corePosition, Vector3.down, out hit, combinedRadius * 2f, LayerIndex.world.mask);

            //blastAttack that launches you into the air
            /*BlastAttack blastAttack = new BlastAttack();
            blastAttack.radius = combinedRadius;
            blastAttack.position = corePosition;
            blastAttack.attacker = body.gameObject;
            blastAttack.falloffModel = BlastAttack.FalloffModel.None;
            blastAttack.damageType = DamageType.Silent;
            blastAttack.baseForce = 0f;
            blastAttack.bonusForce.y = 4000f;
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
            blastAttack.attackerFiltering = AttackerFiltering.AlwaysHit;
            blastAttack.Fire();*/
            #endregion

            #region 2PartKB
            //Disables all colliders when fired
            /*
            Collider[] collider = body.gameObject.GetComponents<Collider>();
            if (collider.Length > 0)
            {
                for (int i = 0; i < collider.Length; i++)
                    collider[i].enabled = false;

            }


            //Bullseye search to get bodies that are grounded
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.teamMaskFilter = TeamMask.all;
            bullseyeSearch.filterByDistinctEntity = true;
            bullseyeSearch.filterByLoS = true;
            bullseyeSearch.maxDistanceFilter = combinedRadius;
            bullseyeSearch.searchOrigin = corePosition;
            bullseyeSearch.searchDirection = body.transform.forward;
            bullseyeSearch.RefreshCandidates();

            SlipLogger.LogD("Sandswept Bullseye Search Creation");
            IEnumerable<HurtBox> list = bullseyeSearch.GetResults();
            SlipLogger.LogD("Sandswept Bullseye Search Results");
            foreach (HurtBox hurtbox in list)
            {
                CharacterBody indexBody = hurtbox.healthComponent.body;
                float difference = Mathf.Max(0, corePosition.y - indexBody.corePosition.y);
                //if the body has a motor and is grounded or at a certain height below the core of the explosion, launch the body
                if (indexBody && indexBody.characterMotor && (indexBody.characterMotor.isGrounded || difference > 0) && indexBody != body)
                {
                    PhysForceInfo physInfo = new PhysForceInfo()
                    {
                        //should hopefully scale the force needed to lift someone above the core
                        force = Vector3.up * (initialConstantForce + (difference * differenceForceMultiplier)),
                        ignoreGroundStick = true,
                        disableAirControlUntilCollision = false,
                        massIsOne = true
                    };
                    indexBody.characterMotor.ApplyForceImpulse(physInfo);
                }
            }

            GameObject knockbackExplosionInstance = UnityEngine.Object.Instantiate<GameObject>(knockbackExplosion, corePosition, Quaternion.identity);
            DelayBlast delayBlast = knockbackExplosionInstance.GetComponent<DelayBlast>();
            if (delayBlast)
            {
                delayBlast.position = corePosition;
                delayBlast.attacker = body.gameObject;
                delayBlast.falloffModel = BlastAttack.FalloffModel.None;
                delayBlast.damageType = DamageType.Silent;
                delayBlast.baseForce = secondForce;
                delayBlast.radius = combinedRadius;
                delayBlast.maxTimer = 0.25f;
            }
            NetworkServer.Spawn(knockbackExplosionInstance);*/
            #endregion
        }
    }
}
