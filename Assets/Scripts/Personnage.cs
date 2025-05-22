using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AI;

public class Personnage : MonoBehaviour
{
    private PlayerInputReader inputReader;
    private Vector2 direction;
    public Rigidbody2D Rb { get; private set; }
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.16f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;
    [SerializeField] public float vitesse { get; private set; }

    public SpriteRenderer spriteRenderer { get; protected set; }
    public NavMeshAgent agent { get; private set; }
    public Animator animator { get; private set; }
    private int viesRestantes = 100;

    [SerializeField] private Transform pointDeTir;
    [SerializeField] private float porteeRecherche = 10f;
    [SerializeField] private GameObject projectileDirect;
    [SerializeField] private GameObject projectileSpiral;
    [SerializeField] private GameObject projectilePoursuite;
    [SerializeField] private TMPro.TMP_Text texteScore;
    [SerializeField]private TMPro.TMP_Text chronoTexte;


    private Transform cibleActuelle;
    public enum TypeAttaque { Directe, Spirale, Poursuite };
    public TypeAttaque typeAttaque;
    private int coeursRestants = 3;
    public int CoeursRestants => coeursRestants;
    public int ViesRestantes => viesRestantes;
    public int score = 0;
    public int progressionArme = 0;
    private bool jeuTermine = false;
    [SerializeField] private GameObject canvasFinDeJeu;
    private float positionArriveeX = 20f;
    private GestionnaireUI gestionnaireUI;


    void Start()
    {
        // Charger les données 
        Rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputReader = GetComponent<PlayerInputReader>();
        animator = GetComponent<Animator>();
        gestionnaireUI = FindObjectOfType<GestionnaireUI>();
        vitesse = 3f;


        inputReader.BS.callback += Dash;
        inputReader.LS_m.callback += Deplacer;
        inputReader.BE.callback += Attaquer;

        if (canvasFinDeJeu != null)
        {
            canvasFinDeJeu.SetActive(false);
        }

    }


    void Update()
    {

        flipX();
        Inclinaison();
        animator.SetFloat("Vitesse", Rb.velocity.magnitude);
        animator.SetBool("IsDashing", isDashing);
        
        if (transform.position.x >= positionArriveeX && !jeuTermine)
        {
            TerminerJeu();
        }
        if (!isDashing)
        {
            Vector2 forceDescente = new Vector2(0, -0.2f);
            Vector2 deplacementFinal = direction * vitesse + forceDescente;
            Rb.velocity = deplacementFinal;
        }
        if (progressionArme <= 10)
        {
            typeAttaque = TypeAttaque.Directe;
        }
        else if (progressionArme > 10 && progressionArme <= 20)
        {
            typeAttaque = TypeAttaque.Spirale;
        }
        else
        {
            typeAttaque = TypeAttaque.Poursuite;
        }
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
        foreach (var collider in colliders)
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
            Vector2 decalage = direction.normalized * 0.7f;
            GameObject projectile = Instantiate(projectileDirect, (Vector2)transform.position + decalage, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialiser(direction, null, Projectile.TypeProjectile.Directe, gameObject);

        }
    }

    void AttaqueSpirale()
    {
        if (direction == Vector2.zero) return;
        {
            Vector2 decalage = direction.normalized * 0.7f;
            GameObject projectile = Instantiate(projectileSpiral, (Vector2)transform.position + decalage, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialiser(direction, null, Projectile.TypeProjectile.Spirale, gameObject);
        }
    }

    void AttaquePoursuite()
    {
        cibleActuelle = ChercherEnnemi();
        if (cibleActuelle != null)
        {
            GameObject projectile = Instantiate(projectilePoursuite, pointDeTir.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialiser(Vector2.zero, cibleActuelle, Projectile.TypeProjectile.Poursuite, gameObject);
        }

    }
    public void SubirDegats(int degats)
    {
        viesRestantes -= degats;

        if (viesRestantes <= 0)
        {
            coeursRestants--;
            if (coeursRestants >= 0)
            {
                // Vérifie si la position en X est supérieure à 10
                if (transform.position.x > 10)
                {
                    // Si c'est le cas, respawn à la position actuelle -10 sur X et Y = 0
                    transform.position = new Vector2(transform.position.x - 10, 0f);
                }
                else
                {
                    // Si la position X est <= 10, respawn à la position actuelle avec Y = 0
                    transform.position = new Vector2(transform.position.x, 0f);
                }

                viesRestantes = 100;
            }
            else
            {
                Debug.Log("Fin du jeu");
                Time.timeScale = 0f;
            }
        }
    }


    public void AjouterVie(int vies)
    {

        viesRestantes += vies;
    }

    public void AjouterCoeur(int coeur)
    {
        if (coeursRestants < 3)
        {
            coeursRestants += coeur;
        }
    }

    public void AugmenterBarreArme(int points)
    {
        progressionArme += points;
    }

    public void TerminerJeu()
    {
        if (!jeuTermine)
        {
            jeuTermine = true;            
            // Mettre le temps en pause pour arrêter le jeu
            Time.timeScale = 0f;

            // Afficher le Canvas de fin de jeu
            if (canvasFinDeJeu != null)
            {
                canvasFinDeJeu.SetActive(true);

                // Récupérer et afficher le temps de jeu
                int minutes = Mathf.FloorToInt(gestionnaireUI.TempsPasse / 60F);
                int secondes = Mathf.FloorToInt(gestionnaireUI.TempsPasse - minutes * 60);
                chronoTexte.text = string.Format("Votre temps de jeu: {0:0}:{1:00}", minutes, secondes);

                // Afficher le score final
                texteScore.text = "Votre score final: " + score;
            }
        }
    }
}
