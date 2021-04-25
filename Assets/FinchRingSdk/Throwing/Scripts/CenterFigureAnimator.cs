using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


[System.Serializable]
public class FigureDescription
{
    public string name = "Undefined";
    public Figure Mesh;
    public Figure Dynamic;
}

[System.Serializable]
public class AnchorPoints
{
    public float Radius;
    public GameObject[] Points;
}

[System.Serializable]
public class CenterFigureAnimation_Reintegration
{
    public AnchorPoints Anchors;

    public float ActivateRange = 1;
    public float ActivateVar = 0.25f;

    public float VelocityMin = 5;
    public float VelocityMax = 10;

    public float LifeTimeMin = 2;
    public float LifeTimeMax = 3;

    public float animation_time = 5;
}



public enum CenterFigureAnimation
{
    None,

    Dithentegration,
    Reintegration,
    Morphing
};


public class CenterFigureAnimator : MonoBehaviour
{
    public FigureDescription[] FigureDescs;

    [HideInInspector]
    public GameObject currentSpawnedObject = null;

    public Transform podium;
    public Material material;
    public GameObject trowableObject;
    public Transform top;
    public Transform bottom;
    public static Action podiumHere;
    [Header("Animation")]
    public Animator hologramm;
    public CenterFigureAnimation_Reintegration Reintegration;
    public float curlnoise_intecity = 1;
    public float curlnoise_scale = 1;
    public AnimationCurve scale_curve;
    public static bool ReitaergrationAnimationPlay = false;
    //private GameObject controller;
    //private ObjectPickup _controller;
    private CenterFigureAnimation m_currentAniamation = CenterFigureAnimation.None;
    private float m_verticalInfluence = 0;
    private bool anim_allowed = true;
    private bool reint_allowed = true;
    private float timer_alpha_chanel = 0;
    private int m_current_figure = 0;
    private bool firstStart = true;
    // private int m_target_figure = 1;
    private bool first_start_game = false;
    private float m_animTime = 1;
    public bool bIn_VR;
    private GameObject managerPyramid;
    public bool bNot_AR_kit = false;

    void Start()
    {

        managerPyramid = GameObject.Find("ManagerPiramydRigidbodies");
        hologramm.transform.gameObject.SetActive(false);
        ResetDynamic();
        hologramm.GetComponent<Animator>();
        GameObject swipePodiumInARcore = GameObject.Find("swipePodium");//что бы анимация запустилась после калибровки, иначе она не запуститься в AR
        //print(first_start_game);

        //когда создали подиум и калибровка уже прошла или мы в ВР проекте
        //if (swipePodiumInARcore != null && swipePodiumInARcore.GetComponent<SwipePodium>().bColibrationFinish == true || bIn_VR)
        //{
        //    if (bNot_AR_kit)
        //    {
        //        Finch.FinchCalibration.OnCalibrationEnd += First_Start_Game;
        //        Debug.Log("we are start");
        //       // RunAnimation_Reintegration();
        //    }
        //}
        //if (!bNot_AR_kit)
        //{
        //    podiumHere += First_Start_Game;
        //}
        bool ar_core = swipePodiumInARcore != null;
        bool ar_kit = !bNot_AR_kit;
        bool vr = bIn_VR;

        //if (ar_kit)
        //{
        //    podiumHere += First_Start_Game;
        //}

        if (ar_core || ar_kit)
        {
            podiumHere?.Invoke();
            RunAnimation_Reintegration();
        }
        if (vr)
        {
            Finch.FinchCalibration.OnCalibrationEnd += First_Start_Game;
        }
    }



    public void First_Start_Game()
    {
        //podiumHere?.Invoke(); //create action when podium is create

        if (first_start_game)
            return;
        first_start_game = true;
        Finch.FinchCalibration.OnCalibrationEnd -= First_Start_Game;
        gameObject.transform.parent.gameObject.SetActive(true);
        if (currentSpawnedObject == null)
            RunAnimation_Reintegration();
        // Debug.Log("we are first start game");
        // hologramm.Play("hologramIn");
    }

    [ContextMenu("RunAnimation_Reintegration")]
    public void RunAnimation_Reintegration()
    {
        hologramm.transform.gameObject.SetActive(true); //сделано что бы на первый запуск реинтеграции включить голограмму
        ShowHologram();
        timer_alpha_chanel = 0;
        StartCoroutine("Reintegration_anim_timer");
        //  material.SetFloat("g_fAlphaReintegrationEnabled", 1);
        RunAnimation(CenterFigureAnimation.Reintegration);
        AudioMaster.Play(AudioMaster.Instance.reintegration);
    }

    public void HideHologram()
    {
        hologramm.Play("hologramOut");
    }

    public void ShowHologram()
    {
        hologramm.Play("hologramIn");
    }

    void ResetDynamic()
    {
        for (int i = 0; i < FigureDescs.Length; ++i)
        {
            FigureDescription desc = FigureDescs[i];
            if (desc.Mesh == null)
                continue;
            desc.Dynamic.InitilizeDynamic(desc.Mesh);
        }
    }




    float linstep(float v, float a, float b)
    {
        return (v - a) / (b - a);
    }

    void Update()
    {


        float T = top.position.y;
        float B = bottom.position.y;
        Vector3 C = FigureDescs[m_current_figure].Mesh.transform.position;
        material.SetFloat("g_fAabbMin", B);
        material.SetFloat("g_fAabbMax", T);
        material.SetVector("g_vOrigin", C);

        if (Input.GetKeyDown(KeyCode.R))
        {
            RunAnimation_Reintegration();
            anim_allowed = false;
        }


        FigureDescription fdesc = FigureDescs[m_current_figure];
        fdesc.Mesh.transform.position = new Vector3(podium.position.x, fdesc.Mesh.transform.position.y, podium.position.z);//всегда следовать за подиумом


        float dt = Time.deltaTime;


        if (m_currentAniamation == CenterFigureAnimation.Reintegration)
        {
            AlphaSnader();
            // print("figure " + figure + " has reint");
            FigureDescription desc = FigureDescs[m_current_figure];
            for (int i = 0; i < desc.Dynamic.RuntimeParticles.Length; ++i)
            {
                // int anchorRandom = Random.Range(0, Reintegration.Anchors.Points.Length);
                FigureParticle particle = desc.Dynamic.RuntimeParticles[i];

                if (particle.m_dead)
                    continue;

                if (!particle.m_transformDynamic.gameObject.activeInHierarchy)
                {
                    // lifetime  to activation
                    particle.m_lifetime += dt;

                    float activate = Reintegration.ActivateRange + particle.m_activate_seed * Reintegration.ActivateVar;


                    float rel_active = particle.m_lifetime / activate;

                    if (rel_active >= particle.m_vert_grad)
                    {
                        particle.m_lifetime = 0;

                        particle.m_transformDynamic.gameObject.SetActive(true);
                    }
                }
                else
                {
                    // lifetime - real life time
                    particle.m_lifetime += dt;

                    float gain = particle.m_lifetime / 1.0f;
                    particle.m_velocity = Vector3.Lerp(Vector3.zero, particle.m_velocityGain, gain);

                    Vector3 target_velocity = particle.m_transformStatic.position - particle.m_transformDynamic.position;
                    float distance = target_velocity.magnitude;

                    target_velocity.Normalize();

                    float vel_ampl = Reintegration.VelocityMin + particle.m_velocity_seed * (Reintegration.VelocityMax - Reintegration.VelocityMin);

                    target_velocity *= vel_ampl;
                    target_velocity *= Mathf.Pow(Mathf.Clamp01(linstep(distance, 0, 1)), 1.25f);

                    float vel_target_alpha = particle.m_lifetime - 0.5f;
                    vel_target_alpha = Mathf.Clamp(vel_target_alpha, 0, 1);
                    particle.m_velocity = Vector3.Lerp(particle.m_velocity, target_velocity, vel_target_alpha);

                    float max_life = Reintegration.LifeTimeMin + particle.m_lifetime_seed * (Reintegration.LifeTimeMax - Reintegration.LifeTimeMin);
                    float lifetime01 = particle.m_lifetime / max_life;




                    if (particle.m_lifetime > max_life)
                    {
                        particle.m_dead = true;
                        particle.m_transformDynamic.gameObject.SetActive(false);
                        particle.m_transformStatic.gameObject.SetActive(true);
                        //print(particle.m_transformStatic.gameObject.activeInHierarchy);
                    }


                    float alpha_final_align_pos = Mathf.Clamp01(linstep(lifetime01, 0.85f, 1.0f));
                    float alpha_final_align_rot = Mathf.Clamp01(linstep(lifetime01, 0.0f, 1.0f));
                    float alpha_final_align_scl = Mathf.Clamp01(lifetime01);
                    alpha_final_align_scl = scale_curve.Evaluate(alpha_final_align_scl);


                    particle.m_virtual_pos += particle.m_velocity * dt;
                    particle.m_transformDynamic.rotation = Quaternion.Lerp(particle.m_anchor_rot0, particle.m_transformStatic.rotation, alpha_final_align_rot);
                    particle.m_transformDynamic.localScale = Vector3.Lerp(particle.m_start_scale, particle.m_transformStatic.localScale, alpha_final_align_scl);
                    particle.m_transformDynamic.position = Vector3.Lerp(particle.m_virtual_pos, particle.m_transformStatic.position, alpha_final_align_pos);
                }
            }
        }
    }

    void AlphaSnader()
    {

        timer_alpha_chanel += Time.deltaTime;
        float c = Mathf.Clamp01(timer_alpha_chanel / Reintegration.animation_time);
        // print(c);
        material.SetFloat("g_fDissolveAlpha", c);

    }


    IEnumerator Reintegration_anim_timer()
    {
        ReitaergrationAnimationPlay = true;
        yield return new WaitForSeconds(Reintegration.animation_time);
        ReitaergrationAnimationPlay = false;
        material.SetFloat("g_fAlphaReintegrationEnabled", 0);
        // timer_alpha_chanel = 0;
        anim_allowed = true;
        reint_allowed = false;

        currentSpawnedObject = Instantiate(trowableObject, FigureDescs[0].Mesh.transform.position, FigureDescs[0].Mesh.transform.rotation);
        currentSpawnedObject.transform.parent = FigureDescs[0].Mesh.transform;
        currentSpawnedObject.transform.localScale = FigureDescs[0].Mesh.transform.localScale;
        RunAnimation(CenterFigureAnimation.None);
    }


    void RunAnimation(CenterFigureAnimation animation)
    {
        m_currentAniamation = animation;

        m_verticalInfluence = 0;

        ResetDynamic();

        m_animTime = 0;


        if (m_currentAniamation == CenterFigureAnimation.Reintegration)
        {
            FigureDescription desc = FigureDescs[m_current_figure];
            for (int i = 0; i < desc.Dynamic.RuntimeParticles.Length; ++i)
            {
                FigureParticle particle = desc.Dynamic.RuntimeParticles[i];

                particle.m_transformStatic.gameObject.SetActive(false);
                particle.m_transformDynamic.gameObject.SetActive(false);

                particle.m_start_scale = new Vector3(Random.Range(0.5f, 0.7f), Random.Range(0.5f, 0.7f), Random.Range(0.5f, 0.7f));//рандом скейла

                particle.m_anchor_target = Random.Range(0, Reintegration.Anchors.Points.Length);


                Transform target_point = Reintegration.Anchors.Points[particle.m_anchor_target].transform;


                Vector3 var_vector = Reintegration.Anchors.Radius * Random.insideUnitSphere;


                particle.m_anchor_target_pos = target_point.position + var_vector;

                particle.m_anchor_rot0 = Quaternion.Euler(Random.value * 360, Random.value * 360, Random.value * 360);

                particle.m_transformDynamic.rotation = particle.m_anchor_rot0;

                particle.m_transformDynamic.position = particle.m_anchor_target_pos;
                particle.m_virtual_pos = particle.m_anchor_target_pos;

                Vector3 forward = particle.m_transformStatic.position - particle.m_anchor_target_pos;
                forward.Normalize();

                Vector3 right = Vector3.Cross(forward, Vector3.up);
                right = Quaternion.Euler(Random.value * 30, 0, 0) * Quaternion.Euler(0, 0, Random.value * 360) * right;
                right *= Mathf.Lerp(0.25f, 1.0f, Random.value);

                right *= Reintegration.VelocityMax;

                particle.m_velocityGain = right;
            }
        }

        if (m_currentAniamation == CenterFigureAnimation.None)
        {
            FigureDescription desc = FigureDescs[m_current_figure];
            for (int i = 0; i < desc.Dynamic.RuntimeParticles.Length; ++i)
            {
                FigureParticle particle = desc.Dynamic.RuntimeParticles[i];
                particle.m_transformStatic.gameObject.SetActive(false);
                particle.m_transformDynamic.gameObject.SetActive(false);
            }
        }
    }

    float noise(float x, float y, float z)
    {
        return Perlin.Noise(x * curlnoise_scale, y * curlnoise_scale, z * curlnoise_scale);
    }

    Vector3 ComputeCurl(float x, float y, float z)
    {
        float eps = 1.0f;
        float n1, n2, a, b;

        Vector3 curl = new Vector3();

        n1 = noise(x, y + eps, z);
        n2 = noise(x, y - eps, z);
        a = (n1 - n2) / (2 * eps);

        n1 = noise(x, y, z + eps);
        n2 = noise(x, y, z - eps);
        b = (n1 - n2) / (2 * eps);

        curl.x = a - b;

        n1 = noise(x, y, z + eps);
        n2 = noise(x, y, z - eps);
        a = (n1 - n2) / (2 * eps);

        n1 = noise(x + eps, y, z);
        n2 = noise(x + eps, y, z);
        b = (n1 - n2) / (2 * eps);

        curl.y = a - b;
        n1 = noise(x + eps, y, z);
        n2 = noise(x - eps, y, z);
        a = (n1 - n2) / (2 * eps);

        n1 = noise(x, y + eps, z);
        n2 = noise(x, y - eps, z);
        b = (n1 - n2) / (2 * eps);

        curl.z = a - b;

        return curl;
    }


    private void OnDrawGizmos()
    {
        var anchors = Reintegration.Anchors;
    }
}



