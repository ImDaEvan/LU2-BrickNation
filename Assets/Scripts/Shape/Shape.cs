using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting.APIUpdating;

public class Shape : MonoBehaviour
{
    public GameObject squareShapeImage;

    [HideInInspector]

    public ShapeData CurrentShapeData;

    private List<GameObject> _currentShape = new List<GameObject>();
    void Start()
    {
    }
    public void RequestNewShape(ShapeData shapeData)
    {
        CreateShape(shapeData);
    }
    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        var totalSquares = GetNumberOfCells(shapeData);

        while(_currentShape.Count <= totalSquares) //instantiate all needed cells
        {
            _currentShape.Add(Instantiate(squareShapeImage,transform) as GameObject);
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
        Vector2 pos;
        pos.x = CalculateXPositionForCell(shapeData,column,moveDistance);
        pos.y = CalculateYPositionForCell(shapeData,row,moveDistance);
        return pos;
    }
    private float CalculateYPositionForCell(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0f;

        if (shapeData.rows>1)
        {
            if (shapeData.rows % 2 != 0 )
            {
                var middleCellIndex = (shapeData.rows - 1) / 2;
                var multiplier = (shapeData.rows - 1) / 2;

                if (row < middleCellIndex)
                {
                    shiftOnY = moveDistance.y;
                    shiftOnY *= multiplier;
                }
                else if (row > middleCellIndex)
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }

            }
            else
            {
                var middleCellIndex1 =  (shapeData.rows ==2 ) ? 1 : (shapeData.rows / 2);
                var middleCellIndex2 = (shapeData.rows ==2 ) ? 0 : (shapeData.rows - 1);
                var multiplier = shapeData.rows / 2;

                if(row == middleCellIndex2)
                {
                    shiftOnY = moveDistance.y/2;
                }
                else if (row == middleCellIndex1)
                {
                    shiftOnY = (moveDistance.y / 2) * -1;
                }
                
                if( row < middleCellIndex1 && row < middleCellIndex2)
                {
                    shiftOnY = moveDistance.y;
                    shiftOnY *= multiplier;
                }
                else if( row > middleCellIndex1 && row > middleCellIndex2)
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }
            }
        }
        return shiftOnY;
    }
    private float CalculateXPositionForCell(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        float shiftOnX = 0f;

        if(shapeData.columns > 1)//vertical position calculate
        {
            if(shapeData.columns % 2 != 0)//shape has a middle that we can place.
            {
                var middleCellIndex = (shapeData.columns - 1) / 2;
                var multiplier = (shapeData.columns - 1) / 2;
                if( column < middleCellIndex)
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if( column > middleCellIndex)
                {
                    shiftOnX = moveDistance.x;
                    shiftOnX *= multiplier;
                }
            }
            else 
            {
                var middleCellIndex1 =  (shapeData.columns ==2 ) ? 1 : (shapeData.columns / 2);
                var middleCellIndex2 = (shapeData.columns ==2 ) ? 0 : (shapeData.columns - 1);
                var multiplier = shapeData.columns / 2;

                if(column == middleCellIndex2)
                {
                    shiftOnX = moveDistance.x/2;
                }
                else if (column == middleCellIndex1)
                {
                    shiftOnX = (moveDistance.x / 2) * -1;
                }

                if( column < middleCellIndex1 && column < middleCellIndex2)
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if( column > middleCellIndex1 && column > middleCellIndex2)
                {
                    shiftOnX = moveDistance.x;
                    shiftOnX *= multiplier;
                }
            }
        }
        return shiftOnX;
    }

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
}
