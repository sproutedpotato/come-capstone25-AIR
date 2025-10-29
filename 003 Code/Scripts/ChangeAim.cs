using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeAim : MonoBehaviour
{
    [SerializeField] private RectTransform aimRectTransform;
    [SerializeField] private Canvas canvas;

    private Vector2 touchPos, screenPos;

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began){
                touchPos = touch.position;
                if(!IsPointerOverUIObject(touchPos)){
                    MoveAimPosition(touchPos);
                }
            }
        }
        
        if(Input.GetMouseButtonDown(0)){
            screenPos = Input.mousePosition;
            if(!IsPointerOverUIObject(screenPos)){
                MoveAimPosition(screenPos);
            }
        }
    }

    private void MoveAimPosition(Vector2 vec){
        aimRectTransform.anchoredPosition = Vector2.zero;

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(aimRectTransform, vec, canvas.worldCamera, out localPos);

        aimRectTransform.anchoredPosition = localPos;
    }

    private bool IsPointerOverUIObject(Vector2 vec){
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = vec;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                return true; // UI 버튼이 있는 곳에서는 에임이 이동하지 않음
            }
        }

        return false; // UI 버튼이 아니면 에임 이동
    }
}
