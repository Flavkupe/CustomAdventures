using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EffectTester : MonoBehaviourEx
{
    public AnimationEffectData Effect;

    public GameObject Target;

    public GameObject Source;

    private void Start()
    {
        
    }

    private void MakeEffect()
    {
        var effect = Effect.CreateTargetedEffect(Target.transform.position, Source.transform.position);
        StartCoroutine(effect.CreateRoutine());
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            MakeEffect();
        }
    }
}

