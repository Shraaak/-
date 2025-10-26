using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeRipple : WindInteractable
{
    public ParticleSystem rippleEffect;

    protected override void OnBlow()
    {
        if (!rippleEffect.isPlaying)
        {
            rippleEffect.Play();
        }    
    }
}
