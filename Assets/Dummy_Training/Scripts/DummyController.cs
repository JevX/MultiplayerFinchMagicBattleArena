using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour
{
    public int health;
    private bool action=true;
    public bool Broken=true;
    public GameObject textHealth;
    public GameObject DummyFull;
    public GameObject DummyBroken;
    public ParticleSystem Hit_FX;
    public AudioClip audioHit1;
    public AudioClip audioHit2;
    public AudioClip audioHit3;
    public AudioClip audioBroken;
    private Animator mAnimator;
    private AudioSource asource;
    private int rand;
    // Start is called before the first frame update
    void Start()
    {
        mAnimator = DummyFull.GetComponent<Animator>();
        TextMesh txt = textHealth.GetComponent<TextMesh>();
        txt.text = health.ToString();
        asource =GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (action) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 10000))
            {
                if (hit.transform.tag == "enemy")
                {
                        //Random Animations Hit Play
                        
                             rand = Random.Range(1, 3);
                            if (rand == 1)
                            {
                            mAnimator.Play("Dummy_Hit1");
                            }
                            else if (rand == 2)
                            {
                            mAnimator.Play("Dummy_Hit2");
                            }
                          

                            
                        
                        //Particle Hit FX Play
                        Hit_FX.Play();


                    //Random Sound Hit Play
                    if (health > 10)
                    {
                        
                        rand = Random.Range(1, 4);
                        if (rand == 1)
                        {
                            asource.PlayOneShot(audioHit1);
                        }
                        else if (rand == 2)
                        {
                            asource.PlayOneShot(audioHit2);
                        }
                        else
                        {
                            asource.PlayOneShot(audioHit3);
                        }
                     }
                        TextMesh txt = textHealth.GetComponent<TextMesh>();
                    health -= 10;
                    txt.text = health.ToString();


                    //Destroy Dummy;
                    if (health == 0)
                    {
                        if (Broken)
                        {
                            DummyFull.SetActive(false);
                                asource.PlayOneShot(audioBroken);
                         
                                DummyBroken.SetActive(true);
                                textHealth.SetActive(false);
                                action = false;
                        }
                        else
                        {
                            mAnimator.Play("Dummy_Death");
                                asource.PlayOneShot(audioHit1);
                                textHealth.SetActive(false);
                                action = false;
                        }
                    }
                }
            }
        }
        }
    }
}
