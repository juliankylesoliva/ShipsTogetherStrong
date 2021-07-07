using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainShipMovement : MonoBehaviour
{
    /* COMPONENTS */
    private Camera mainCam;
    private Rigidbody2D rb2D;
    private AudioSource soundPlayer;
    public SpriteRenderer shipSprite;

    /* ENABLES */
    public bool enableMouseMovement = true;
    public bool enableShipMovement = true;
    public bool enableShooting = true;
    public bool enableEjectMode = true;
    public bool enableDrifting = true;

    /* SHIP VARIABLES */
    public float baseShipSpeed = 1.0f;
    public float baseMaxShipVelocity = 1.0f;
    public float recoilSpeedPenalty = 0.8f;
    public float baseFiringDelay = 0.5f;
    public float manualEjectSpeed = 2.0f;
    public float damageEjectSpeed = 5.0f;
    public float knockbackTime = 1.0f;
    public float respawnTime = 5.0f;
    public float postDamageCooldownTime = 3.0f;
    public float driftingSpeedPenalty = 0.9f;
    public float minDriftTime = 1.0f;
    public float maxDriftTime = 3.0f;
    public float driftCooldown = 1.0f;

    public int startingLives = 3;
    public int maxLives = 10;
    public int killstreakBonusInterval = 10;
    public int maxKillstreak = 100;
    public int baseKillstreakBonus = 1000;

    /* POWER-UP MODIFIERS */
    private float modShipSpeed = 1.0f;
    private float modMaxShipVelocity = 1.0f;
    private float modSpeedPenalty = 1.0f;
    private float modFiringDelay = 1.0f;
    private float modProjectileSize = 1.0f;
    private float modProjectileRecoil = 0.0f;
    private int currentScoreMultiplier = 1;

    /* PRIVATE SHIP VARIABLES */
    private bool isFiringDelayed = false;
    private bool isEjectModeOn = false;
    private bool isDamaged = false;
    private bool isDamageproof = false;
    private bool isDrifting = false;
    private int totalEnemiesDestroyed = 0;
    private int enemyKillstreak = 0;
    private int livesLeft;
    private AllyType[] ejectionList;

    /* PREFABS AND OTHER DRAG AND DROPS */
    public Transform cannon;
    public GameObject projectile;
    public Scorekeeper scoringSystem;
    public Transform[] formationSlots;
    public AudioClip[] soundEffects;
    public GameObject explosionPrefab;
    public GameObject driftPivotPrefab;
    public Transform driftPivotPosition;
    public Animator spriteAnim;
    public Animator hurtFXAnim;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        mainCam = Camera.main;
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        soundPlayer = this.gameObject.GetComponent<AudioSource>();
        ejectionList = new AllyType[4];
        livesLeft = startingLives;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableDrifting) { GravityDrift(); }
        if (enableMouseMovement) { PointToMouse(); }
        if (enableShipMovement) { MoveShip(); }
        if (enableShooting) { FireProjectile(); }
        if (enableEjectMode) { EjectModeHandler(); }
        CheckGameOver();
        CheckPowerups();
        CheckQuit();
    }
    
    // Ship sprite points itself to the mouse cursor's position.
    void PointToMouse()
    {
        if (!isDamaged && !isDrifting)
        {
            Vector3 dirVec = Input.mousePosition - mainCam.WorldToScreenPoint(this.transform.position);
            float angle = (Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg) - 90.0f;
            this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    // Moves the ship in the direction it is currently facing.
    void MoveShip()
    {
        if (!isDamaged && !isDrifting && Input.GetKey(KeyCode.Space))
        {
            if (rb2D.velocity.magnitude < (baseMaxShipVelocity * modMaxShipVelocity))
            {
                rb2D.AddForce(this.transform.up * baseShipSpeed * modShipSpeed);
            }
        }
    }

    // The ship will drift around a distant point
    void GravityDrift()
    {
        if (!isDrifting && !isDamaged && Input.GetMouseButtonDown(1))
        {
            StartCoroutine(DoDrift());
        }
    }

    // Drift helper function
    IEnumerator DoDrift()
    {
        Vector3 initialPos = driftPivotPosition.position;
        float driftTime = 0.0f;
        isDrifting = true;
        shipSprite.color = Color.blue;

        GameObject pivot = Instantiate(driftPivotPrefab, initialPos, Quaternion.identity);

        while (!isDamaged && Input.GetMouseButton(1))
        {
            Vector3 dirVec = initialPos - this.transform.position;
            float angle = (Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg) - 90.0f;
            this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            if (rb2D.velocity.magnitude < (baseMaxShipVelocity * modMaxShipVelocity * driftingSpeedPenalty))
            {
                rb2D.AddForce(this.transform.up * baseShipSpeed * modShipSpeed * driftingSpeedPenalty);
            }

            driftTime += Time.deltaTime;
            if (driftTime > maxDriftTime)
            {
                driftTime = maxDriftTime;
            }
            yield return new WaitForSeconds(0.0f);
        }

        GameObject.Destroy(pivot);

        if (!isDamaged && Input.GetMouseButtonUp(1))
        {
            if (driftTime >= minDriftTime)
            {
                float angle = (Mathf.Atan2(rb2D.velocity.y, rb2D.velocity.x) * Mathf.Rad2Deg) - 90.0f;
                this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                rb2D.AddForce(rb2D.velocity * driftTime * 0.75f, ForceMode2D.Impulse);
            }
        }

        yield return new WaitForSeconds(driftCooldown);

        isDrifting = false;
        shipSprite.color = Color.white;
        yield break;
    }

    // Fires a projectile in the direction the ship is currently facing.
    void FireProjectile()
    {
        if (!isDamaged && !isFiringDelayed && !isDrifting && Input.GetMouseButton(0))
        {
            PlaySoundEffect(soundEffects[0]);
            GameObject tempObj = Instantiate(projectile, cannon);
            tempObj.transform.localScale *= modProjectileSize;
            if (rb2D.velocity.magnitude < (baseMaxShipVelocity * modMaxShipVelocity * recoilSpeedPenalty))
            {
                rb2D.AddForce(-this.transform.up * modProjectileRecoil, ForceMode2D.Impulse);
            }
            StartCoroutine(DoFiringDelay());
        }
    }

    // Helper function for making a firing delay
    IEnumerator DoFiringDelay()
    {
        isFiringDelayed = true;
        yield return new WaitForSeconds(baseFiringDelay * modFiringDelay);
        isFiringDelayed = false;
    }

    // Handle collision cases here
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isDamaged && col.transform.tag == "Ally")
        {
            int openSlot = findOpenAllySlot();

            if (openSlot != -1)
            {
                BaseAllyScript ally = col.gameObject.GetComponent<BaseAllyScript>();
                Transform chosenSlot = formationSlots[openSlot];

                if (!ally.getIsAttached())
                {
                    ally.AttachToPlayer(chosenSlot);
                }
            }
        }
    }

    // Helper function -- finds the first available ally slot
    int findOpenAllySlot()
    {
        for (int i = 0; i < formationSlots.Length; ++i)
        {
            if (formationSlots[i].childCount == 0)
            {
                return i;
            }
        }
        return -1;
    }

    // Function for toggling Eject Mode
    void EjectModeHandler()
    {
        if (isDamaged) { return; }

        if (Input.GetKeyDown(KeyCode.E) && !isEjectModeOn)
        {
            isEjectModeOn = true;
        }

        if (!enableEjectMode || (Input.GetKeyDown(KeyCode.Q) && isEjectModeOn))
        {
            isEjectModeOn = false;
        }

        if (isEjectModeOn)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (isSlotOccupied(0))
                {
                    BaseAllyScript ally = formationSlots[0].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                    ally.DetachFromShip(manualEjectSpeed);
                    PlaySoundEffect(soundEffects[1]);
                }
                isEjectModeOn = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (isSlotOccupied(1))
                {
                    BaseAllyScript ally = formationSlots[1].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                    ally.DetachFromShip(manualEjectSpeed);
                    PlaySoundEffect(soundEffects[1]);
                }
                isEjectModeOn = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (isSlotOccupied(2))
                {
                    BaseAllyScript ally = formationSlots[2].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                    ally.DetachFromShip(manualEjectSpeed);
                    PlaySoundEffect(soundEffects[1]);
                }
                isEjectModeOn = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (isSlotOccupied(3))
                {
                    BaseAllyScript ally = formationSlots[3].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                    ally.DetachFromShip(manualEjectSpeed);
                    PlaySoundEffect(soundEffects[1]);
                }
                isEjectModeOn = false;
            }
            else { }
        }
    }

    // Helper function -- checks the specified ally slot if a ship is occupied there
    bool isSlotOccupied(int slot)
    {
        return formationSlots[slot].childCount == 1;
    }

    // Updates the ship's stats based on the types of ally ships collected
    void CheckPowerups()
    {
        int speedCount = 0;
        int shieldCount = 0;
        int rapidCount = 0;
        int magnifyCount = 0;
        int scoreCount = 0;

        for (int i = 0; i < formationSlots.Length; ++i)
        {
            if (formationSlots[i].childCount == 1)
            {
                BaseAllyScript ally = formationSlots[i].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                AllyType type = ally.getPowerupType();

                ejectionList[i] = type;

                switch (type)
                {
                    case AllyType.Speed:
                        ++speedCount;
                        break;
                    case AllyType.Shield:
                        ++shieldCount;
                        break;
                    case AllyType.Rapid:
                        ++rapidCount;
                        break;
                    case AllyType.Magnify:
                        ++magnifyCount;
                        break;
                    case AllyType.Score:
                        ++scoreCount;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                ejectionList[i] = AllyType.None;
            }
        }

        modShipSpeed = (1.0f + (speedCount * 0.25f));
        modMaxShipVelocity = (1.0f + (speedCount * 0.2f));
        modSpeedPenalty = (1.0f - (shieldCount * 0.125f));
        modFiringDelay = (1.0f - (rapidCount * 0.125f));
        modProjectileSize = (1.0f + (magnifyCount * 0.75f));
        modProjectileRecoil = (magnifyCount * 2.0f);
        currentScoreMultiplier = (1 + (scoreCount * 1));
    }

    // Restart on Game Over
    void CheckGameOver()
    {
        if (livesLeft == 0 && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    void CheckQuit()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // Helper function -- counts how many allies are attached
    public int getAllyCount()
    {
        int retVal = 0;

        for (int i = 0; i < formationSlots.Length; ++i)
        {
            if (formationSlots[i].childCount == 1)
            {
                ++retVal;
            }
        }

        return retVal;
    }

    // If the player gets hit while holding ships, all ships get ejected. Otherwise, a life is lost
    public void TakeDamage()
    {
        StartCoroutine(DamageCoroutine());
    }

    private IEnumerator DamageCoroutine()
    {
        if (isDamageproof || isDamaged) { yield break; }

        if (getAllyCount() > 0)
        {
            PlaySoundEffect(soundEffects[2]);
            isDamaged = true;
            //shipSprite.color = Color.yellow;
            spriteAnim.Play("KnockbackShip");

            CutKillstreak();

            if (isSlotOccupied(0))
            {
                BaseAllyScript ally = formationSlots[0].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                ally.DetachFromShip(damageEjectSpeed, true);
            }

            if (isSlotOccupied(1))
            {
                BaseAllyScript ally = formationSlots[1].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                ally.DetachFromShip(damageEjectSpeed, true);
            }

            if (isSlotOccupied(2))
            {
                BaseAllyScript ally = formationSlots[2].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                ally.DetachFromShip(damageEjectSpeed, true);
            }

            if (isSlotOccupied(3))
            {
                BaseAllyScript ally = formationSlots[3].GetChild(0).gameObject.GetComponent<BaseAllyScript>();
                ally.DetachFromShip(damageEjectSpeed, true);
            }

            hurtFXAnim.Play("UI_Hurt_Short");
            yield return new WaitForSeconds(knockbackTime);

            isDamaged = false;
            //shipSprite.color = Color.white;
            spriteAnim.Play("StaticMainShip");

            StartCoroutine(DamageCooldown());
        }
        else
        {
            PlaySoundEffect(soundEffects[3]);
            decrementLivesLeft();
            //shipSprite.color = Color.red;
            spriteAnim.Play("DamagedShip");

            ResetKillstreak();

            rb2D.velocity = Vector3.zero;

            isDamaged = true;
            Explode();

            if (livesLeft > 0)
            {
                hurtFXAnim.Play("UI_Hurt_Long");
                yield return new WaitForSeconds(respawnTime);

                isDamaged = false;
                //shipSprite.color = Color.white;
                spriteAnim.Play("StaticMainShip");

                StartCoroutine(DamageCooldown());
            }
            else
            {
                // Game Over
                hurtFXAnim.Play("UI_Hurt_GameOver");
            }
        }
    }

    // Post-damage invulnerability
    IEnumerator DamageCooldown()
    {
        isDamageproof = true;
        //shipSprite.color = Color.blue;
        spriteAnim.Play("DamageCooldownShip");
        yield return new WaitForSeconds(postDamageCooldownTime);
        isDamageproof = false;
        //shipSprite.color = Color.white;
        spriteAnim.Play("StaticMainShip");
    }

    private void CheckKillstreak()
    {
        ++enemyKillstreak;
        if (enemyKillstreak > 0 && (enemyKillstreak % killstreakBonusInterval) == 0)
        {
            if (enemyKillstreak > maxKillstreak)
            {
                enemyKillstreak = maxKillstreak;
                scoringSystem.AddToScore(baseKillstreakBonus * (enemyKillstreak / killstreakBonusInterval), currentScoreMultiplier, $"{enemyKillstreak}+ Streak!");
            }
            else
            {
                scoringSystem.AddToScore(baseKillstreakBonus * (enemyKillstreak / killstreakBonusInterval), currentScoreMultiplier, $"{enemyKillstreak} Streak!");
            }
        }
    }

    private void CutKillstreak(int factor = 2)
    {
        if (factor == 0) { return; }
        enemyKillstreak /= factor;
    }

    private void ResetKillstreak()
    {
        enemyKillstreak = 0;
    }

    // Accessor method for totalEnemiesDestroyed
    public int getTotalEnemiesDestroyed()
    {
        return totalEnemiesDestroyed;
    }

    // Setter method for totalEnemiesDestroyed
    public void increaseTotalEnemiesDestroyed()
    {
        ++totalEnemiesDestroyed;
        CheckKillstreak();
    }

    // Setter methods for lives left
    public void incrementLivesLeft()
    {
        if (livesLeft < maxLives)
        {
            ++livesLeft;
        }
    }

    public void decrementLivesLeft()
    {
        if (livesLeft > 0)
        {
            --livesLeft;
        }
        else
        {
            // Game Over
            PlayerPrefs.SetInt("HighScore", scoringSystem.highScore);
            hurtFXAnim.Play("UI_Hurt_GameOver");
            isDamaged = true;
        }
    }

    // Accessor method for lives left
    public int getLivesLeft()
    {
        return livesLeft;
    }

    // Accessor method for isDamaged
    public bool getIsDamaged()
    {
        return isDamaged;
    }

    // Accessor method for isDamageProof
    public bool getIsDamageproof()
    {
        return isDamageproof;
    }

    // Accessor method for currentScoreMultiplier
    public int getCurrentScoreMultiplier()
    {
        return currentScoreMultiplier;
    }

    // Accessor method for ejectionList Array
    public AllyType getEjectionList(int index)
    {
        if (index < 0 || index > 3) { return AllyType.None; }
        return ejectionList[index];
    }

    // Plays sound effects
    public void PlaySoundEffect(AudioClip theClip)
    {
        soundPlayer.clip = theClip;
        soundPlayer.Play();
    }

    public void Explode()
    {
        GameObject objTemp = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        ExplodeSoundScript explosionScript = objTemp.GetComponent<ExplodeSoundScript>();
        explosionScript.DoExplosion(0, 1.0f);
    }

    // Accessor method for isEjectModeOn
    public bool getIsEjectModeOn()
    {
        return isEjectModeOn;
    }

    // Accessor method for rigidbody velocity
    public float getVelocity()
    {
        return rb2D.velocity.magnitude;
    }
}
