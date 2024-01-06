using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class RodBalanceAgent : Agent
{
    [SerializeField] private Transform baseTransform;
    [SerializeField] private Rigidbody baseRigidbody; // Reference to the Rigidbody of the base

    private Rigidbody rodRigidbody;
    private Transform rodTransform;
    private Quaternion origRodRotation;
    

    public override void Initialize()
    {
        rodRigidbody = GetComponent<Rigidbody>();
        rodTransform = GetComponent<Transform>();
        origRodRotation = rodTransform.rotation;
    }

    public override void OnEpisodeBegin()
    {
        // Reset the base's position and rotation     

        baseRigidbody.velocity = new Vector3(0, 0, 0); // Reset linear velocity
        baseRigidbody.angularVelocity = Vector3.zero; // Reset angular velocity
        baseTransform.localPosition = new Vector3(0, -0.5f, 0);
        baseTransform.localRotation = Quaternion.Euler(0, 0, 0);

        // Reset the rod's position and rotation
        rodRigidbody.velocity = new Vector3(0,0,Random.Range(-3f,3f)); // Reset linear velocity
        rodRigidbody.angularVelocity = Vector3.zero; // Reset angular velocity
        rodTransform.localPosition = new Vector3(0, 2.0f, 0);
        rodTransform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(baseTransform.localPosition.z);

        float rotation = transform.localRotation.x;
        sensor.AddObservation(rotation);

        float angVel = rodRigidbody.angularVelocity.x;
        sensor.AddObservation(angVel);

        //add some noise
        float moveX = Random.Range(-0.2f,0.2f);
        baseRigidbody.velocity += new Vector3(0.0f, 0.0f, moveX);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0]; // Continuous action for movement

        float movementSpeed = 6.0f;

        baseTransform.localPosition += new Vector3(0.0f, 0.0f, moveX) * Time.deltaTime * movementSpeed;
        //baseRigidbody.velocity += new Vector3(0.0f, 0.0f, moveX);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical"); // Use Vertical axis for continuous input
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Wall wall))
        {
            AddReward(-2.0f);
            EndEpisode();
        }
        if (collision.gameObject.TryGetComponent(out Target target))
        {
            AddReward(10.0f);
            if(GetCumulativeReward() > 100f)
            {
                EndEpisode();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Wall wall))
        {
            AddReward(-5.0f);
            EndEpisode();
        }
       else if (other.TryGetComponent(out Target target))
        {
            AddReward(2.0f);
            if (GetCumulativeReward() > 15.0f)
            {
                EndEpisode();
            }
        }
    }

}
