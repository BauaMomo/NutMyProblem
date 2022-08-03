using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    GameObject Platform;
    GameObject Player;
    playerController pCon;

    bool isLeverActive = false;

    Vector2 PathStartPoint;
    Vector2 PathEndPoint;

    Vector2 posChange;

    public float lerpPos;
    [SerializeField] float platformSpeed;
    float localSpeed = -1;

    public bool playerOnPlatform;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        pCon = Player.GetComponent<playerController>();

        Platform = transform.Find("Platform").gameObject;
        PathStartPoint = transform.Find("PathStartPoint").localPosition;
        PathEndPoint = transform.Find("PathEndPoint").localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lerpPos = Mathf.Clamp(lerpPos, 0, 1);
        if (isLeverActive || transform.Find("Lever") == null)
            if (lerpPos >= 1 || lerpPos <= 0) localSpeed *= -1;
        lerpPos += localSpeed * 0.02f * platformSpeed * (1 / Vector2.Distance(PathStartPoint, PathEndPoint));

        playerOnPlatform = Platform.GetComponent<BoxCollider2D>().IsTouchingLayers(1 << LayerMask.NameToLayer("Player"));

        if (playerOnPlatform && pCon.isGrounded)
        {
            if(pCon.moveDir == 0)   Player.transform.position += new Vector3(posChange.x, posChange.y, 0);
            else Player.transform.position += new Vector3(0, posChange.y, 0);
        }

        Vector2 newPos = Vector2.Lerp(PathStartPoint, PathEndPoint, lerpPos);

        posChange =  newPos - (Vector2)Platform.transform.localPosition;
        Platform.transform.localPosition = newPos;
    }

    public void OnLeverFlip(bool _isActive)
    {
        isLeverActive = _isActive;
    }
}
