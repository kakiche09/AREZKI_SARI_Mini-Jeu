using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Transform cible;
    
    public float yFixe          { get; protected set; }
    public float zFixe          { get; protected set; }

    void Start()
    {
        yFixe = transform.position.y;
        zFixe = transform.position.z;
    }

    void Update()
    {

        if (cible == null) return;

        // Calcul de la nouvelle position en X avec la Lerp
        float nouvelleX = Mathf.Lerp(transform.position.x, cible.position.x, Time.deltaTime);
        
        // Limite la position X à 0, la caméra ne pourra pas dépasser la position X de 0
        nouvelleX = Mathf.Max(nouvelleX, -1f);

        // Met à jour la position de la caméra
        transform.position = new Vector3(nouvelleX, yFixe, zFixe);
    }
}
