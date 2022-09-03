using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonstorm.Components;

//temporary, this script is scuffed as hell but its just for debugging right now

public class HGControllerFinderBillboard : MonoBehaviour
{
    public ParticleSystem particle;
    public Material material;

    public MaterialControllerComponents.MaterialController materialController = null;
    public void OnEnable()
    {
        if (particle && material)
        {
            materialController = gameObject.AddComponent<MaterialControllerComponents.HGCloudRemapController>();
            if (materialController)
            {
                materialController.material = material;
                materialController.MaterialName = material.name;
                Destroy(this);
            }
            else
                enabled = false;
        }
        else
            enabled = false;
    }
}
