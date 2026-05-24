using UnityEngine;

public class CamaraColision : MonoBehaviour
{
    private float minDistancia = 1;
    private float maxDistancia = 5;
    private float suavidad = 10;
    private float distancia;

    Vector3 direccion;

    private void Start()
    {
        
        direccion = transform.localPosition.normalized;
        distancia = transform.localPosition.magnitude;
    }

    private void Update()
    {
       
        Vector3 posDeCamara = transform.parent.TransformPoint(direccion * maxDistancia);

        RaycastHit hit;
        
        if (Physics.Linecast(transform.parent.position, posDeCamara, out hit))
        {
            
            distancia = Mathf.Clamp(hit.distance * 0.85f, minDistancia, maxDistancia);
        }
        else
        {
            
            distancia = maxDistancia;
        }

        
        transform.localPosition = Vector3.Lerp(transform.localPosition, direccion * distancia, suavidad * Time.deltaTime);
    }
}