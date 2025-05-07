using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class Slimes : MonoBehaviour
{
    enum Etats { Recherche, Combat, Mort };
    [SerializeField] private Etats etatActuel;
    [SerializeField] private float vitesse = 2f;
    [SerializeField] private float distanceMax = 3f;
    private Vector3 positionInitiale;
    private int direction = 1;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform pointDeTir;
    [SerializeField] private int vie = 50;



    public Rigidbody2D Rb { get; private set; }
    public Animator animator                { get; private set;   }
    public SpriteRenderer spriteRenderer    { get; private set; }
    private float tempsEntreAttaques = 0.4f;
    private float tempsDerniereAttaque = 0f;


    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        etatActuel = Etats.Recherche;
        positionInitiale = transform.position;

    }

    void Update()
    {
        switch (etatActuel)
        {
            case Etats.Recherche: Update_EtatRecherche(); break;
            case Etats.Combat: Update_EtatCombat(); break;
            case Etats.Mort: Update_EtatMort(); break;
        }
        animator.SetFloat("Vitesse", Rb.velocity.magnitude);
        flipX();
    }

    void Update_EtatRecherche()
    {
        if (Chercherjoueur())
        {
            etatActuel = Etats.Combat;
        }

        transform.Translate(Vector2.right * direction * vitesse * Time.deltaTime);
        if (Mathf.Abs(transform.position.x - positionInitiale.x) >= distanceMax)
        {
            direction *= -1;
        }
    }
    void Update_EtatCombat()
    {
        Transform cible = Chercherjoueur();
        if (cible == null)
        {
            etatActuel = Etats.Recherche;
            return;
        }

        if (Time.time >= tempsDerniereAttaque + tempsEntreAttaques)
        {
            animator.SetTrigger("Attaque");
            Attaquer(cible);
            tempsDerniereAttaque = Time.time;
        }
    }
    void Update_EtatMort()
    {
        Destroy(gameObject);
    }

    Transform Chercherjoueur()
    {
        float rayonDetection = 5f;    
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, rayonDetection);
        float distanceMin = Mathf.Infinity;
        Transform ennemiLePlusProche = null;
        foreach (var collider  in colliders)
        {
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
        return ennemiLePlusProche;
    }

    public void flipX()
    {
        if (direction== -1)
        {
            spriteRenderer.flipX = false;
        }
        else 
            spriteRenderer.flipX = true;
    }


    void Attaquer(Transform joueur)
    {
        if (joueur == null) return;

        Vector2 direction = (joueur.position - transform.position).normalized;

        Vector2 positionDeTir = (pointDeTir != null ? (Vector2)pointDeTir.position : (Vector2)transform.position) + direction * 0.7f;

        GameObject projectile = Instantiate(projectilePrefab, positionDeTir, Quaternion.identity);
        Projectile proj = projectile.GetComponent<Projectile>();
        proj.Initialiser(direction, null, Projectile.TypeProjectile.Directe, gameObject);
    }
   public void SubirDegats(int degats)
    {   
        vie -= degats;
        if (vie <= 0)
        {
            etatActuel = Etats.Mort;
        }
    }
}
