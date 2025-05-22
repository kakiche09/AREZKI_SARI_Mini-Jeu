using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;



public class Slimes : MonoBehaviour
{
    enum Etats { Recherche, Combat, Mort };
    
    [SerializeField] private GameObject objetScore;
    [SerializeField] private GameObject objetVie;
    [SerializeField] private GameObject objetCoeur;
    [SerializeField] private GameObject objetArme;
    [SerializeField] private GameObject projectilePrefab;

    private Personnage joueur;
    private Etats etatActuel                                    { get; set; }
    public float vitesse                                        { get; protected set; }
    public float distanceMax                                    { get; protected set; }

    public Vector2 positionInitiale                             { get; protected set; }
    public int direction                                        { get; protected set; }
    public int vie                                              { get; protected set; }
    public Rigidbody2D Rb                                       { get; protected set; }
    public Animator animator                                    { get; protected set; }
    public SpriteRenderer spriteRenderer                        { get; protected set; }
    public float tempsEntreAttaques                             { get; protected set; }
    public float tempsDerniereAttaque                           { get; protected set; }
    public float rayonDetection                                 { get; protected set; }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] AudioClip sonAttaque;
    [SerializeField] AudioClip sonMort;
    [SerializeField] AudioClip sonDegats;

    void Start()
    {
        // Charger les données
        Rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        joueur = FindObjectOfType<Personnage>();

        positionInitiale = transform.position;
        etatActuel = Etats.Recherche;
        positionInitiale = transform.position;

        // Initialiser les attributs
        SetAttributes();

    }

    // Fonction pour initialiser les attributs de l'ennemi
    private void SetAttributes()
    {
        vitesse = Random.Range(1f, 3f);
        distanceMax = Random.Range(2f, 4f);
        vie = Random.Range(20, 50);
        tempsEntreAttaques = Random.Range(0.8f, 1.8f);
        tempsDerniereAttaque = 0f;
        direction = 1;
        rayonDetection = 8f;

    }

    void Update()
    {
        // Vérifier en quel état se trouve l'ennemi et mettre à jour en conséquence
        switch (etatActuel)
        {
            case Etats.Recherche: Update_EtatRecherche(); break;
            case Etats.Combat: Update_EtatCombat(); break;
            case Etats.Mort: Update_EtatMort(); break;
        }

        // Mettre à jour l'animation
        animator.SetFloat("Vitesse", Rb.velocity.magnitude);

        // Mettre à jour la direction du sprite
        flipX();
    }

    // Fonction pour mettre à jour l'état de recherche
    void Update_EtatRecherche()
    {
        // Vérifier si le joueur est détecté, si oui, passer à l'état de combat
        if (Chercherjoueur())
        {
            etatActuel = Etats.Combat;
        }

        // Sinon, continuer à se déplacer
        transform.Translate(Vector2.right * direction * vitesse * Time.deltaTime);
        if (Mathf.Abs(transform.position.x - positionInitiale.x) >= distanceMax)
        {
            direction *= -1;
        }
    }

    // Fonction pour mettre à jour l'état de combat
    void Update_EtatCombat()
    {
        // Vérifier si le joueur est toujours détecté, sinon revenir à l'état de recherche
        Transform cible = Chercherjoueur();
        if (cible == null)
        {
            etatActuel = Etats.Recherche;
            return;
        }

        // Tenter d'attaquer le joueur
        if (Time.time >= tempsDerniereAttaque + tempsEntreAttaques)
        {
            animator.SetTrigger("Attaque");
            Attaquer(cible);
            tempsDerniereAttaque = Time.time;
        }
    }

    // Fonction pour mettre à jour l'état de mort
    void Update_EtatMort()
    {
        Destroy(gameObject);
        audioSource.PlayOneShot(sonMort);
    }

    // Fonction pour détecter le joueur
    // Renvoie la position du joueur si détecté, sinon null
    Transform Chercherjoueur()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, rayonDetection);
        float distanceMin = Mathf.Infinity;
        Transform ennemiLePlusProche = null;

        // Vérifier si le joueur est dans la zone de détection
        foreach (var collider in colliders)
        {
            // Vérifier si le collider est un joueur
            Personnage potentielEnnemi = collider.GetComponent<Personnage>();
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
        // Si le joueur est détecté, retourner sa position
        return ennemiLePlusProche;
    }

    // Fonction pour faire face à la direction du joueur
    public void flipX()
    {
        if (direction == -1)
        {
            spriteRenderer.flipX = false;
        }
        else
            spriteRenderer.flipX = true;
    }

    // Fonction pour attaquer le joueur
    void Attaquer(Transform joueur)
    {
        if (joueur == null) return;

        // Vérifier si le joueur est dans la zone de détection et calculer la direction
        Vector2 direction = (joueur.position - transform.position).normalized;
        Vector2 positionDeTir = (Vector2)transform.position + (direction * 0.7f);

        // Créer le projectile et l'initialiser 
        GameObject projectile = Instantiate(projectilePrefab, positionDeTir, Quaternion.identity);
        Projectile proj = projectile.GetComponent<Projectile>();
        proj.Initialiser(direction, null, Projectile.TypeProjectile.Directe, gameObject);

        // Jouer le son d'attaque
        audioSource.PlayOneShot(sonAttaque);
        
    }

    // Fonction pour faire subir des dégâts à l'ennemi
    // Si la vie de l'ennemi est inférieure ou égale à 0, il meurt
    // et des objets aléatoires sont générés
    public void SubirDegats(int degats)
    {
        vie -= degats;
        audioSource.PlayOneShot(sonDegats);
        if (vie <= 0)
        {
            etatActuel = Etats.Mort;
            if (Random.value < 0.40f) Instantiate(objetScore, transform.position, Quaternion.identity);
            if (Random.value < 0.10f) Instantiate(objetVie, transform.position, Quaternion.identity);
            if (Random.value < 0.05f) Instantiate(objetCoeur, transform.position, Quaternion.identity);
            if (Random.value < 0.30f) Instantiate(objetArme, transform.position, Quaternion.identity);

            int score = Random.Range(5, 20);
            joueur.score += score;
        }
    }
}
