using UnityEngine;
using UnityEngine.EventSystems;

public class UI_UpdateFirstSelcted : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}
