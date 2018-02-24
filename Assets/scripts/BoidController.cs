using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{

    //create public controls here
    public GameObject m_boidPrefab;

    public GameObject m_searchArea;

    public LayerMask m_searchLayer;

    public LayerMask m_POILayer;

    [Range(5.0f, 20.0f)]
    public int m_spawnCount = 8;

    [Range(4.0f, 20.0f)]
    public float m_spawnRadius =10.0f;  

    [Range(0.1f, 20.0f)]
    public float m_velocity = 6.0f;

    [Range(4.0f, 20.0f)]
    public float m_neighbourDistance = 4.0f;

    [Range(1.0f, 2.0f)]
    public float m_velocityVariation = 1.2f;

    [Range(10f, 100.0f)]
    public float m_rotationDiffusion = 10.0f;

    // private variables here.
    private float m_controllerVelocity = 5.0f;
    Vector3 randomLocation = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 mouse = new Vector3(0.0f, 0.0f, 0.0f);
    List<Boids> myBoids = new List<Boids>();
    Vector3 m_controllerDirection;
    float noiseOffset;

    [Range(10f, 100.0f)]
    public float changeLocationTimer = 10.0f;
    //Getters and Setters.
    public float Velocity
    {
        get { return m_controllerVelocity; }
        set { m_controllerVelocity = value; }
    }


    void Start()
    {
        //spawn boids here.
        for (int i = 0; i < m_spawnCount; i++)
        {
            Spawn();
        }
        m_controllerDirection = Vector3.forward;
        noiseOffset = Random.value * 10.0f;
    }

    void Update()
    {
        Debug.Log(myBoids.Count);
        m_controllerVelocity = 0.0f;
        m_controllerDirection = Vector3.forward;
        float noise = Mathf.PerlinNoise(Time.time, noiseOffset) * 2.0f - 1.0f;

        foreach (Boids b in myBoids)
        {
            m_controllerVelocity += b.Velocity;
            m_controllerDirection += b.Direction;
        }
        m_controllerVelocity += m_velocity * (1.0f + noise);
        m_controllerVelocity /= (myBoids.Count + 1);


        //COME BACK TO THIS - Trying to get a \/\/\/\ movement along the foward vector. 
        /*
        float wave = Mathf.Sin(Time.time);
        Vector3 rotation2 = new Vector3(0, wave * Time.deltaTime , 0);
        transform.Rotate(rotation2);
        */
        changeLocationTimer -= Time.deltaTime;
        if (changeLocationTimer <= 0.0f)
        { 
            Vector3 areaPosition = m_searchArea.transform.position;
            Vector3 areaScale = m_searchArea.transform.localScale;
            Vector3 max = areaPosition + areaScale;
            Vector3 min = areaPosition - areaScale;
            randomLocation = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
            changeLocationTimer = 10.0f;
        }
        /////////////!MOUSE POINT AND CLICK!/////////////////////
        /*
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
            }
        }
        m_controllerDirection += ((mouse - transform.position));
        */
        /////////////!MOUSE POINT AND CLICK!/////////////////////
        m_controllerDirection += ((randomLocation - transform.position));

        // m_controllerDirection /= ( myBoids.Count );
        m_controllerDirection = m_controllerDirection.normalized;
        Quaternion newRotation = Quaternion.FromToRotation(Vector3.forward, m_controllerDirection);
        if (transform.rotation != newRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, m_rotationDiffusion * Time.deltaTime);
        }


        //Constantly move towards and past the point of interest.  
        transform.position = transform.position + transform.forward * (m_controllerVelocity * Time.deltaTime);
    }


    public GameObject Spawn()

    {
        return Spawn(Random.insideUnitSphere * m_spawnRadius);

    }



    public GameObject Spawn(Vector3 position)

    {

        Quaternion rotation = new Quaternion(0, 0, 0,0);

        GameObject boid = Instantiate(m_boidPrefab, position, rotation) as GameObject;

        boid.GetComponent<Boids>().controller = this;
        myBoids.Add(boid.GetComponent<Boids>());


        return boid;

    }
}
