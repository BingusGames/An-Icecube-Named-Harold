using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipYRotationTime = 0.5f;

    private Coroutine _turnCoroutine;

    private Haroldthe4thScript _player;

    private bool _IsFacingRight;

    private void Awake()
    {
        _player = _playerTransform.gameObject.GetComponent<Haroldthe4thScript>();

        _IsFacingRight = _player.isFacingRight;
    }

    private void FixedUpdate()
    {
        //make the FollowCameraObject follow the player's position
        transform.position = _playerTransform.position;
    }

    public void CallFlip()
    {
        //_turnCoroutine = StartCoroutine(FlipYLerp());

        LeanTween.rotateY(gameObject, DetermineEndRotation(), _flipYRotationTime).setEaseInOutSine();

    }

    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float yRotation = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < _flipYRotationTime)
        {
            elapsedTime += Time.deltaTime;

            //lerp the y rotation
            yRotation = Mathf.Lerp(startRotation, endRotationAmount, (elapsedTime / _flipYRotationTime));
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            yield return null;
        }
    }

    private float DetermineEndRotation()
    {
        _IsFacingRight = !_IsFacingRight;

        if (_IsFacingRight)
        {
            return 180f;
        }

        else
        {
            return 0f;
        }
    }

}


