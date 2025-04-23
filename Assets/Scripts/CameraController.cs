using UnityEngine;

public class CameraFollowX : MonoBehaviour
{
    [SerializeField] private Transform cible;
    [SerializeField] private float vitesseSuivi = 5f;

    private float yFixe;
    private float zFixe;

    void Start()
    {
        yFixe = transform.position.y;
        zFixe = transform.position.z;
    }

    void Update()
    {
        if (cible == null) return;

        float nouvelleX = Mathf.Lerp(transform.position.x, cible.position.x, vitesseSuivi * Time.deltaTime);
        transform.position = new Vector3(nouvelleX, yFixe, zFixe);
    }
}
