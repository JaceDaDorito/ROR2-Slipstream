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
        //SlipLogger.LogD(body.name + " collided with something");
        if(collision.gameObject.layer == LayerIndex.world.intVal && !collided)
        {
            collided = true;
            //SlipLogger.LogD(body.name + " died");
            AffixSandswept.FireKBBlast(body);
            AffixSandswept.CreateOrb(body, attackerBody);
            SlipLogger.LogD(gameObject + " glass statue died to Collision");
            body.healthComponent.Suicide(attackerBody?.gameObject);

            Destroy(this);
        }
    }
}
