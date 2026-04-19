using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target;

    public float baseYOffset = 0f;
    public float downOffset = 2f;
    public float upOffset = -1.5f;
    public float offsetChangeSpeed = 2f;

    public bool followX = false;
    public bool stopVerticalOffset = false;

    private float currentYOffset;

    private float shakeAmount;
    private float shakeTime;

    private float savedShakeX;
    private bool isShaking;

    private void LateUpdate()
    {
        if (target == null) return;

        float targetOffset = baseYOffset;

        if (!stopVerticalOffset)
        {
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                targetOffset = downOffset;
            }
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                targetOffset = upOffset;
            }
        }

        currentYOffset = Mathf.Lerp(currentYOffset, targetOffset, Time.deltaTime * offsetChangeSpeed);

        float x = followX ? target.position.x : transform.position.x;
        float y = target.position.y + currentYOffset;

        if (shakeTime > 0f)
        {
            shakeTime -= Time.unscaledDeltaTime;

            x = savedShakeX + Random.Range(-shakeAmount, shakeAmount);
            y += Random.Range(-shakeAmount, shakeAmount);
        }
        else if (isShaking)
        {
            x = savedShakeX;
            isShaking = false;
        }

        transform.position = new Vector3(x, y, transform.position.z);
    }

    public void Shake(float amount, float duration)
    {
        shakeAmount = amount;
        shakeTime = duration;

        savedShakeX = transform.position.x;
        isShaking = true;
    }
}