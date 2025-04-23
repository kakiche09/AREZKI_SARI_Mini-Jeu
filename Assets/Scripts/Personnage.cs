using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Personnage : MonoBehaviour
{
    private PlayerInputReader inputReader;
    private Vector2 direction;
    private Rigidbody2D Rb;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.16f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;

    [SerializeField] private float vitesse = 3f;
    public SpriteRenderer spriteRenderer    { get; protected set; }
    public NavMeshAgent agent               { get; private set;   }
    public Animator animator                { get; private set;   }

    [SerializeField] private Transform pointDeTir;
    [SerializeField] private float porteeRecherche = 10f;
    [SerializeField] private GameObject projectilePrefab;

    private Transform cibleActuelle;
    public enum TypeAttaque { Directe, Spirale, Poursuite };
    public TypeAttaque typeAttaque;
    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputReader = GetComponent<PlayerInputReader>();
        animator = GetComponent<Animator>();
        typeAttaque = TypeAttaque.Directe; 

        inputReader.BS.callback += Dash;
        inputReader.LS_m.callback += Deplacer;
        inputReader.BE.callback += Attaquer; 
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            Vector2 forceDescente = new Vector2(0, -0.2f); 
            Vector2 deplacementFinal = direction * vitesse + forceDescente;
            Rb.velocity = deplacementFinal;
        }
    }

    void Update()
    {
        flipX();
        Inclinaison();
        animator.SetFloat("Vitesse", Rb.velocity.magnitude);
        animator.SetBool("IsDashing", isDashing);
        Debug.Log("IsDashing: " + isDashing);
    }

    void Dash()
    {
        if (canDash && !isDashing && direction != Vector2.zero)
        {
            StartCoroutine(DashRoutine());
        }
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        canDash = false;

        float originalGravity = Rb.gravityScale;
        Rb.gravityScale = 0f;

        Rb.velocity = direction.normalized * dashForce;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        Rb.gravityScale = originalGravity;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Deplacer(Vector2 dir)
    {
        direction = dir.normalized; 
    }

    public void flipX()
    {
        if (Rb.velocity.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (Rb.velocity.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }
    
    void Inclinaison()
    {
        float vx = -Rb.velocity.x;
        float maxAngle = 25f;
        float targetAngle = Mathf.Clamp(vx / vitesse, -1f, 1f) * maxAngle;

        if (Mathf.Abs(vx) < 0.05f)
            targetAngle = 0f;

        float currentZ = transform.rotation.eulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;

        float newZ = Mathf.Lerp(currentZ, targetAngle, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Euler(0f, 0f, newZ);  
    }

    Transform ChercherEnnemi()
    {
        float rayonDetection = 20f;    
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, rayonDetection);
        float distanceMin = Mathf.Infinity;
        Transform ennemiLePlusProche = null;
        foreach (var collider  in colliders)
        {
            Slimes potentielEnnemi = collider.GetComponent<Slimes>();
            if (potentielEnnemi != null)
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);

                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    ennemiLePlusProche = collider.transform;
                }
            }
        }
        return ennemiLePlusProche;
    }

    void Attaquer()
    {
        switch (typeAttaque)
        {
            case TypeAttaque.Directe:
                AttaqueLigne();
                break;
            case TypeAttaque.Spirale:
                AttaqueSpirale();
                break;
            case TypeAttaque.Poursuite:
                AttaquePoursuite();
                break;
        }
    }
    
    void AttaqueLigne()
    {
        if (direction == Vector2.zero) return;
        {
            GameObject projectile = Instantiate(projectilePrefab, pointDeTir.position, Quaternion.identity);
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Initialiser(direction, null, Projectile.TypeProjectile.Directe);
        }
    }
    
    void AttaqueSpirale()
    {
        if (direction == Vector2.zero) return;
        {    
            GameObject projectile = Instantiate(projectilePrefab, pointDeTir.position, Quaternion.identity);
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Initialiser(direction, null, Projectile.TypeProjectile.Spirale);
        }
    }

    void AttaquePoursuite()
    {
        cibleActuelle = ChercherEnnemi();
        if (cibleActuelle != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, pointDeTir.position, Quaternion.identity);
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Initialiser(Vector2.zero, cibleActuelle, Projectile.TypeProjectile.Poursuite);
        }
       
    }   
    
}
