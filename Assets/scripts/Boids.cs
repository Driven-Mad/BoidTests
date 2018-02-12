using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    public BoidController controller;
    public float animationSpeedVariation = 0.2f;
    int identifier;


    Vector3 mouse = new Vector3(0.0f, 0.0f, 0.0f);
    float randomDestination;

    // Use this for initialization

   

    float noiseOffset;
    private float velocity;
    public float Velocity
    {
        get
        {
            return velocity;
        }

        set
        {
            velocity = value;
        }
    }

    void Start()
    {
        noiseOffset = Random.value * 10.0f;
        var animator = GetComponent<Animator>();

        if (animator)
        {
            animator.speed = Random.Range(-1.0f, 1.0f) * animationSpeedVariation + 1.0f;
        }
        randomDestination = Random.Range(3.0f, 3.0f + controller.m_destinatiinVariation);
        identifier = GetInstanceID();
    }



    // Update is called once per frame
    void Update()
    {
        //create local variables to get transform inforation. 
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        // Initializes the 3 vectors for the 3 reynolds rules of boid systems
        Vector3 separation = Vector3.zero;
        Vector3 alignment = controller.transform.forward;
        Vector3 cohesion = controller.transform.position;
               

        //get some random noise. 
        float noise = Mathf.PerlinNoise(Time.time, noiseOffset) * 2.0f - 1.0f;
     
        //Slowly rotate towards where the mouse points to (No snap)

        //Get an array of all the nearby boids.
        Collider[] nearbyBoids = Physics.OverlapSphere(currentPosition, controller.m_neighbourDistance, controller.m_searchLayer);
        int neighbours = nearbyBoids.Length;
        foreach (Collider b in nearbyBoids)
        {
            //
            if (b.gameObject == gameObject) continue;
            Vector3 neighbourDirection = currentPosition - b.transform.position;
            float neighbourDistance = neighbourDirection.magnitude;
            float scaler = Mathf.Clamp01(1.0f - neighbourDistance / controller.m_neighbourDistance);
            separation += (neighbourDirection * scaler / neighbourDistance);
            //Debug.Log("separation");
            // add all boids forward vectors together. 
            alignment += b.transform.forward;
            //Get all the positions and add them together
            cohesion += b.transform.position;
        }
        float average = 1.0f / neighbours;
        cohesion *= average;
        alignment *= average;
        cohesion = (cohesion - currentPosition).normalized;

        Vector3 direction = separation + alignment + cohesion;


        Quaternion newRotation = Quaternion.FromToRotation(Vector3.forward, direction );
        if (currentRotation != newRotation)
        {
            transform.rotation = Quaternion.RotateTowards(currentRotation, newRotation,controller.m_rotationDiffusion * Time.deltaTime);
        }

       // Debug.Log(identifier + " HAS " + nearbyBoids.Length + " NEIGHBOURS");
        /* if (Vector3.Distance(currentPosition, mouse) >= randomDestination)
         {
             float velocity = controller.m_velocity * (1.0f + noise * controller.m_velocityVariation);
             //While the distance between the currentposition and where the mouse clicked
             //If the transform isn't facing the direction turn towards the direction
             if (transform.forward != direction)
             {

                 //Debug.Log("TURNING");
                 Vector3 tpos = currentPosition + (direction + transform.right.normalized) * (velocity * Time.deltaTime);
                 transform.position = tpos;
             }
             else
             {
                //    Debug.Log("FORWARD");
                 Vector3 tpos = currentPosition + direction * (velocity * Time.deltaTime);
                 transform.position = tpos;
             }
         }*/
       
        velocity = controller.m_velocity * (1.0f + noise * controller.m_velocityVariation);
        transform.position  = currentPosition + transform.forward * (velocity * Time.deltaTime);


    }
}
