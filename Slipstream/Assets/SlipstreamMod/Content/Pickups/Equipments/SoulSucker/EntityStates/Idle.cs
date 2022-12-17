using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;

namespace EntityStates.SoulSucker
{
    public class Idle : SoulSuckerBaseState
    {
        private float searchTimer;
		private SphereSearch sphereSearch;
		private TeamMask teamMask;
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
				teamMask = TeamMask.GetEnemyTeams(teamIndex);

				sphereSearch = new SphereSearch();
				sphereSearch.radius = grabRadius;
				LayerIndex entityPrecise = LayerIndex.entityPrecise;
				sphereSearch.mask = entityPrecise.mask;
			}
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active)
            {
                FixedUpdateServer();
            }
        }
        public void FixedUpdateServer()
        {
            searchTimer -= Time.fixedDeltaTime;
            if(searchTimer <= 0)
            {
                searchTimer += 0.1f;
				Vector3 position = base.transform.position;
				sphereSearch.origin = position;
				HurtBox[] hurtBoxes = sphereSearch.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(this.teamIndex)).OrderCandidatesByDistance().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes();
				foreach(HurtBox hurtBox in hurtBoxes)
				{
					HealthComponent healthComponent = hurtBox.healthComponent;
                    if (healthComponent)
                    {
                        this.outer.SetNextState(new Grabma { victim = healthComponent });
                        return;
					}
					
				}
			}
        }
        public override void OnExit()
        {
            sphereSearch = null;
            base.OnExit();
        }
    }
}
