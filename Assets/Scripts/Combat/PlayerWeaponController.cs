using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Weapon Data")]
    public WeaponData[] weapons;
    public int currentWeaponIndex = 0;

    [Header("References")]
    public Transform weaponSocket;
    public Transform firePoint;
    public Animator animator;

    private GameObject currentWeaponModel;
    private GameObject currentDefenseObject;

    private float nextAttackTime;
    private float nextDefenseTime;

    public bool IsDefending { get; private set; }
    public float CurrentDefenseDamageRate { get; private set; } = 1f;
    private RevivableAlly currentReviveTarget;

    private void Start()
    {
        if (weapons != null && weapons.Length > 0)
        {
            EquipWeapon(currentWeaponIndex);
        }
    }

    private void Update()
    {
        HandleWeaponChangeInput();
        HandleCombatInput();
    }

    private void HandleWeaponChangeInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) EquipWeapon(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) EquipWeapon(4);
    }

    private void HandleCombatInput()
    {
        if (weapons == null || weapons.Length == 0) return;

        WeaponData weapon = weapons[currentWeaponIndex];

        bool attackDown = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space);
        bool attackHeld = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);

        bool defenseDown = Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.LeftShift);
        bool defenseHeld = Input.GetMouseButton(1) || Input.GetKey(KeyCode.LeftShift);
        bool defenseUp = Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.LeftShift);

        if (weapon.attackType == AbilityType.PoisonMushroomSpread)
        {
            if (attackHeld)
            {
                ContinuePoisonMushroomSpread(weapon);
            }
        }
        else
        {
            if (attackDown)
            {
                UseAttack();
            }
        }

        if (weapon.defenseType == AbilityType.ElectricReviveChannel)
        {
            if (defenseHeld)
            {
                ContinueElectricRevive(weapon);
            }

            if (defenseUp)
            {
                StopElectricRevive();
            }
        }
        else
        {
            if (defenseDown)
            {
                UseDefense();
            }
        }
    }

    public void EquipWeapon(int index)
    {
        if (weapons == null) return;
        if (index < 0 || index >= weapons.Length) return;

        currentWeaponIndex = index;
        WeaponData weapon = weapons[currentWeaponIndex];

        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }

        if (weapon.weaponModelPrefab != null && weaponSocket != null)
        {
            currentWeaponModel = Instantiate(
                weapon.weaponModelPrefab,
                weaponSocket.position,
                weaponSocket.rotation,
                weaponSocket
            );

            currentWeaponModel.transform.localPosition = Vector3.zero;
            currentWeaponModel.transform.localRotation = Quaternion.identity;
        }

        Debug.Log($"Equipped Weapon: {weapon.weaponName}");
    }

    private void UseAttack()
    {
        if (weapons == null || weapons.Length == 0) return;

        WeaponData weapon = weapons[currentWeaponIndex];

        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + weapon.attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        switch (weapon.attackType)
        {
            case AbilityType.Projectile:
                SpawnProjectile(weapon);
                break;

            case AbilityType.Bomb:
                SpawnBomb(weapon);
                break;

            case AbilityType.Snowball:
                SpawnSnowball(weapon);
                break;

            case AbilityType.ElectricMelee:
                SpawnElectricMelee(weapon);
                break;

            case AbilityType.PoisonMushroomSpread:
                ContinuePoisonMushroomSpread(weapon);
                break;

            case AbilityType.None:
                Debug.Log("No attack ability assigned.");
                break;
        }
    }

    private void UseDefense()
    {
        if (weapons == null || weapons.Length == 0) return;

        WeaponData weapon = weapons[currentWeaponIndex];

        if (Time.time < nextDefenseTime) return;

        nextDefenseTime = Time.time + weapon.defenseCooldown;

        if (animator != null)
        {
            animator.SetTrigger("Defend");
        }

        switch (weapon.defenseType)
        {
            case AbilityType.Barrier:
                SpawnBarrier(weapon);
                break;

            case AbilityType.FlameWall:
                SpawnFlameWall(weapon);
                break;

            case AbilityType.IceShield:
                SpawnIceShield(weapon);
                break;

            case AbilityType.HealZone:
                SpawnHealZone(weapon);
                break;

            case AbilityType.None:
                Debug.Log("No defense ability assigned.");
                break;
        }
    }

     private void SpawnProjectile(WeaponData weapon)
    {
        if (weapon.attackPrefab == null)
        {
            Debug.LogWarning("Attack Prefab is not assigned.");
            return;
        }

        Transform spawnPoint = firePoint != null ? firePoint : transform;

        GameObject obj = Instantiate(
            weapon.attackPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        // Debug.Log($"Bullet spawned at {spawnPoint.position}, direction: {spawnPoint.forward}");

        Projectile projectile = obj.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Init(weapon.attackDamage, weapon.attackSpeed, spawnPoint.forward);
            // Debug.Log("Projectile script found and initialized.");
        }
        else
        {
            Debug.LogWarning("Projectile script is missing on Attack Prefab.");
        }
    }
    
    private void SpawnBarrier(WeaponData weapon)
    {
        if (weapon.defensePrefab == null)
        {
            Debug.LogWarning("Defense Prefab is not assigned.");
            return;
        }

        if (currentDefenseObject != null)
        {
            Destroy(currentDefenseObject);
        }

        currentDefenseObject = Instantiate(
            weapon.defensePrefab,
            transform.position,
            transform.rotation,
            transform
        );

        Debug.Log("Shield spawned.");

        Barrier barrier = currentDefenseObject.GetComponent<Barrier>();

        if (barrier != null)
        {
            IsDefending = true;
            CurrentDefenseDamageRate = weapon.defenseDamageRate;
            barrier.Init(weapon.defensePower, weapon.defenseDuration, weapon.defenseRadius, this);
        }
    }

    private void SpawnBomb(WeaponData weapon)
    {
        if (weapon.attackPrefab == null)
        {
            Debug.LogWarning("Bomb Prefab is not assigned.");
            return;
        }

        Transform spawnPoint = firePoint != null ? firePoint : transform;

        GameObject obj = Instantiate(
            weapon.attackPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Bomb bomb = obj.GetComponent<Bomb>();

        if (bomb != null)
        {
            bomb.Init(weapon.attackDamage, weapon.attackRadius);
        }
        else
        {
            Debug.LogWarning("Bomb script is missing on Attack Prefab.");
        }

        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 throwDirection = spawnPoint.forward + Vector3.up * 0.5f;
            rb.AddForce(throwDirection.normalized * weapon.attackSpeed, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("Rigidbody is missing on Bomb Prefab.");
        }
    }

    private void SpawnFlameWall(WeaponData weapon)
    {
        if (weapon.defensePrefab == null)
        {
            Debug.LogWarning("Flame Wall Prefab is not assigned.");
            return;
        }

        Vector3 spawnPosition = transform.position + transform.forward * 3f;
        spawnPosition.y = 0.05f;

        Quaternion spawnRotation = Quaternion.LookRotation(transform.forward, Vector3.up);

        GameObject obj = Instantiate(
            weapon.defensePrefab,
            spawnPosition,
            spawnRotation
        );

        FlameWall flameWall = obj.GetComponent<FlameWall>();

        if (flameWall != null)
        {
            flameWall.Init(weapon.defensePower, weapon.defenseDuration);
        }
        else
        {
            Debug.LogWarning("FlameWall script is missing on Defense Prefab.");
        }

        Debug.Log("Flame wall spawned.");
    }

    private void SpawnSnowball(WeaponData weapon)
    {
        if (weapon.attackPrefab == null)
        {
            Debug.LogWarning("Snowball Prefab is not assigned.");
            return;
        }

        Transform spawnPoint = firePoint != null ? firePoint : transform;

        GameObject obj = Instantiate(
            weapon.attackPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        SnowballProjectile snowball = obj.GetComponent<SnowballProjectile>();

        if (snowball != null)
        {
            snowball.Init(weapon.attackDamage);
        }
        else
        {
            Debug.LogWarning("SnowballProjectile script is missing on Attack Prefab.");
        }

        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 throwDirection = spawnPoint.forward + Vector3.up * 0.45f;
            rb.AddForce(throwDirection.normalized * weapon.attackSpeed, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("Rigidbody is missing on Snowball Prefab.");
        }
    }

    private void SpawnIceShield(WeaponData weapon)
    {
        if (weapon.defensePrefab == null)
        {
            Debug.LogWarning("Ice Shield Prefab is not assigned.");
            return;
        }

        if (currentDefenseObject != null)
        {
            Destroy(currentDefenseObject);
        }

        Vector3 spawnPosition = transform.position + transform.forward * 1.2f + Vector3.up * 1.0f;
        Quaternion spawnRotation = Quaternion.LookRotation(transform.forward, Vector3.up);

        currentDefenseObject = Instantiate(
            weapon.defensePrefab,
            spawnPosition,
            spawnRotation,
            transform
        );

        IceShield iceShield = currentDefenseObject.GetComponent<IceShield>();

        if (iceShield != null)
        {
            IsDefending = true;
            CurrentDefenseDamageRate = weapon.defenseDamageRate;
            iceShield.Init(weapon.defenseDuration, this);
        }
        else
        {
            Debug.LogWarning("IceShield script is missing on Defense Prefab.");
        }

        Debug.Log("Ice shield spawned.");
    }

    private void SpawnElectricMelee(WeaponData weapon)
    {
        if (weapon.attackPrefab == null)
        {
            Debug.LogWarning("Electric Melee Prefab is not assigned.");
            return;
        }

        Vector3 spawnPosition = transform.position + transform.forward * 1.2f + Vector3.up * 1.0f;
        Quaternion spawnRotation = Quaternion.LookRotation(transform.forward, Vector3.up);

        GameObject obj = Instantiate(
            weapon.attackPrefab,
            spawnPosition,
            spawnRotation,
            transform
        );

        ElectricMeleeHitbox hitbox = obj.GetComponent<ElectricMeleeHitbox>();

        if (hitbox != null)
        {
            hitbox.Init(weapon.attackDamage, weapon.attackDuration);
        }
        else
        {
            Debug.LogWarning("ElectricMeleeHitbox script is missing on Attack Prefab.");
        }

        Debug.Log("Electric swatter attack spawned.");
    }

    private void ContinueElectricRevive(WeaponData weapon)
    {
        if (Time.time < nextDefenseTime) return;

        RevivableAlly target = FindReviveTarget(weapon.defenseRadius);

        if (target == null)
        {
            if (currentReviveTarget != null)
            {
                Debug.Log("Electric revive target lost.");
                currentReviveTarget = null;
            }

            return;
        }

        if (currentReviveTarget != target)
        {
            currentReviveTarget = target;
            Debug.Log($"Electric revive started: {target.gameObject.name}");
        }

        target.AddReviveProgress(Time.deltaTime);

        if (!target.IsDowned)
        {
            nextDefenseTime = Time.time + weapon.defenseCooldown;
            StopElectricRevive();
        }
    }

    private RevivableAlly FindReviveTarget(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        RevivableAlly bestTarget = null;
        float bestDistance = float.MaxValue;

        foreach (Collider collider in colliders)
        {
            RevivableAlly ally = collider.GetComponentInParent<RevivableAlly>();

            if (ally == null) continue;
            if (!ally.IsDowned) continue;

            Vector3 directionToAlly = ally.transform.position - transform.position;
            directionToAlly.y = 0f;

            if (directionToAlly.magnitude <= 0.01f) continue;

            float forwardDot = Vector3.Dot(transform.forward, directionToAlly.normalized);

            // 플레이어 뒤쪽에 있는 아군은 소생 대상에서 제외
            if (forwardDot < 0.3f) continue;

            float distance = directionToAlly.magnitude;

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestTarget = ally;
            }
        }

        return bestTarget;
    }

    private void StopElectricRevive()
    {
        if (currentReviveTarget != null)
        {
            Debug.Log("Electric revive stopped.");
        }

        currentReviveTarget = null;
    }

    private void ContinuePoisonMushroomSpread(WeaponData weapon)
    {
        if (Time.time < nextAttackTime) return;

        if (weapon.attackPrefab == null)
        {
            Debug.LogWarning("Poison Mushroom Prefab is not assigned.");
            return;
        }

        nextAttackTime = Time.time + weapon.attackCooldown;

        Vector2 randomCircle = Random.insideUnitCircle * weapon.mushroomSpreadRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        spawnPosition.y = 0.05f;

        GameObject obj = Instantiate(
            weapon.attackPrefab,
            spawnPosition,
            Quaternion.identity
        );

        PoisonMushroomAlly mushroom = obj.GetComponent<PoisonMushroomAlly>();

        if (mushroom != null)
        {
            mushroom.Init(weapon.attackDamage, weapon.attackDuration, weapon.attackRadius);
        }
        else
        {
            Debug.LogWarning("PoisonMushroomAlly script is missing on Attack Prefab.");
        }

        Debug.Log("Poison mushroom spawned.");
    }

    private void SpawnHealZone(WeaponData weapon)
    {
        if (weapon.defensePrefab == null)
        {
            Debug.LogWarning("Heal Zone Prefab is not assigned.");
            return;
        }

        if (currentDefenseObject != null)
        {
            Destroy(currentDefenseObject);
        }

        Vector3 spawnPosition = transform.position + transform.forward * 1.5f;
        spawnPosition.y = 0.05f;

        currentDefenseObject = Instantiate(
            weapon.defensePrefab,
            spawnPosition,
            Quaternion.identity
        );

        HealZone healZone = currentDefenseObject.GetComponent<HealZone>();

        if (healZone != null)
        {
            healZone.Init(weapon.defensePower, weapon.defenseDuration, weapon.defenseRadius);
        }
        else
        {
            Debug.LogWarning("HealZone script is missing on Defense Prefab.");
        }

        Debug.Log("Heal zone spawned.");
    }


    public void EndDefense()
    {
        IsDefending = false;
        CurrentDefenseDamageRate = 1f;
        currentDefenseObject = null;
    }
}
