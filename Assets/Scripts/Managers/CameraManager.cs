using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [Header("ScreenShake")]
    [SerializeField] private Vector2 shakeVelocity;

    private CinemachineImpulseSource impulseSource;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void ScreenShake(float shakeDirection)
    {
        impulseSource.DefaultVelocity = new Vector2(shakeVelocity.x * shakeDirection, shakeVelocity.y);
        impulseSource.GenerateImpulse();
    }

}
