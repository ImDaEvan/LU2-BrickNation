using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public int columns = 9;
    public int rows = 9;
    public float cellGap = 0.9f;
    public GameObject gridCell;
    public Vector2 startPosition = new Vector2(0.0f,0.0f);
    public float cellScale = 0.5f;
    public float everyCellOffset = 0.0f;

    private Vector2 _offset = new Vector2(0.0f,0.0f);
    private List<GameObject> _gridCells = new List<GameObject>();



    void Start()
    {
        CreateGrid();
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

                _gridCells[_gridCells.Count-1].GetComponent<GridCell>().SetImage(cellIndex % 2 ==0);
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
}
