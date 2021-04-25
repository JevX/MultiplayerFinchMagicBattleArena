using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantmentGlyph : MonoBehaviour
{
    public GameObject ElectroGlyph;
    public GameObject FireGlyph;
    public GameObject IceGlyph;
    public GameObject KineticGlyph;

    MagicType currentType;
    public LineRenderer playerDrawing;

    public SimpleRotation ElectroGlyphSimpleRotation;
    public SimpleRotation FireGlyphSimpleRotation;
    public SimpleRotation IceGlyphSimpleRotation;
    public SimpleRotation KineticGlyphSimpleRotation;

    public ParticleSystem ElectroExplosion;
    public ParticleSystem FireExplosion;
    public ParticleSystem IceExplosion;
    public ParticleSystem KineticExplosion;

    public  Color electroColor;
    public  Color fireColor;
    public  Color iceColor;
    public  Color kineticColor;

    SimpleRotation selectedRotation;

    bool goingToExplode;

    public float maximumRotation;

    public AnimationCurve rotationCurve;
    public float explosionSpeed;

    Coroutine mainCoroutine;

    float currentTime;

    public float DecreaseSpeedMofifier = 2f;

    public float explosionRadius = 0.5f;

    public int damage = 1;
    public float kineticForce = 2f;

    Rigidbody myRb;

    public float speedFollowPlayer = 0.4f;

    [FMODUnity.EventRef]
    public string FMODFireEnchantment;
    [FMODUnity.EventRef]
    public string FMODIceEnchantment;
    [FMODUnity.EventRef]
    public string FMODElectroEnchantment;
    [FMODUnity.EventRef]
    public string FMODKineticEnchantment;

    [FMODUnity.EventRef]
    public string FMODFireLoop;
    [FMODUnity.EventRef]
    public string FMODIceLoop;
    [FMODUnity.EventRef]
    public string FMODElectroLoop;
    [FMODUnity.EventRef]
    public string FMODKineticLoop;

    private FMOD.Studio.EventInstance FMODInstance;

    private void Awake()
    {
        myRb = GetComponent<Rigidbody>();
    }

    public void SetGlyph(MagicType type, Vector3[] image)
    {
        FireGlyph.SetActive(false);
        IceGlyph.SetActive(false);
        ElectroGlyph.SetActive(false);
        KineticGlyph.SetActive(false);

        currentType = type;

        FMODInstance.release();

        switch (currentType)
        {
            case MagicType.None:
                break;

            case MagicType.Electro:
                ElectroGlyph.SetActive(true);
                playerDrawing.startColor = electroColor;
                playerDrawing.endColor = electroColor;
                selectedRotation = ElectroGlyphSimpleRotation;
                FMODInstance = FMODUnity.RuntimeManager.CreateInstance(FMODElectroLoop);
                break;

            case MagicType.Fire:
                FireGlyph.SetActive(true);
                playerDrawing.startColor = fireColor;
                playerDrawing.endColor = fireColor;
                selectedRotation = FireGlyphSimpleRotation;
                FMODInstance = FMODUnity.RuntimeManager.CreateInstance(FMODFireLoop);
                break;

            case MagicType.Ice:
                IceGlyph.SetActive(true);
                playerDrawing.startColor = iceColor;
                playerDrawing.endColor = iceColor;
                selectedRotation = IceGlyphSimpleRotation;
                FMODInstance = FMODUnity.RuntimeManager.CreateInstance(FMODIceLoop);
                break;

            case MagicType.Kinetic:
                KineticGlyph.SetActive(true);
                playerDrawing.startColor = kineticColor;
                playerDrawing.endColor = kineticColor;
                selectedRotation = KineticGlyphSimpleRotation;
                FMODInstance = FMODUnity.RuntimeManager.CreateInstance(FMODKineticLoop);
                break;
        }

        FMODInstance.start();


        playerDrawing.positionCount = image.Length;
        playerDrawing.SetPositions(image);
    }

    private void FixedUpdate()
    {
        FMODInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Wand")
        {
            EnchantWand();
        }

        if (goingToExplode)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, explosionRadius, transform.forward);
            foreach (RaycastHit hit in hits)
            {
                TargetsManager.Instance.DamageRegisteredTarget(damage, currentType, hit.collider.gameObject);

                if (currentType == MagicType.Kinetic && hit.collider.attachedRigidbody != null)
                {
                    hit.collider.attachedRigidbody.AddForce((transform.position - hit.collider.gameObject.transform.position).normalized * kineticForce, ForceMode.Impulse);
                }
            }

            gameObject.SetActive(false);
        }
    }

    private void EnchantWand()
    {
        MagicWand.Instance.ChangeMagicType(currentType);

        gameObject.SetActive(false);
    }

    IEnumerator IncreaseSpeed()
    {
        while (selectedRotation.rotationSpeed < maximumRotation)
        {
            currentTime += Time.deltaTime;
            selectedRotation.rotationSpeed = rotationCurve.Evaluate(currentTime);
            if (selectedRotation.rotationSpeed > explosionSpeed)
            {
                goingToExplode = true;
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator DecreaseSpeed()
    {
        while (selectedRotation.rotationSpeed > 270)
        {
            currentTime -= Time.deltaTime * DecreaseSpeedMofifier;
            selectedRotation.rotationSpeed = rotationCurve.Evaluate(currentTime);
            yield return null;
        }
    }

    IEnumerator Autodisable()
    {
        yield return new WaitForSeconds(10);
        gameObject.SetActive(false);
    }

    IEnumerator ComeToPlayer()
    {
        while (gameObject.activeInHierarchy)
        {
            myRb.MovePosition(transform.position + (MagicWand.Instance.transform.position - transform.position).normalized * Time.deltaTime * speedFollowPlayer);
            yield return null;
        }
        yield return null;
    }

    public void GlyphThrowed()
    {
        if (mainCoroutine != null)
            StopCoroutine(mainCoroutine);

        if (!goingToExplode)
        {
            mainCoroutine = StartCoroutine(DecreaseSpeed());
        }
        else
        {
            myRb.isKinematic = false;
            mainCoroutine = StartCoroutine(Autodisable());
        }

    }

    public void GlyphCarried()
    {
        if (mainCoroutine != null)
            StopCoroutine(mainCoroutine);
        mainCoroutine = StartCoroutine(IncreaseSpeed());
    }

    private void OnEnable()
    {
        if (mainCoroutine != null)
            StopCoroutine(mainCoroutine);
        mainCoroutine = StartCoroutine(ComeToPlayer());
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private void OnDisable()
    {
        goingToExplode = false;
        currentTime = 0;

        if (selectedRotation != null)
            selectedRotation.rotationSpeed = 5;

        if (mainCoroutine != null)
            StopCoroutine(mainCoroutine);

        myRb.isKinematic = true;
        myRb.velocity = Vector3.zero;

        FMODInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        FMODInstance.release();

        switch (currentType)
        {
            case MagicType.Electro:
                Instantiate(ElectroExplosion).transform.position = transform.position;
                FMODUnity.RuntimeManager.PlayOneShot(FMODElectroEnchantment, transform.position);
                //FMODInstance = FMODUnity.RuntimeManager.CreateInstance(FMODElectroEnchantment);
                break;
            case MagicType.Ice:
                Instantiate(IceExplosion).transform.position = transform.position;
                FMODUnity.RuntimeManager.PlayOneShot(FMODIceEnchantment, transform.position);
                //FMODInstance = FMODUnity.RuntimeManager.CreateInstance(FMODIceEnchantment);
                break;
            case MagicType.Fire:
                Instantiate(FireExplosion).transform.position = transform.position;
                FMODUnity.RuntimeManager.PlayOneShot(FMODFireEnchantment, transform.position);
                //FMODInstance = FMODUnity.RuntimeManager.CreateInstance(FMODFireEnchantment);
                break;
            case MagicType.Kinetic:
                Instantiate(KineticExplosion).transform.position = transform.position;
                FMODUnity.RuntimeManager.PlayOneShot(FMODKineticEnchantment, transform.position);
                //FMODInstance = FMODUnity.RuntimeManager.CreateInstance(FMODKineticEnchantment);
                break;
        }

        //FMODInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        //FMODInstance.start();
        //FMODInstance.release();
    }
}
