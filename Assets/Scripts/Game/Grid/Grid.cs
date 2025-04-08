using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int columns = 9;
    public int rows = 9;
    public float cellGap = 0.9f;
    public GameObject gridCell;
    public Vector2 startPosition = new Vector2(0.0f,0.0f);
    public float cellScale = 0.5f;
    public float everyCellOffset = 0.0f;

    private Vector2 _offset = new Vector2(0.0f,0.0f);
    private List<GameObject> _gridCells = new List<GameObject>();
    private LineDetector _lineDetector;
    private int moveCount = 0;


    void Start()
    {
        _lineDetector = GetComponent<LineDetector>();
        CreateGrid();
    }
    public Models.GridSize GetGridSize()
    {
        Models.GridSize gridSize = new Models.GridSize(columns,rows);
        return gridSize;
    }
    private void OnEnable()
    {
        GameEvents.CanPlaceShape += CanPlaceShape;
    }
    private void OnDisable()
    {
         GameEvents.CanPlaceShape -= CanPlaceShape;
    }
    private void CreateGrid()
    {
        SpawnGridCells();
        SetGridCellPositions();

    }

    private void SpawnGridCells()
    {
        int cellIndex = 0;
        for (var row=0; row<rows; ++row)
        {
            for (var column = 0; column<columns; ++column)
            {
                _gridCells.Add(Instantiate(gridCell) as GameObject);

                _gridCells[_gridCells.Count-1].transform.SetParent(this.transform);

                _gridCells[_gridCells.Count-1].transform.localScale = new Vector3(cellScale,cellScale,cellScale);

                _gridCells[_gridCells.Count-1].GetComponent<GridCell>().SetImage(_lineDetector.GetGridSquareIndex(cellIndex) % 2 ==0);
                _gridCells[_gridCells.Count-1].GetComponent<GridCell>().CellIndex = cellIndex;
                cellIndex++;

            }
        }
    }
    private void SetGridCellPositions()
    {
        int columnNum = 0;
        int rowNum = 0;
        Vector2 cellGapNum = new Vector2(0.0f,0.0f);
        bool rowMoved = false;

        var squareRect = _gridCells[0].GetComponent<RectTransform>();
        _offset.x = squareRect.rect.width * squareRect.transform.localScale.x + everyCellOffset;
        _offset.y = squareRect.rect.height * squareRect.transform.localScale.y + everyCellOffset;
        foreach (GameObject cell in _gridCells)
        {
            if (columnNum >= columns)
            {
                cellGapNum.x = 0;

                columnNum = 0;
                rowNum++;
                rowMoved = false;
            }
            var posOffsetX = _offset.x * columnNum + (cellGapNum.x * cellGap);
            var posOffsetY = _offset.y * rowNum + (cellGapNum.y * cellGap);

            if (columnNum > 0 && columnNum % 4 == 0)
            {
                cellGapNum.x++;
                posOffsetX += cellGap;
            }
            if (rowNum > 0 && rowNum % 4 ==0 && !rowMoved)
            {
                rowMoved = true;
                cellGapNum.y++;
                posOffsetY += cellGap;
            }
            
            cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                startPosition.x + posOffsetX,
                startPosition.y-posOffsetY
                );
            cell.GetComponent<RectTransform>().localPosition = new Vector3(
                startPosition.x + posOffsetX,
                startPosition.y-posOffsetY,
                0.0f
                );

            columnNum++;

        }
        

    }

    private void CanPlaceShape()
    {
        var cellIndexes = new List<int>();
        foreach (var cell in _gridCells)
        {
            var gridCell = cell.GetComponent<GridCell>();
            if (gridCell.Selected && !gridCell.CellOccupied)
            {
                cellIndexes.Add(gridCell.CellIndex);
                gridCell.Selected = false;
            }
        }
        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null) return;
        
        if(currentSelectedShape.TotalCellsInShape == cellIndexes.Count)
        {
            foreach ( var cellIndex in cellIndexes)
            {
                _gridCells[cellIndex].GetComponent<GridCell>().PlaceShapeOnBoard();
                moveCount++;
            }

            var shapesLeft = 0;

            foreach (Shape shape in shapeStorage.shapeList)
            {
                if (shape.IsOnStartPos() && shape.IsAnyShapeCellActive())
                {
                    shapesLeft++;
                }
            }
            if (shapesLeft == 0)
            {
                GameEvents.RequestNewShapes();
            }
            else
            {
                GameEvents.DiminishShapeControls();
            }
        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }
       
    }
   
}
