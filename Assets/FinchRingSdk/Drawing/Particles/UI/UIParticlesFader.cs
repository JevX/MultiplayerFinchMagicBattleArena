using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParticlesFader : MonoBehaviour
{
    public float UIParticlesAlpha = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalFloat(FShaderIDs.g_UIParticles, UIParticlesAlpha);
    }
}
