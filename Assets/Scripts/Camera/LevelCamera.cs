using NUnit.Framework;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    public List<Player> playerList {  get; private set; }   
    private CinemachineCamera cinemachine;

    [SerializeField] private Transform cameraCenterPoint;
    public float minCameraSize;
    public float maxCameraSize;
    private float targetLensSize;

    private bool cameraActive;
    private bool cameraFocusOnCenter;
    private Transform player1;
    private Transform player2;
    private void Awake()
    {
        cinemachine = GetComponentInChildren<CinemachineCamera>(true);
        targetLensSize =  cinemachine.Lens.OrthographicSize;
        playerList = new List<Player>();
        EnableCamera(false);

        //EnableLimits(false);
        InvokeRepeating(nameof(UpdateCenterPointPosition), 0, 0.1f);
    }

    private void Update()
    {
        if(cameraActive == false)
            return;

        UpdateLensSizeIfNeeded();
    }

    //private void UpdateCameraStatus()
    //{
    //    playerList = PlayerManager.instance.GetPlayerList();
    //    if(playerList.Count > 1)
    //    {
    //        SetNewTarget(cameraCenterPoint);

    //        player1 = playerList[0].transform;
    //        player2 = playerList[1].transform;
    //        cameraFocusOnCenter = true;
    //    }
    //    else
    //    {
    //        cameraFocusOnCenter = false;
    //        SetNewTarget(playerList[0].transform);
    //    }
    //}
    private void UpdateCameraStatus()
    {
        playerList = PlayerManager.instance.GetPlayerList();

        if (playerList.Count >= 2)
        {
            SetNewTarget(cameraCenterPoint);

            player1 = playerList[0].transform;
            player2 = playerList[1].transform;
            cameraFocusOnCenter = true;
        }
        else if (playerList.Count == 1)
        {
            cameraFocusOnCenter = false;
            SetNewTarget(playerList[0].transform);
        }
        else
        {
            cameraFocusOnCenter = false;
            SetNewTarget(null);
        }
    }

    public void UpdateCenterPointPosition()
    {
        if (cameraActive == false)
            return;

        if (cameraFocusOnCenter == false)
            return;

        Vector3 midPoint = (player1.position + player2.position) / 2;
        cameraCenterPoint.position = midPoint;  
    }
    public void ChangeCameraLensSize(float size)
    {
        targetLensSize = size;
    }
    private void UpdateLensSizeIfNeeded()
    {
        float currentLensSize =  cinemachine.Lens.OrthographicSize;

        if (Mathf.Abs(currentLensSize - targetLensSize) > .05f)
        {
            float newLensSize = Mathf.Lerp(currentLensSize, targetLensSize, .5f * Time.deltaTime);
            cinemachine.Lens.OrthographicSize = newLensSize;
        }
        else
            cinemachine.Lens.OrthographicSize = targetLensSize;
    }
    public void EnableCamera (bool enable)
    {
        cameraActive = enable;
        cinemachine.gameObject.SetActive(enable);
    }
   
    public void SetNewTarget(Transform newTarget)
    {
        cinemachine.Follow = newTarget;
    }

    private void OnEnable()
    {
        PlayerManager.OnPlayerRespawn += UpdateCameraStatus;
        PlayerManager.OnPlayerDeath += UpdateCameraStatus;
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerRespawn -= UpdateCameraStatus;
        PlayerManager.OnPlayerDeath -= UpdateCameraStatus;
    }
}
