using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    public BoidController controller;
    public float animationSpeedVariation = 0.5f;

    // Use this for initialization
    
    float noiseOffset;
    //Velocity getters and setters 
    private float m_velocity;
    private Vector3 m_direction;
    public Vector3 Direction
    {
        get { return m_direction; }
        set { m_direction = value; }
    }
    public float Velocity
    {
        get { return m_velocity; }
        set { m_velocity = value; }
    }

    void Start()
    {
        noiseOffset = Random.value * 10.0f;
        var animator = GetComponent<Animator>();

        if (animator)
        {
            animator.speed = Random.Range(-1.0f, 1.0f) * animationSpeedVariation + 1.0f;
        }
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

        //Get an array of all the nearby boids based on the layer set in the controller.
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

        m_direction = separation + alignment + cohesion;


        Quaternion newRotation = Quaternion.FromToRotation(Vector3.forward, m_direction);
        if (currentRotation != newRotation)
        {
            transform.rotation = Quaternion.RotateTowards(currentRotation, newRotation, controller.m_rotationDiffusion * Time.deltaTime);
        }
       
        m_velocity = controller.m_velocity * (1.0f + noise * controller.m_velocityVariation);
        transform.position  = currentPosition + transform.forward * (m_velocity * Time.deltaTime);


    }
}
