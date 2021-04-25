using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UPenParticles : MonoBehaviour
{
    public bool Emit = false;

    public ParticleSystem[] particles;

    void Update()
    {
        if (particles != null)
            for (int i = 0; i < particles.Length; ++i)
            {
                var particle = particles[i];
                var em = particle.emission;
                em.enabled = Emit;
            }
    }
}
