using System.Collections;
using UnityEngine;

public class CharacterShooter : MonoBehaviour
{
    public enum AttackType
    {
        Melee,
        Projectile
    }

    public static CharacterShooter Active { get; private set; }

    [Header("Attack")]
    [SerializeField] private AttackType attackType = AttackType.Melee;
    [SerializeField, Min(1)] private int damage = 100;
    [SerializeField, Min(0f)] private float hitDelay = 0.2f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField, Min(0.01f)] private float animationDuration = 0.5f;
    [SerializeField] private bool continuousAnimation;

    [Header("Projectile")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private CheeseProjectile projectilePrefab;

    [Header("Automatic Attack Card")]
    [SerializeField, Min(0.1f)] private float automaticAttackInterval = 1f;
    [SerializeField] private bool automaticAttackUnlocked;
    [SerializeField] private bool automaticAttackEnabled;

    private bool isAttacking;
    private bool attackQueued;
    private AttackSource queuedAttackSource;
    private CheeseController currentTarget;
    private Coroutine attackCoroutine;
    private float nextAutomaticAttackTime;

    public bool AutomaticAttackUnlocked => automaticAttackUnlocked;
    public bool AutomaticAttackEnabled => automaticAttackEnabled;

    public void MakeActiveManualAttacker()
    {
        if (isActiveAndEnabled)
            Active = this;
    }

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        ShowIdlePose();
    }

    private void OnEnable()
    {
        Active = this;
        ShowIdlePose();
    }

    private void OnDisable()
    {
        if (Active == this)
            Active = null;

        if (currentTarget != null)
            currentTarget.Died -= HandleTargetDied;

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        isAttacking = false;
        attackQueued = false;
        queuedAttackSource = AttackSource.Manual;
        currentTarget = null;
        attackCoroutine = null;
    }

    private void Update()
    {
        if (!automaticAttackUnlocked || !automaticAttackEnabled || isAttacking ||
            Time.time < nextAutomaticAttackTime)
            return;

        CheeseController target = FindTarget();

        if (target == null)
            return;

        bool attackAccepted = Attack(target, AttackSource.Automatic);

        if (!attackAccepted)
            return;

        float speedMultiplier = GetAttackSpeedMultiplier();
        float effectiveAutomaticInterval = automaticAttackInterval / speedMultiplier;
        nextAutomaticAttackTime = Time.time + Mathf.Max(0.1f, effectiveAutomaticInterval);
    }

    public bool Attack(CheeseController target, AttackSource source = AttackSource.Manual)
    {
        if (target == null || !target.IsAlive)
            return false;

        if (!isAttacking)
        {
            BeginAttack(target, source);
            return true;
        }

        if (source == AttackSource.Automatic)
            return false;

        if (target == currentTarget && currentTarget.IsAlive && !attackQueued)
        {
            attackQueued = true;
            queuedAttackSource = source;
            return true;
        }

        return false;
    }

    public void UnlockAutomaticAttack()
    {
        automaticAttackUnlocked = true;
        automaticAttackEnabled = true;
        nextAutomaticAttackTime = Time.time;
    }

    public void SetAutomaticAttackEnabled(bool enabled)
    {
        automaticAttackEnabled = automaticAttackUnlocked && enabled;
        if (automaticAttackEnabled)
            nextAutomaticAttackTime = Time.time;
    }

    private void BeginAttack(CheeseController target, AttackSource source)
    {
        if (target == null || !target.IsAlive)
            return;

        isAttacking = true;
        attackQueued = false;
        currentTarget = target;
        currentTarget.Died += HandleTargetDied;
        attackCoroutine = StartCoroutine(AttackSequence(target, source));
    }

    private IEnumerator AttackSequence(CheeseController target, AttackSource source)
    {
        float speedMultiplier = GetAttackSpeedMultiplier();
        float effectiveAnimationDuration =
            Mathf.Max(0.01f, animationDuration / speedMultiplier);
        float effectiveHitDelay = Mathf.Clamp(
            hitDelay / speedMultiplier, 0f, effectiveAnimationDuration);

        PlayAttackAnimation(speedMultiplier);

        if (effectiveHitDelay > 0f)
            yield return new WaitForSeconds(effectiveHitDelay);

        if (target != null && target.IsAlive)
        {
            if (attackType == AttackType.Projectile)
                LaunchProjectile(target, source);
            else
            {
                int appliedDamage = target.TakeDamage(GetAttackDamage(), target.HitPosition);

                if (appliedDamage > 0)
                    RewardSuccessfulHit(source);
            }
        }

        float remainingAnimationTime = effectiveAnimationDuration - effectiveHitDelay;
        if (remainingAnimationTime > 0f)
            yield return new WaitForSeconds(remainingAnimationTime);

        FinishAttack(target);
    }

    private void RewardSuccessfulHit(AttackSource source)
    {
        GameAssets assets = GameAssets.Instance;

        if (assets != null && assets.CombatRewards != null)
            assets.CombatRewards.RewardSuccessfulHit(source);
    }

    private float GetAttackSpeedMultiplier()
    {
        GameAssets assets = GameAssets.Instance;

        if (assets == null || assets.PlayerProgress == null)
            return 1f;

        return Mathf.Max(0.1f, assets.PlayerProgress.AttackSpeedMultiplier);
    }

    private int GetAttackDamage()
    {
        GameAssets assets = GameAssets.Instance;
        float multiplier = assets != null && assets.PlayerProgress != null
            ? assets.PlayerProgress.AttackDamageMultiplier
            : 1f;

        return Mathf.Max(1, Mathf.RoundToInt(damage * multiplier));
    }

    private void FinishAttack(CheeseController target)
    {
        bool canRunQueuedAttack =
            attackQueued &&
            currentTarget == target &&
            currentTarget != null &&
            currentTarget.IsAlive &&
            currentTarget.CurrentHealth > 0;

        CheeseController queuedTarget = canRunQueuedAttack ? currentTarget : null;

        AttackSource nextAttackSource = queuedAttackSource;

        if (currentTarget != null)
            currentTarget.Died -= HandleTargetDied;

        isAttacking = false;
        attackQueued = false;
        queuedAttackSource = AttackSource.Manual;
        currentTarget = null;
        attackCoroutine = null;

        if (canRunQueuedAttack)
            BeginAttack(queuedTarget, nextAttackSource);
        else
            ShowIdlePose();
    }

    private void HandleTargetDied(CheeseController deadCheese)
    {
        if (deadCheese != currentTarget)
            return;

        deadCheese.Died -= HandleTargetDied;
        attackQueued = false;
        queuedAttackSource = AttackSource.Manual;
        currentTarget = null;
    }

    private void LaunchProjectile(CheeseController target, AttackSource source)
    {
        if (firePoint == null || projectilePrefab == null)
        {
            Debug.LogError("A projectile attacker needs a Fire Point and Projectile Prefab.", this);
            return;
        }

        GameAssets assets = GameAssets.Instance;
        Transform parent = assets != null ? assets.EffectsParent : null;
        CheeseProjectile projectile = Instantiate(
            projectilePrefab, firePoint.position, Quaternion.identity, parent);
        projectile.Launch(target, GetAttackDamage(), source);
    }

    private void PlayAttackAnimation(float speedMultiplier)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return;

        if (continuousAnimation)
        {
            animator.enabled = true;
            return;
        }

        animator.enabled = true;
        animator.speed = speedMultiplier;
        animator.Play(0, 0, 0f);
        animator.Update(0f);
    }

    private void ShowIdlePose()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return;

        animator.enabled = true;
        animator.speed = 1f;
        animator.Play(0, 0, 0f);
        animator.Update(0f);

        if (!continuousAnimation)
            animator.enabled = false;
    }

    private static CheeseController FindTarget()
    {
        foreach (CheeseController cheese in FindObjectsByType<CheeseController>())
        {
            if (cheese.IsAlive)
                return cheese;
        }

        return null;
    }
}
