using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Balance : Agent
{

    [SerializeField] private Transform rodTransform;
    //[SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject rod;

    [SerializeField] private Rigidbody rodRigidbody; // Reference to the Rigidbody of the rod
    [SerializeField] private Rigidbody baseRigidbody; // Reference to the Rigidbody of the base

    Quaternion origRodRotation;
   

    public override void Initialize()
    {
        origRodRotation = rodTransform.rotation;
        // Ensure that you have references to the Rigidbody components
        rodRigidbody = rod.GetComponent<Rigidbody>();
        baseRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset the base's position and rotation
        //targetTransform.localPosition = new Vector3(0, -0.5f, Random.Range(-4.3f, 4.3f));


        baseRigidbody.velocity = new Vector3(0,0,Random.Range(-2f,2f)); // Reset linear velocity
        baseRigidbody.angularVelocity = Vector3.zero; // Reset angular velocity
        transform.localPosition = new Vector3(0, -0.5f, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);

        // Reset the rod's position and rotation
        rodRigidbody.velocity = Vector3.zero; // Reset linear velocity
        rodRigidbody.angularVelocity = Vector3.zero; // Reset angular velocity
        rodTransform.localPosition = new Vector3(0, 2.0f, 0);
        rodTransform.localRotation = origRodRotation;
    }

    public override void CollectObservations(VectorSensor sensor)
    //what it can "see", i.e. states of intereset
    //we are interested in the x-position of the base,
    // and the angle of the pole
    {
        sensor.AddObservation(transform.localPosition.x);

        float rotation = rodTransform.localRotation.x;
        sensor.AddObservation(rotation);       

    }

    public override void OnActionReceived(ActionBuffers actions)
    //whenever the Ml Agent takes an action, this will be called
    //i.e. tha base is moved with this
    {
        int move = actions.DiscreteActions[0];

        float movementSpeed = 3.0f;
        float moveX = 0.0f;

        // Map discrete actions: 0 = no movement, 1 = move forward, 2 = move backward
        if (move == 1)
            moveX = 1.0f; // W key
        else if (move == 2)
            moveX = -1.0f; // S key

        transform.localPosition += new Vector3(0.0f, 0.0f, moveX) * Time.deltaTime * movementSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.W))
            discreteActionsOut[0] = 1;
        else if (Input.GetKey(KeyCode.S))
            discreteActionsOut[0] = 2;
        else
            discreteActionsOut[0] = 0;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out Wall wall))
        {
            AddReward(-2.0f);
            EndEpisode();
        }
    }

}
