using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector = new Vector3 (20f, 0f, 0f);
    [Range(0, 1)] [SerializeField] float frequency;
    [Range(-1, 1)][SerializeField] float movementFactor;

    Vector3 startingPose;
    Vector3 currentPose;

    // Start is called before the first frame update
    void Start() {
        startingPose = transform.position;
        currentPose = startingPose;
        frequency = 0.2f;
    }

    // Update is called once per frame
    void Update() {
        movementFactor = Mathf.Sin(2 * Mathf.PI * frequency * Time.time);
        currentPose = startingPose + (movementVector * movementFactor);
        transform.position = currentPose;
    }
}
