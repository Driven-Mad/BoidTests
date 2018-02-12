using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{

    //create global controls here
    public GameObject m_boidPrefab;

    public LayerMask m_searchLayer;

    public int m_spawnCount = 10;

    public float m_spawnRadius = 4.0f;

    [Range(5.0f, 60.0f)]
    public float m_destinatiinVariation = 6.0f;

    [Range(0.1f, 20.0f)]
    public float m_velocity = 6.0f;

    [Range(4.0f, 20.0f)]
    public float m_neighbourDistance = 4.0f;

    [Range(1.0f, 2.0f)]
    public float m_velocityVariation = 1.2f;

    [Range(0.001f, 2.0f)]
    public float m_rotationDiffusion = 10.0f;

    public float m_controllerVelocity = 5.0f;

    Vector3 mouse = new Vector3(0.0f, 0.0f, 0.0f);
    List<Boids> myBoids = new List<Boids>();

    void Start()
    {
        //spawn boids here.
        for (int i = 0; i < m_spawnCount; i++)
        {
            Spawn();
        }
         

    }

    void Update()
    {
        Debug.Log(myBoids.Count);
        m_controllerVelocity = 0.0f;
        foreach (Boids b in myBoids)
        {
            m_controllerVelocity += b.Velocity;
        }

       
        m_controllerVelocity /= myBoids.Count;


        //COME BACK TO THIS - Trying to get a \/\/\/\ movement along the foward vector. 
        /*
        float wave = Mathf.Sin(Time.time);
        Vector3 rotation2 = new Vector3(0, wave * Time.deltaTime , 0);
        transform.Rotate(rotation2);
        */

      

        if (Input.GetMouseButton(0))
        {
            //Cast a ray
            RaycastHit move;
            //between camera (tagged as main) and where the mouse clicks (best to put a "Floor" down)
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out move))
            {
                //get the point of where the mouse hits.
                mouse = move.point;
                //set the direction to be where we want to go near the mouse.
            }
        }
        Vector3 newDirection = (mouse - transform.position);
        newDirection = newDirection.normalized;
         Quaternion newRotation = Quaternion.FromToRotation(transform.forward, newDirection);
         if (transform.rotation != newRotation)
         {
             transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 0.3f);
         }


        //Constantly move forward for now. 
        //transform.position += (transform.forward * (m_controllerVelocity * Time.deltaTime));
        transform.position += (newDirection * (m_controllerVelocity * Time.deltaTime));
    }


    public GameObject Spawn()

    {
        return Spawn(Random.insideUnitSphere * m_spawnRadius);

    }



    public GameObject Spawn(Vector3 position)

    {

        Quaternion rotation = new Quaternion(0, 0, 0,0); // Quaternion.Slerp(transform.rotation, Random.rotation, m_rotationDiffusion);

        GameObject boid = Instantiate(m_boidPrefab, position, rotation) as GameObject;

        boid.GetComponent<Boids>().controller = this;
        myBoids.Add(boid.GetComponent<Boids>());


        return boid;

    }
}
