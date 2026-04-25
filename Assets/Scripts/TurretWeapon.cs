using UnityEngine;
using UnityEngine.InputSystem;

public class TurretWeapon : MonoBehaviour
{
    public enum MouseFireButton
    {
        None,
        Left,
        Right,
        Middle
    }
    [Header("Turret Parts")]
    public Transform yawPivot;
    public Transform pitchPivot;
    public Transform firePoint;

    [Header("Rotation Keys")]
    public Key rotateLeftKey = Key.LeftArrow;
    public Key rotateRightKey = Key.RightArrow;
    public Key rotateUpKey = Key.UpArrow;
    public Key rotateDownKey = Key.DownArrow;

    [Header("Rotation Limits Relative To Start")]
    public float minYaw = -45f;
    public float maxYaw = 45f;
    public float minPitch = -30f;
    public float maxPitch = 30f;

    [Header("Rotation")]
    public float rotationSpeed = 60f;

    public Vector3 yawAxis = Vector3.up;
    public Vector3 pitchAxis = Vector3.right;

    public bool invertPitch = false;

    [Header("Fire Binding")]
    public Key fireKey = Key.Space;
    public MouseFireButton mouseFireButton = MouseFireButton.None;
    public bool holdToFire = true;
    [SerializeField]public Weapon weapon;

    
   

    //[Header("Projectile Settings")]
    //public float projectileSpeed = 40f;
    //public float projectileLifeTime = 4f;
    //public GameObject projectileExplosionPrefab;
    //public float projectileExplosionLifetime = 2f;

    //[Header("Projectile Scale Animation")]
    //public float projectileTargetZScale = 0.1f;
    //public float projectileGrowDuration = 0.08f;
    //public float projectileShrinkDuration = 0.15f;

    //[Header("Projectile Damage")]
    //public float projectileShieldDamage = 15f;
    //public float projectilePhysicalDamage = 10f;

    [Header("Projectile Ignore")]
    public bool projectileIgnoresOwnShields = true;
    public ShieldSector[] shieldsToIgnore;

    [Header("Beam")]
    public LineRenderer beamLine;

    [Tooltip("Only visual length of the beam.")]
    public float beamVisualDistance = 100f;

    [Tooltip("How far the turret checks for a target. Use a very large value for almost infinite.")]
    public float beamHitDetectionDistance = 10000f;

    public LayerMask beamHitMask = ~0;
    public Transform ignoreRoot;

    public Color beamNormalColor = new Color(0f, 0.7f, 1f, 0.45f);
    public Color beamHitColor = new Color(1f, 0f, 0f, 0.9f);

    private Quaternion yawStartRotation;
    private Quaternion pitchStartRotation;

    private float currentYaw;
    private float currentPitch;
    private float fireTimer;

    void Start()
    {
        if (yawPivot != null)
            yawStartRotation = yawPivot.localRotation;

        if (pitchPivot != null)
            pitchStartRotation = pitchPivot.localRotation;

        if (ignoreRoot == null)
            ignoreRoot = transform.root;

        if (beamLine == null)
            beamLine = GetComponentInChildren<LineRenderer>();

        if (beamLine != null)
        {
            beamLine.positionCount = 2;
            beamLine.useWorldSpace = true;
        }
    }

    void Update()
    {
        UpdateRotation();
        UpdateFire();
        UpdateBeam();
    }

    void UpdateRotation()
    {
        if (yawPivot == null || pitchPivot == null)
            return;

        float yawInput = 0f;
        float pitchInput = 0f;

        if (IsKeyPressed(rotateLeftKey))
            yawInput = -1f;

        if (IsKeyPressed(rotateRightKey))
            yawInput = 1f;

        if (IsKeyPressed(rotateUpKey))
            pitchInput = 1f;

        if (IsKeyPressed(rotateDownKey))
            pitchInput = -1f;

        if (invertPitch)
            pitchInput *= -1f;

        currentYaw += yawInput * rotationSpeed * Time.deltaTime;
        currentPitch += pitchInput * rotationSpeed * Time.deltaTime;

        currentYaw = Mathf.Clamp(currentYaw, minYaw, maxYaw);
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        yawPivot.localRotation =
            yawStartRotation *
            Quaternion.AngleAxis(currentYaw, yawAxis.normalized);

        pitchPivot.localRotation =
            pitchStartRotation *
            Quaternion.AngleAxis(currentPitch, pitchAxis.normalized);
    }

    void UpdateFire()
    {
        if (weapon.projectilePrefab == null || firePoint == null)
            return;

        fireTimer -= Time.deltaTime;

        if (IsFirePressed() && fireTimer <= 0f)
        {
            Fire();
            fireTimer = weapon.fireRate;
        }
    }

    bool IsFirePressed()
    {
        bool keyboardFire;
        bool mouseFire;

        if (holdToFire)
        {
            keyboardFire = IsKeyPressed(fireKey);
            mouseFire = IsMouseButtonPressed(mouseFireButton);
        }
        else
        {
            keyboardFire = WasKeyPressedThisFrame(fireKey);
            mouseFire = WasMouseButtonPressedThisFrame(mouseFireButton);
        }

        return keyboardFire || mouseFire;
    }

    void Fire()
    {
        GameObject projectile = Instantiate(
            weapon.projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        ConfigureProjectile(projectile);
    }

    void ConfigureProjectile(GameObject projectile)
    {
        Projectile projectileController =
            projectile.GetComponent<Projectile>();

        if (projectileController != null)
        {
            projectileController = weapon.configureProjectile(projectileController);

            projectileController.ignoreRoot = transform.root;
        }

        ShieldDamageController shieldDamage =
            projectile.GetComponent<ShieldDamageController>();

        if (shieldDamage != null)
        {
            //shieldDamage.shieldDamage = projectileShieldDamage;

            if (projectileIgnoresOwnShields)
            {
                ShieldSector[] ignored = shieldsToIgnore;

                if (ignored == null || ignored.Length == 0)
                    ignored = transform.root.GetComponentsInChildren<ShieldSector>(true);

                shieldDamage.SetIgnoredShields(ignored);
            }
        }

        //PhysicalDamageSource physicalDamage =
        //    projectile.GetComponent<PhysicalDamageSource>();

        //if (physicalDamage != null)
        //    physicalDamage.physicalDamage = projectilePhysicalDamage;
    }

    void UpdateBeam()
    {
        if (beamLine == null || firePoint == null)
            return;

        Vector3 start = firePoint.position;
        Vector3 direction = firePoint.forward;

        Vector3 visualEnd = start + direction * beamVisualDistance;

        bool hitSomething = FindBeamHit(start, direction, out RaycastHit hit);

        beamLine.SetPosition(0, start);
        beamLine.SetPosition(1, visualEnd);

        if (hitSomething)
            ApplyBeamColor(beamHitColor);
        else
            ApplyBeamColor(beamNormalColor);
    }

    bool FindBeamHit(Vector3 start, Vector3 direction, out RaycastHit closestHit)
    {
        closestHit = new RaycastHit();

        RaycastHit[] hits = Physics.RaycastAll(
            start,
            direction,
            beamHitDetectionDistance,
            beamHitMask,
            QueryTriggerInteraction.Collide
        );

        bool found = false;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider hitCollider = hits[i].collider;

            if (hitCollider == null)
                continue;

            if (ShouldIgnoreCollider(hitCollider))
                continue;

            if (hits[i].distance < closestDistance)
            {
                closestDistance = hits[i].distance;
                closestHit = hits[i];
                found = true;
            }
        }

        return found;
    }

    bool ShouldIgnoreCollider(Collider hitCollider)
    {
        if (ignoreRoot == null)
            return false;

        if (hitCollider.transform == ignoreRoot)
            return true;

        if (hitCollider.transform.IsChildOf(ignoreRoot))
            return true;

        return false;
    }

    void ApplyBeamColor(Color color)
    {
        if (beamLine == null)
            return;

        beamLine.startColor = color;

        Color endColor = color;
        endColor.a = 0f;
        beamLine.endColor = endColor;

        if (beamLine.material != null)
        {
            if (beamLine.material.HasProperty("_BaseColor"))
                beamLine.material.SetColor("_BaseColor", color);

            if (beamLine.material.HasProperty("_Color"))
                beamLine.material.SetColor("_Color", color);

            if (beamLine.material.HasProperty("_EmissionColor"))
                beamLine.material.SetColor("_EmissionColor", color);
        }
    }

    bool IsKeyPressed(Key key)
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return false;

        if (key == Key.None)
            return false;

        return keyboard[key].isPressed;
    }

    bool WasKeyPressedThisFrame(Key key)
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return false;

        if (key == Key.None)
            return false;

        return keyboard[key].wasPressedThisFrame;
    }

    bool IsMouseButtonPressed(MouseFireButton button)
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
            return false;

        if (button == MouseFireButton.Left)
            return mouse.leftButton.isPressed;

        if (button == MouseFireButton.Right)
            return mouse.rightButton.isPressed;

        if (button == MouseFireButton.Middle)
            return mouse.middleButton.isPressed;

        return false;
    }

    bool WasMouseButtonPressedThisFrame(MouseFireButton button)
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
            return false;

        if (button == MouseFireButton.Left)
            return mouse.leftButton.wasPressedThisFrame;

        if (button == MouseFireButton.Right)
            return mouse.rightButton.wasPressedThisFrame;

        if (button == MouseFireButton.Middle)
            return mouse.middleButton.wasPressedThisFrame;

        return false;
    }
}