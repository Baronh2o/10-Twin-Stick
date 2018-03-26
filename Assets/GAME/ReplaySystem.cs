using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaySystem : MonoBehaviour {
    private const int bufferFrames = 1000;
    private MyKeyFrame[] keyFrames = new MyKeyFrame[bufferFrames];

    private GameManager gameManager;
    private Rigidbody rigidBody;

    private bool recordToggle;  // This tracks when user switches between Record and Playback
    private int startKeyFrame;  // Frame recording begin at
    private int totalKeyFrame;  // How many frames were recorded
    private int counterKeyFrame; // Current position in Playback


    // Use this for initialization
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        recordToggle = !gameManager.recording;
    }


    // Update is called once per frame
    void Update() {
        if (gameManager.recording) {
            Record();
        } else {
            Playback();
        }
    }

    private void Playback() {
        if (recordToggle != gameManager.recording)  // Started playback
        {
            recordToggle = gameManager.recording;
            rigidBody.isKinematic = true;

            int endKeyFrame = Time.frameCount;
            if ((endKeyFrame - startKeyFrame) >= bufferFrames) // Recorded past buffer limit.  Playback entire buffer
            {
                startKeyFrame = endKeyFrame + 1;
                totalKeyFrame = bufferFrames;
            } else  // Only playback recorded portion
              {
                totalKeyFrame = endKeyFrame - startKeyFrame;
            }

            counterKeyFrame = 0;
        }

        int keyFrame = (startKeyFrame + counterKeyFrame) % bufferFrames;
        Debug.Log("keyFrames[" + keyFrame + "].keyTime = " + keyFrames[keyFrame].frameTime);

        transform.position = keyFrames[keyFrame].position;
        transform.rotation = keyFrames[keyFrame].rotation;

        counterKeyFrame++;
        if (counterKeyFrame >= totalKeyFrame) // Replay from beginning if exceeded recorded portion
        {
            counterKeyFrame = 0;
        }
    }


    private void Record() {
        if (recordToggle != gameManager.recording)  // Started recording
        {
            recordToggle = gameManager.recording;
            rigidBody.isKinematic = false;
            startKeyFrame = Time.frameCount;
        }

        int keyFrame = Time.frameCount % bufferFrames;
        keyFrames[keyFrame] = new MyKeyFrame(Time.time, transform.position, transform.rotation);
    }

}

/// <summary>
/// A structure for storing time, rotation and position.
/// </summary>
public struct MyKeyFrame {
    public float frameTime;
    public Vector3 position;
    public Quaternion rotation;

    public MyKeyFrame(float aTime, Vector3 aPosition, Quaternion aRotation) {
        frameTime = aTime;
        position = aPosition;
        rotation = aRotation;
    }
}
