using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AllyType { Base, Speed, Rapid, Magnify, Shield, Reflector, Copycat, Score, Bomb, Parasite, Life, None }
public enum AttachType { None, Player, Enemy }

public class BaseAllyScript : MonoBehaviour
{
    /* COMPONENTS */
    [HideInInspector] public Rigidbody2D rb2D;
    [HideInInspector] public AudioSource soundPlayer;
    [HideInInspector] public Animator allyAnim;

    /* PRIVATE VARIABLES */
    [HideInInspector] public bool isAttached = false;
    [SerializeField] private AllyType powerupType = AllyType.Base; // Can be edited in the inspector
    [HideInInspector] public AttachType attachedTo = AttachType.None;

    /* ALLY SHIP VARIABLES */
    public bool spawnInFreefall = false;
    public float baseDespawnTimer = 10.0f;

    /* DRAG AND DROP */
    public AllySpriteList allySprites;
    public AllySoundList allySounds;
    public SpriteRenderer spriteRender;
    public GameObject explosionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        soundPlayer = this.gameObject.GetComponent<AudioSource>();
        allyAnim = this.gameObject.GetComponent<Animator>();
        if (spawnInFreefall) { SpawnInFreefall(); }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Accessor method for isAttached
    public bool getIsAttached()
    {
        return isAttached;
    }

    // Accessor method for powerupType
    public AllyType getPowerupType()
    {
        return powerupType;
    }

    // Setter method for powerupType
    public void setPowerupType(AllyType type)
    {
        powerupType = type;

        switch (type)
        {
            case AllyType.Speed:
                spriteRender.sprite = allySprites.spriteList[1];
                break;
            case AllyType.Rapid:
                spriteRender.sprite = allySprites.spriteList[2];
                break;
            case AllyType.Magnify:
                spriteRender.sprite = allySprites.spriteList[3];
                break;
            case AllyType.Score:
                spriteRender.sprite = allySprites.spriteList[8];
                break;
            case AllyType.Shield:
                spriteRender.sprite = allySprites.spriteList[4];
                break;
            case AllyType.Reflector:
                spriteRender.sprite = allySprites.spriteList[6];
                break;
            case AllyType.Copycat:
                spriteRender.sprite = allySprites.spriteList[7];
                break;
            case AllyType.Bomb:
                spriteRender.sprite = allySprites.spriteList[9];
                break;
            case AllyType.Parasite:
                spriteRender.sprite = allySprites.spriteList[10];
                break;
            case AllyType.Life:
                spriteRender.sprite = allySprites.spriteList[11];
                break;
            default:
                spriteRender.sprite = allySprites.spriteList[0];
                break;
        }
    }

    // Spawns the ally ship in a freefall state
    public void SpawnInFreefall()
    {
        DetachFromShip();
    }

    // Attaches ally to player ship
    public virtual void AttachToPlayer(Transform slot)
    {
        if (isAttached) { return; }

        PlaySoundEffect(allySounds.soundEffects[0]);

        if (!allyAnim.GetCurrentAnimatorStateInfo(0).IsName("StaticFreefall"))
        {
            allyAnim.Play("StaticFreefall");
        }

        attachHelper(slot);

        attachedTo = AttachType.Player;
    }

    // Attaches ally to enemy ship
    public void AttachToEnemy(Transform slot)
    {
        if (isAttached) { return; }

        if (rb2D == null)
        {
            rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        }

        attachHelper(slot);

        attachedTo = AttachType.Enemy;
    }

    // Helper function for base attachment code
    protected void attachHelper(Transform slot)
    {
        rb2D.isKinematic = true;
        rb2D.constraints = RigidbodyConstraints2D.FreezeAll;

        this.transform.parent = slot;
        this.transform.position = slot.position;
        this.transform.rotation = slot.rotation;

        isAttached = true;
    }

    // Detaches ally from its parent
    public virtual void DetachFromShip(float ejectSpeed = 0.0f, bool isFromDamage = false)
    {
        Vector2 ejectDirection;

        if (this.transform.parent != null)
        {
            Vector3 dirVec = (this.transform.position - this.transform.parent.parent.position);
            ejectDirection = new Vector2(dirVec.x, dirVec.y);
        }
        else
        {
            ejectDirection = Vector2.zero;
        }

        detachHelper();

        if (ejectSpeed != 0.0f)
        {
            rb2D.AddForce(ejectDirection * ejectSpeed, ForceMode2D.Impulse);
        }

        StartCoroutine(FreefallTimer(baseDespawnTimer));
    }

    // Helper function for base detachment code
    protected void detachHelper()
    {
        this.transform.parent = null;
        rb2D.isKinematic = false;
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        isAttached = false;
        attachedTo = AttachType.None;
    }

    // Helper function used to keep track of despawn time
    public IEnumerator FreefallTimer(float seconds)
    {
        if (isAttached) { yield break; }

        float degreesPerSecond = 360.0f / seconds;
        float currDegrees = 0.0f;

        while (currDegrees >= -360.0f && !isAttached)
        {
            if (currDegrees <= -180.0f && !allyAnim.GetCurrentAnimatorStateInfo(0).IsName("ExpiringFreefall"))
            {
                allyAnim.Play("ExpiringFreefall");
            }
            currDegrees -= (degreesPerSecond * Time.deltaTime);
            this.transform.rotation = Quaternion.AngleAxis(currDegrees, Vector3.forward);
            yield return new WaitForSeconds(0.0f);
        }

        if (!isAttached) { GameObject.Destroy(this.gameObject); }
    }

    // Call this to destroy the ally ship
    public virtual void DestroyAllyShip(bool isProjectile = false)
    {
        Explode();
        GameObject.Destroy(this.gameObject);
    }

    public void Explode()
    {
        GameObject objTemp = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        ExplodeSoundScript explosionScript = objTemp.GetComponent<ExplodeSoundScript>();
        explosionScript.DoExplosion(2, 0.5f);
    }

    // Plays sound effects
    public void PlaySoundEffect(AudioClip theClip)
    {
        soundPlayer.clip = theClip;
        soundPlayer.Play();
    }
}
