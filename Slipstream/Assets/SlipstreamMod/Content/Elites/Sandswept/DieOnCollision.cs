using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using Slipstream;
using Slipstream.Buffs;
using AK;
using Slipstream.Orbs;
using RoR2.Orbs;

public class DieOnCollision : MonoBehaviour
{
    private CharacterBody body;
    public CharacterBody attackerBody;
    public bool collided = false;

    private void Start()
    {
        body = gameObject.GetComponent<CharacterBody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //SlipLog.Debug(body.name + " collided with something");
        if(collision.gameObject.layer == LayerIndex.world.intVal && !collided)
        {
            collided = true;
            //SlipLog.Debug(body.name + " died");
            AffixSandswept.FireKBBlast(body);
            AffixSandswept.CreateOrb(body, attackerBody);
            SlipLog.Debug(gameObject + " glass statue died to Collision");
            if (attackerBody)
                body.healthComponent.Suicide(attackerBody?.gameObject);
            else
                body.healthComponent.Suicide();

            Destroy(this);
        }
    }
}
