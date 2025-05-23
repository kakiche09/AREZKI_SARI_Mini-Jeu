using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AI;

public class Personnage : MonoBehaviour
{
    
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.16f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private Transform pointDeTir;
    [SerializeField] private float porteeRecherche = 10f;
    [SerializeField] private GameObject projectileDirect;
    [SerializeField] private GameObject projectileSpiral;
    [SerializeField] private GameObject projectilePoursuite;
    [SerializeField] private TMPro.TMP_Text texteScore;
    [SerializeField] private TMPro.TMP_Text chronoTexte;
    [SerializeField] private AudioClip sonAttaque;
    [SerializeField] private AudioClip sonDash;
    [SerializeField] private AudioClip sonDegats;
    [SerializeField] private AudioClip sonMort;
    [SerializeField] private AudioClip sonVies;
    [SerializeField] private AudioClip sonArme;
    [SerializeField] private AudioClip sonScore;
    [SerializeField] private AudioClip sonFinJeu;
    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioClip sonFond;
    [SerializeField] private GameObject canvasFinDeJeu;


    public enum TypeAttaque { Directe, Spirale, Poursuite };
    public PlayerInputReader inputReader        { get; protected set; }
    public Vector2 direction                    { get; protected set; }
    public Rigidbody2D Rb                       { get; protected set; }
    public SpriteRenderer spriteRenderer        { get; protected set; }
    public NavMeshAgent agent                   { get; protected set; }
    public Animator animator                    { get; protected set; }
    public Transform cibleActuelle              { get; protected set; }
    public TypeAttaque typeAttaque              { get; protected set; }
    public GestionnaireUI gestionnaireUI        { get; protected set; }

    public float vitesse                    { get; protected set; }
    public bool isDashing                   { get; protected set; }
    public bool canDash                     { get; protected set; }
    public int viesRestantes                { get; protected set; }
    public float tempsEntreAttaques         { get; protected set; }
    public bool peutAttaquer                { get; protected set; }
    public int coeursRestants               { get; protected set; }
    public int progressionArme              { get; protected set; }
    public bool jeuTermine                  { get; protected set; }
    public float positionArriveeX           { get; protected set; }
    public float rayonDetection             { get; protected set; }

    public int score { get; set; }

    public int CoeursRestants => coeursRestants;
    public int ViesRestantes => viesRestantes;
    public int ProgressionArme => progressionArme;


    void Start()
    {
        // Charger les données 
        Rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputReader = GetComponent<PlayerInputReader>();
        animator = GetComponent<Animator>();
        gestionnaireUI = FindObjectOfType<GestionnaireUI>();
        audioSource1 = GetComponent<AudioSource>();

        // Charger le son de fond
        audioSource1.clip = sonFond;
        audioSource1.loop = true;
        audioSource1.Play();

        // Initialiser les attributs
        SetAttributes();
        
        inputReader.BS.callback += Dash;
        inputReader.LS_m.callback += Deplacer;
        inputReader.BE.callback += Attaquer;

        if (canvasFinDeJeu != null)
        {
            canvasFinDeJeu.SetActive(false);
        }

    }

    // Fonction pour initialiser les attributs du personnage
    void SetAttributes()
    {
        vitesse = 5f;
        isDashing = false;
        canDash = true;
        viesRestantes = 100;
        tempsEntreAttaques = 0.3f;
        peutAttaquer = true;
        coeursRestants = 3;
        score = 0;
        progressionArme = 0;
        jeuTermine = false;
        positionArriveeX = 320f;
        rayonDetection = 20f;
    }


    void Update()
    {
        
        flipX();
        Inclinaison();
        
        // Mettre à jour la vitesse de l'animation
        // et l'état de dash
        animator.SetFloat("Vitesse", Rb.velocity.magnitude);
        animator.SetBool("IsDashing", isDashing);

        // Mettre à jour la direction du personnage
        if ((transform.position.x >= positionArriveeX && !jeuTermine) || (CoeursRestants <= 0))
        {
            TerminerJeu();
        }
        // Vérifier si le personnage est en train de dash
        if (!isDashing)
        {
            Vector2 forceDescente = new Vector2(0, -0.2f);
            Vector2 deplacementFinal = direction * vitesse + forceDescente;
            Rb.velocity = deplacementFinal;
        }
        // Gerer la progression de l'arme
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

    // Fonction pour gérer le dash
    void Dash()
    {
        if (canDash && !isDashing && direction != Vector2.zero)
        {
            StartCoroutine(DashRoutine());
            audioSource1.PlayOneShot(sonDash);
        }
    }

    // Coroutine pour gérer le dash
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

    // Fonction pour déplacer le personnage
    void Deplacer(Vector2 dir)
    {
        direction = dir.normalized;
    }

    // Fonction pour gérer la direction du sprite
    // en fonction de la vitesse
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

    // Fonction pour incliner le personnage
    void Inclinaison()
    {
        float vx = -Rb.velocity.x;
        float maxAngle = 25f;
        float targetAngle = Mathf.Clamp(vx / vitesse, -1f, 1f) * maxAngle;

        // Si la vitesse est très faible, on remet l'angle à 0
        if (Mathf.Abs(vx) < 0.05f)
            targetAngle = 0f;

        // On utilise Mathf.Lerp pour lisser la rotation
        float currentZ = transform.rotation.eulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;

        float newZ = Mathf.Lerp(currentZ, targetAngle, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Euler(0f, 0f, newZ);
    }

    // Fonction pour chercher un ennemi
    // On utilise un rayon de détection pour trouver l'ennemi le plus proche
    Transform ChercherEnnemi()
    {
        // On cherche tous les colliders dans un rayon de détection
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, rayonDetection);
        float distanceMin = Mathf.Infinity;
        Transform ennemiLePlusProche = null;

        // On parcourt tous les colliders trouvés
        // et on cherche le plus proche
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
        // Si un ennemi est trouvé, on le retourne
        // Sinon, on retourne null
        return ennemiLePlusProche;
    }

    // Fonction pour gérer l'attaque
    // On utilise un switch pour choisir le type d'attaque
    void Attaquer()
    {
        if (peutAttaquer)
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

            // Après l'attaque, on lance la coroutine pour le cooldown
            StartCoroutine(AttaqueCooldown());
            audioSource1.PlayOneShot(sonAttaque);
        }
    }

    // Fonctio pour l'aattaque directe
    // On utilise un decalage pour le point de tir
    void AttaqueLigne()
    {
        if (direction == Vector2.zero) return;
        {
            Vector2 decalage = direction.normalized * 0.7f;
            GameObject projectile = Instantiate(projectileDirect, (Vector2)transform.position + decalage, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialiser(direction, null, Projectile.TypeProjectile.Directe, gameObject);

        }
    }

    // Fonction pour l'attaque en spirale
    // On utilise un angle pour faire tourner le projectile
    void AttaqueSpirale()
    {
        if (direction == Vector2.zero) return;
        {
            Vector2 decalage = direction.normalized * 0.7f;
            GameObject projectile = Instantiate(projectileSpiral, (Vector2)transform.position + decalage, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialiser(direction, null, Projectile.TypeProjectile.Spirale, gameObject);
        }
    }

    // Fonction pour l'attaque en poursuite
    // On cherche l'ennemi le plus proche et on le suit
    void AttaquePoursuite()
    {
        cibleActuelle = ChercherEnnemi();
        if (cibleActuelle != null)
        {
            GameObject projectile = Instantiate(projectilePoursuite, pointDeTir.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialiser(Vector2.zero, cibleActuelle, Projectile.TypeProjectile.Poursuite, gameObject);
        }

    }

    // Fonction pour gérer les dégâts
    // On utilise un son pour les dégâts et un autre pour la mort
    public void SubirDegats(int degats)
    {
        viesRestantes -= degats;
        audioSource1.PlayOneShot(sonDegats);

        // Vérifie si le joueur a perdu toutes ses vies
        // Si c'est le cas, on enlève un coeur
        if (viesRestantes <= 0)
        {
            coeursRestants--;
            audioSource1.PlayOneShot(sonMort);
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
        }
    }

    // Fonction pour ajouter des vies
    public void AjouterVie(int vies)
    {
        viesRestantes += vies;
        audioSource1.PlayOneShot(sonVies);
    }

    // Fonction pour ajouter des coeurs
    public void AjouterCoeur(int coeur)
    {
        if (coeursRestants < 3)
        {
            coeursRestants += coeur;
            audioSource1.PlayOneShot(sonMort);
        }
    }

    // Fonction pour ajouter des points au score
    public void AugmenterBarreArme(int points)
    {
        progressionArme += points;
        audioSource1.PlayOneShot(sonArme);
    }

    // Fonction pour terminer le jeu
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
            // Jouer le son de fin de jeu
            audioSource1.PlayOneShot(sonFinJeu);
        }
    }

    // Fonction pour gérer le cooldown de l'attaque
    IEnumerator AttaqueCooldown()
    {
        peutAttaquer = false;  // Empêche d'attaquer pendant le délai
        yield return new WaitForSeconds(tempsEntreAttaques);  // Attendre le temps de cooldown
        peutAttaquer = true;  // Permet à nouveau d'attaquer
    }
}
