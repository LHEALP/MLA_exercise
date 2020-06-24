using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class CCTV : Agent
{
    Vector3 StartPos;
    Vector3 EndPos = new Vector3(0f, 12f, 4f);

    new Rigidbody rigidbody;


    public GameObject people;
    public GameObject plane;

    int score = 0;

    float moveSpeed = 2f;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void Initialize()
    {
        rigidbody = GetComponent<Rigidbody>();
        StartPos = transform.position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 disToPeople = people.transform.position - transform.position;

        sensor.AddObservation(Mathf.Clamp(disToPeople.x / 5f, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(disToPeople.z / 5f, -1f, 1f));

        Vector3 relPos = plane.transform.position - transform.position;

        sensor.AddObservation(Mathf.Clamp(relPos.x / 5f, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(relPos.z / 5f, -1f, 1f));

        sensor.AddObservation(Mathf.Clamp(rigidbody.velocity.x / 10f, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(rigidbody.velocity.z / 10f, -1f, 1f));
    }

    private void FixedUpdate()
    {
        RequestDecision();
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        AddReward(-0.001f);

        float horizontal = vectorAction[0];
        float vertical = vectorAction[1];

        rigidbody.AddForce(horizontal * moveSpeed, 0f, vertical * moveSpeed);
    }

    public override void OnEpisodeBegin()
    {
        Reset();
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = 0;
    }

    void ResetPeople()
    {
        Vector3 randPos = new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));

        people.transform.position = randPos + plane.transform.position;
    }

    void ResetAgent()
    {
        Vector3 randPos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));

        transform.position = randPos + plane.transform.position;

        rigidbody.velocity = Vector3.zero;

        ResetPeople();
    }

    private void Reset()
    {
        ResetAgent();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            score--;
            Debug.Log("감소 : " + score);
            AddReward(-1.0f);
            EndEpisode();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("People"))
        {
            score += 1;
            Debug.Log("증가 : " + score);
            ResetPeople();
            AddReward(1.0f);
        }
    }
}
