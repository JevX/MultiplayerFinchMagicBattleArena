using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureParticle
{
    public Transform m_transformStatic;
    public Transform m_transformDynamic;

    public float m_activate_seed;
    public float m_velocity_seed;
    public float m_lifetime_seed;



    /////////////////////////////////////////
    // Index of target anchor
    public int m_anchor_target;
    public Vector3 m_anchor_target_pos;
    public Vector3 m_anchor_morphing;
    public Vector3 m_velocity;

    public Vector3 m_virtual_pos;
    public Vector3 m_start_scale;
    public Quaternion m_anchor_rot0;



    public float m_lifetime;
    public float m_vert_grad;
    public bool m_dead;

    public Vector3 m_velocityGain;

    public FigureParticle(Transform transform, Transform transform_static, float height_min, float height_max)
    {
        transform.localPosition = transform_static.localPosition;
        transform.localRotation = transform_static.localRotation;
        transform.localScale = transform_static.localScale;

        transform_static.gameObject.SetActive(false);
        transform.gameObject.SetActive(false);

        m_lifetime = 0;
        m_dead = false;
        // m_start_scale = new Vector3(0.5f, 0.5f, 0.5f);

        m_velocity = Vector3.zero;
        m_anchor_target = -1;
        m_anchor_target_pos = Vector3.zero;
        m_anchor_morphing = Vector3.zero;

        m_transformStatic = transform_static;
        m_transformDynamic = transform;

        m_activate_seed = Random.value;
        m_velocity_seed = Random.value;
        m_lifetime_seed = Random.value;


        float p_cur = transform_static.localPosition.y;
        float vert_grad = (p_cur - height_min)
             / (height_max - height_min);
        m_vert_grad = Mathf.Clamp01(vert_grad);
    }
}

public class Figure : MonoBehaviour
{
    public GameObject[] Particles;
    public float HeightMax = 1;
    public float HeightMin = -1;

    //////////////////////////////////////////////
    /// Dynamic
    [HideInInspector]
    [System.NonSerialized]
    public FigureParticle[] RuntimeParticles;


    public void InitilizeDynamic(Figure mesh_static)
    {
        RuntimeParticles = new FigureParticle[Particles.Length];
        for (int i = 0; i < Particles.Length; ++i)
        {
            RuntimeParticles[i] = new FigureParticle(
                Particles[i].transform,
                mesh_static.Particles[i].transform,
                mesh_static.HeightMin,
                mesh_static.HeightMax
            );
        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnDrawGizmosSelected()
    {
        Vector3 p = transform.position;

        Vector3 l0 = new Vector3(0, HeightMax, 0);
        Vector3 l1 = new Vector3(0, HeightMin, 0);

        l0 = transform.TransformPoint(l0);
        l1 = transform.TransformPoint(l1);

        var color = new Color(Random.Range(1, 250), Random.Range(1, 250), Random.Range(1, 250));
        Debug.DrawLine(l0, l1, color);
    }
}
