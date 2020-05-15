using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    enum GameState { Alive, Transition, Dead }

    [SerializeField] float rotationalThrust = 100f;
    [SerializeField] float mainThrust = 1600f;
    [SerializeField] AudioClip audio_mainEngine;
    [SerializeField] AudioClip audio_Death;
    [SerializeField] AudioClip audio_Success;
    [SerializeField] ParticleSystem particle_mainEngine;
    [SerializeField] ParticleSystem particle_Death;
    [SerializeField] ParticleSystem particle_Success;

    Rigidbody rocket_rigidBody;
    AudioSource rocket_audioSource;
    Vector3 landingOrientation = new Vector3(0, 0, 0);
    GameState game_state = GameState.Alive;
    float trans_delay = 2f;

    bool collisions_disabled = false;

    // Start is called before the first frame update
    void Start () {
        // Retrive rigid body object (automatically retrieves the rigidbody available for the game object the script refers to)
        rocket_rigidBody = GetComponent<Rigidbody>();
        rocket_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update () {
        if (game_state == GameState.Alive) {
            Thrust();
            Rotate();
            Land();
        }
        if(Debug.isDebugBuild) {
            PollDebugKeys();
        }
    }

    private void PollDebugKeys() {
        if (Input.GetKeyDown(KeyCode.N)) { // Jump to the next level
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            collisions_disabled = !collisions_disabled;
        }
    }

    private void Thrust() {
        // Normalize the rotational thrust according to the frame rate
        float forceMultiplier = mainThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space)) {
            rocket_rigidBody.AddRelativeForce(Vector3.up * forceMultiplier);
            if (!rocket_audioSource.isPlaying) {
                rocket_audioSource.PlayOneShot(audio_mainEngine);
            }
            particle_mainEngine.Play();
        }
        else {
            rocket_audioSource.Stop();
            particle_mainEngine.Stop();
        }
    }



    private void Rotate() {
        // Normalize the rotational thrust according to the frame rate
        float forceMultiplier = rotationalThrust * Time.deltaTime;

        // Manually control the rotation of the Rocket
        rocket_rigidBody.freezeRotation = true;

        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Rotate(Vector3.forward * forceMultiplier);
        }
        else if (Input.GetKey(KeyCode.RightArrow)) {
            transform.Rotate(-Vector3.forward * forceMultiplier);
        }

        // Let the physics engine control the rotation of the Rocket
        rocket_rigidBody.freezeRotation = false;
    }

    private void Land() {
        //Get into landing orientation

        // Manually control the rotation of the Rocket
        rocket_rigidBody.freezeRotation = true;

        if (Input.GetKey(KeyCode.DownArrow)) {
            transform.eulerAngles = landingOrientation;
        }

        // Let the physics engine control the rotation of the Rocket
        rocket_rigidBody.freezeRotation = false;
    }

    // Message: called every time the object collides with something
    void OnCollisionEnter(Collision collision) {
        // Avoid multiple INVOKE calls when not in an ALIVE state. AKA ignore collisions when dead
        if (game_state != GameState.Alive || collisions_disabled) {
            return;
        }

        switch (collision.gameObject.tag) {
            case "Hostile": // Hit an hostile obj. Destroy rocket
                game_state = GameState.Dead;
                if (rocket_audioSource.isPlaying) {
                    rocket_audioSource.Stop();
                }
                particle_Death.Play();
                rocket_audioSource.PlayOneShot(audio_Death);
                Invoke("LoadLevel", trans_delay); // Load first level after 1 second delay
                break;
            case "Fuel": // Hit a fuel refill bons obj. Refill tank
                game_state = GameState.Alive;
                break;
            case "Finish": // Hit a landing pad. Proceed to the next level
                game_state = GameState.Transition;
                if (rocket_audioSource.isPlaying) {
                    rocket_audioSource.Stop();
                }
                particle_Success.Play();
                rocket_audioSource.PlayOneShot(audio_Success);
                Invoke("LoadLevel", trans_delay); // Load next level after 1 second delay
                break;
            default: // Friendly collision. Do nothing
                game_state = GameState.Alive;
                break;
        }
    }

    private void LoadLevel() {
        switch (game_state) {
            case GameState.Dead:
                SceneManager.LoadScene(0);
                break;
            case GameState.Transition:
                SceneManager.LoadScene(1);
                break;
        }
    }

    private void LoadNextLevel() {
        print(SceneManager.sceneCount);
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        if(currentScene + 1 > SceneManager.sceneCount) {
            currentScene = -1;
        }
        SceneManager.LoadScene(currentScene + 1);
    }


}
