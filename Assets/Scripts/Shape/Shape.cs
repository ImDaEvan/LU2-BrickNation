using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Scripting.APIUpdating;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler,IEndDragHandler,IDragHandler,IPointerDownHandler 
{
    public GameObject squareShapeImage;
    public Vector3 selectedScale;
    public Vector2 offset = new Vector2(0f,1000f);

    [HideInInspector]

    public ShapeData CurrentShapeData;

    public int TotalCellsInShape {get;set;}

    private List<GameObject> _currentShape = new List<GameObject>();
    private Vector3 _shapeStartScale;
    private RectTransform _transform;
    private bool _shapeDraggable = true;
    private Canvas _mainFrame;
    private Vector3 _startPosition;
    private bool _shapeActive = true;
   
    public void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _mainFrame = GetComponentInParent<Canvas>();
        _shapeDraggable = true;
        _startPosition = _transform.localPosition;
        _shapeActive = true;
    }
    private void OnEnable()
    {
        GameEvents.MoveShapeToStartPosition += MoveShapeToStartPosition;
        GameEvents.DiminishShapeControls += DiminishShapeControls;
    }

    private void OnDisable()
    {
        GameEvents.MoveShapeToStartPosition -= MoveShapeToStartPosition;
        GameEvents.DiminishShapeControls -= DiminishShapeControls;
    }
    public bool IsOnStartPos()
    {
        return (_transform.localPosition == _startPosition);
    }
    public bool IsAnyShapeCellActive()
    {
        foreach (var cell in _currentShape)
        {
            if(cell.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
    private void DiminishShapeControls()
    {
        if (IsOnStartPos() == false && IsAnyShapeCellActive())
        {
            foreach(var cell in _currentShape)
            {
                cell.gameObject.SetActive(false);
            }
        }
    }
    public void DeactivateShape()
    {
        if (_shapeActive)
        {
            foreach (var cell in _currentShape)
            {
                cell?.GetComponent<ShapeSquare>().DeactivateCell();
            }
        }

        _shapeActive = false;
    }
    public void ActivateShape()
    {
        if (!_shapeActive)
        {
            foreach (var cell in _currentShape)
            {
                cell?.GetComponent<ShapeSquare>().ActivateCell();
            }
        }
        
        _shapeActive = true;
    }
    public void RequestNewShape(ShapeData shapeData)
    {
        _transform.localPosition = _startPosition;
        CreateShape(shapeData);
    }
    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        TotalCellsInShape = GetNumberOfCells(shapeData);

        while(_currentShape.Count < TotalCellsInShape) //instantiate all needed cells
        {
            GameObject addedShape = Instantiate(squareShapeImage,transform) as GameObject;
            addedShape.name = shapeData.name;
            _currentShape.Add(addedShape);
        }
        foreach(var square in _currentShape)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }
        var squareRect = squareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x,
        squareRect.rect.height * squareRect.localScale.y);

        int currentIndexInList = 0;
        //set positions
        for(var row = 0; row < shapeData.rows; row++)
        {
            for (var column = 0; column < shapeData.columns; column++)
            {
                if (shapeData.board[row].column[column])
                {
                    _currentShape[currentIndexInList].SetActive(true);
                    _currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition = CalculatePosForCell(shapeData,column,row,moveDistance);
                    currentIndexInList++;
                }
            }
        }
    }
    private Vector2 CalculatePosForCell(ShapeData shapeData, int column,int row, Vector2 moveDistance)
    {
        // Vector2 pos;
        // pos.x = CalculateXPositionForCell(shapeData,column,moveDistance);
        // pos.y = CalculateYPositionForCell(shapeData,row,moveDistance);
        // return pos;
        float centerX = (shapeData.columns - 1) / 2f;
        float centerY = (shapeData.rows - 1) / 2f;

        float x = (column - centerX) * moveDistance.x;
        float y = -(row - centerY) * moveDistance.y; // Flip Y for UI layout if needed

        return new Vector2(x, y);
    }
    // private float CalculateYPositionForCell(ShapeData shapeData, int row, Vector2 moveDistance)
    // {
    //     float shiftOnY = 0f;

    //     if (shapeData.rows >= 1)
    //     {
    //         if (shapeData.rows % 2 != 0 )
    //         {
    //             var middleCellIndex = (shapeData.rows - 1) / 2;
    //             var multiplier = (shapeData.rows - 1) / 2;

    //             if (row < middleCellIndex)
    //             {
    //                 shiftOnY = moveDistance.y;
    //                 shiftOnY *= multiplier;
    //             }
    //             else if (row > middleCellIndex)
    //             {
    //                 shiftOnY = moveDistance.y * -1;
    //                 shiftOnY *= multiplier;
    //             }

    //         }
    //         else
    //         {
    //             var middleCellIndex1 =  (shapeData.rows ==2 ) ? 1 : (shapeData.rows / 2);
    //             var middleCellIndex2 = (shapeData.rows ==2 ) ? 0 : (shapeData.rows - 1);
    //             var multiplier = shapeData.rows / 2;

    //             if(row == middleCellIndex2)
    //             {
    //                 shiftOnY = moveDistance.y/2;
    //             }
    //             else if (row == middleCellIndex1)
    //             {
    //                 shiftOnY = (moveDistance.y / 2) * -1;
    //             }
                
    //             if( row < middleCellIndex1 && row < middleCellIndex2)
    //             {
    //                 shiftOnY = moveDistance.y;
    //                 shiftOnY *= multiplier;
    //             }
    //             else if( row > middleCellIndex1 && row > middleCellIndex2)
    //             {
    //                 shiftOnY = moveDistance.y * -1;
    //                 shiftOnY *= multiplier;
    //             }
    //         }
    //     }
    //     return shiftOnY;
    // }
    // private float CalculateXPositionForCell(ShapeData shapeData, int column, Vector2 moveDistance)
    // {
    //     float shiftOnX = 0f;

    //     if(shapeData.columns >= 1)
    //     {
    //         if(shapeData.columns % 2 != 0)//shape has a middle that we can place.
    //         {
    //             var middleCellIndex = (shapeData.columns - 1) / 2;
    //             var multiplier = (shapeData.columns - 1) / 2;
    //             if( column < middleCellIndex)
    //             {
    //                 shiftOnX = moveDistance.x * -1;
    //                 shiftOnX *= multiplier;
    //             }
    //             else if( column > middleCellIndex)
    //             {
    //                 shiftOnX = moveDistance.x;
    //                 shiftOnX *= multiplier;
    //             }
    //         }
    //         else 
    //         {
    //             var middleCellIndex1 =  (shapeData.columns ==2 ) ? 1 : (shapeData.columns / 2);
    //             var middleCellIndex2 = (shapeData.columns ==2 ) ? 0 : (shapeData.columns - 1);
    //             var multiplier = shapeData.columns / 2;

    //             if(column == middleCellIndex2)
    //             {
    //                 shiftOnX = moveDistance.x/2;
    //             }
    //             else if (column == middleCellIndex1)
    //             {
    //                 shiftOnX = (moveDistance.x / 2) * -1;
    //             }

    //             if( column < middleCellIndex1 && column < middleCellIndex2)
    //             {
    //                 shiftOnX = moveDistance.x * -1;
    //                 shiftOnX *= multiplier;
    //             }
    //             else if( column > middleCellIndex1 && column > middleCellIndex2)
    //             {
    //                 shiftOnX = moveDistance.x;
    //                 shiftOnX *= multiplier;
    //             }
    //         }
    //     }
    //     return shiftOnX;
    // }

    private int GetNumberOfCells(ShapeData shapeData)
    {
        int countedCells = 0;
        foreach ( var rowData in shapeData.board)
        {
            foreach( bool cellActive in rowData.column)
            {
                if (cellActive)
                {
                    countedCells++;
                }
            }
        }
        return countedCells;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
    public void OnPointerUp(PointerEventData eventData)
    {

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _transform.localScale = selectedScale;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = _shapeStartScale;
        GameEvents.CanPlaceShape();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchorMin = new Vector2(0,0);
        _transform.anchorMax = new Vector2(0,0);
        _transform.pivot = new Vector2(0,0);
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_mainFrame.transform as RectTransform,eventData.position,Camera.main,out pos);
        _transform.localPosition = pos+offset;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    private void MoveShapeToStartPosition()
    {
        _transform.transform.localPosition = _startPosition;
    }
}
