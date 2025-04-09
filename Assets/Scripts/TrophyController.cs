using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.EventSystems;
public class TrophyController : MonoBehaviour, IBeginDragHandler,IEndDragHandler,IDragHandler,IPointerDownHandler 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created;
    private RectTransform _transform;
    private Vector3 selectedScale = new Vector3(0.3f,0.3f,0.3f);
    public Vector3 _trophyStartScale = new Vector3(0.3f,0.3f,0.3f);
    private Vector3 _trophyStartPos;
     public GameObject trophySelection,trophyDropField;
    private Canvas _mainFrame;
    public int PrefabID;
    public bool draggable = true;
    private List<GameObjectServer> placedObjects = new List<GameObjectServer>();
    void Start()
    {
        _mainFrame = GetComponentInParent<Canvas>();
        _transform = GetComponent<RectTransform>();
       // _trophyStartScale = _transform.localScale;
        _trophyStartPos = _transform.localPosition;
    }
     public void OnBeginDrag(PointerEventData eventData)
    {
        if (draggable)
        {_transform.localScale = selectedScale;}
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = _trophyStartScale;
        CanPlaceTrophy();
        

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggable)
        {
        _transform.anchorMin = new Vector2(0,0);
        _transform.anchorMax = new Vector2(0,0);
        _transform.pivot = new Vector2(0,0);
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_mainFrame.transform as RectTransform,eventData.position,Camera.main,out pos);
        _transform.localPosition = pos;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    private void CanPlaceTrophy()
    {
        if ((_transform.position.y - _transform.rect.height/2 < trophySelection.transform.position.y + trophySelection.GetComponent<RectTransform>().rect.height/2) && draggable == true)
        {
            draggable = false;
            transform.SetParent(trophyDropField.transform); 
            GameEvents.PlaceNewTrophy(_transform.gameObject.name);
            GameEvents.SaveTrophyState(transform);
            
        }
        else{
            MoveShapeToStartPosition();
        }
    }
    private void MoveShapeToStartPosition()
    {
        _transform.transform.localPosition = _trophyStartPos;
    }
    
}
