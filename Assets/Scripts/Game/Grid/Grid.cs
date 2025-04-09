using System;
using System.Collections.Generic;
using Models;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UIElements;

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
    
    public GameSettings settings = new GameSettings()
    {
        scorePerLine = 10,
        scoreMultiplier2Lines=2f,
        scoreMultiplier3Lines=10f,
        initialComboMultiplier=1.1f,
        comboProgression=5.5f,
        comboDeter = 4
    };


    private Vector2 _offset = new Vector2(0.0f,0.0f);
    private List<GameObject> _gridCells = new List<GameObject>();
    private LineDetector _lineDetector;
    private int moveCount = 0;
    private int comboCount = 0;
    private int lastMoveClear = -2;

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
            CheckClearableCombinations();
        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }
       
    }

    private void CheckClearableCombinations()
    {
        moveCount++;
        List<int[]> lines = new List<int[]>();
        
        //add columns to lines List
        foreach (var column in _lineDetector.columnIndexes)
        {
            lines.Add(_lineDetector.GetVerticalLine(column));
        }
        //add rows to lines List
        for (var row = 0; row<9;row++)
        {
            lines.Add(_lineDetector.GetHorizontalLine(row));
        }
        for (var square = 0; square<9;square++)
        {
           int[] data = new int[9];
            for(var index=0; index<9;index++)
            {
                data[index]= _lineDetector.square_data[square,index];
            }
            lines.Add(data);
        }

        var completedLines = CheckClearableSquares(lines);
        
        float scoreToAdd = 0;

        Debug.Log(completedLines);
        if (completedLines>=1)
        {
            scoreToAdd = completedLines*settings.scorePerLine;
            scoreToAdd *= settings.initialComboMultiplier + (float)Math.Pow(comboCount, 0.5);
            if (moveCount-lastMoveClear<=settings.comboDeter)
            {
                comboCount += completedLines;
            }
            else
            {
                comboCount=completedLines-1;
            }
            lastMoveClear = moveCount;
        }
        else if (moveCount - lastMoveClear > settings.comboDeter)
        {
            comboCount = 0;
        }
        int add = Mathf.RoundToInt(scoreToAdd);
        GameEvents.AddScore(add);
        GameEvents.UpdateCombo(comboCount);
        CheckPlayerLost();
    }
    
    private int CheckClearableSquares(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();

        int linesCompleted = 0;
        foreach (var line in data)
        {
            var lineCompleted = true;
            foreach ( var squareIndex in line)
            {
                var comp = _gridCells[squareIndex].GetComponent<GridCell>();
                if (comp.CellOccupied == false)
                {
                    lineCompleted = false;
                }
            }
            if (lineCompleted)
            {
                completedLines.Add(line);
            }
        }
        foreach(var line in completedLines)
        {
            var lineCompleted = false;
            foreach (var index in line)
            {
                var comp = _gridCells[index].GetComponent<GridCell>();
                comp.DeactivateCell();
                lineCompleted = true;
            }
            foreach (var index in line)
            {
                var comp = _gridCells[index].GetComponent<GridCell>();
                comp.ClearCell();
                lineCompleted = true;
            }
            if (lineCompleted) 
            {
                linesCompleted++;
            }
        }
        return linesCompleted;
    }
    private void CheckPlayerLost()
    {
        var validShapes = 0;
        for (var index = 0; index < shapeStorage.shapeList.Count;index++)
        {
            var isShapeActive = shapeStorage.shapeList[index].IsAnyShapeCellActive();
            if(CheckShapePlaceability(shapeStorage.shapeList[index]) && isShapeActive)
            {
                shapeStorage.shapeList[index]?.ActivateShape();
                validShapes++;
            }

        }
        if (validShapes == 0)
        {
            GameEvents.GameOver(false);
            Debug.Log("Game Over.");
        }
    }
    private bool CheckShapePlaceability(Shape shape)
    {
        var currentShapeData = shape.CurrentShapeData;
        var shapeColumns = currentShapeData.columns;
        var shapeRows = currentShapeData.rows;
        List<int> filledSquares = new List<int>();
        var squareIndex = 0;
        for (int rowIndex=0;rowIndex<shapeRows;rowIndex++)
        {
            for (int columnIndex=0;columnIndex<shapeColumns;columnIndex++)
            {
                if (currentShapeData.board[rowIndex].column[columnIndex])
                {
                    filledSquares.Add(squareIndex);
                }
                squareIndex++;
            }
        }
        if (shape.TotalCellsInShape != filledSquares.Count)
        {

        }
        var squareList = GetAllSquareCombinations(shapeColumns,shapeRows);

        bool canBePlaced = false;
        foreach (var num in squareList )
        {
            bool shapeCanbePlaced = true;
            foreach(var squareIndexCheck in filledSquares)
            {
                var comp = _gridCells[num[squareIndexCheck]].GetComponent<GridCell>();
                if (comp.CellOccupied)
                {
                    shapeCanbePlaced = false;
                }
            }
            if (shapeCanbePlaced)
            {
                canBePlaced = true;
            }
        }
        return canBePlaced;

    }
    private List<int[]> GetAllSquareCombinations(int columns,int rows)
    {
        var squareList= new List<int[]>();
        var lastColumnIndex = 0;
        var lastRowIndex = 0;
        
        int safeIndex = 0;

        while(lastRowIndex + (rows-1) < 9)
        {
            var rowData = new List<int>();
            for (var row=lastRowIndex;row < lastRowIndex + rows; row++ )
            {
                for (var column=lastColumnIndex;column < lastColumnIndex + columns; column++ )
                {
                    rowData.Add(_lineDetector.line_data[row,column]);
                }
            }
            squareList.Add(rowData.ToArray());
            lastColumnIndex++;
            if (lastColumnIndex + (columns-1) >= 9)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }
            safeIndex++;
            if (safeIndex > 100)
            {
                break;
            }
        }   
        return squareList;
    }
}
