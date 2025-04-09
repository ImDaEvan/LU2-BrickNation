using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
public class LineDetector : MonoBehaviour
{
   //data system to detect lines 
    public int[,] line_data = new int[9,9]
    {
        {0,1,2,     3,4,5,      6,7,8},
        {9,10,11,   12,13,14,   15,16,17},
        {18,19,20,  21,22,23,   24,25,26},

        {27,28,29,  30,31,32,   33,34,35},
        {36,37,38,  39,40,41,   42,43,44},
        {45,46,47,  48,49,50,   51,52,53},

        {54,55,56,  57,58,59,   60,61,62},
        {63,64,65,  66,67,68,   69,70,71},
        {72,73,74,  75,76,77,   78,79,80}

    };
    public int[,] square_data = new int[9,9]
    {
        {0,1,2,9,10,11,18,19,20},//square 1-9
        {3,4,5,12,13,14, 21,22,23},
        {6,7,8,15,16,17,24,25,26},
        {27,28,29,36,37,38,45,46,47},
        {30,31,32,39,40,41,48,49,50},
        {33,34,35,42,43,44,51,52,53},
        {54,55,56,63,64,65,72,73,74},
        {57,58,59,66,67,68, 75,76,77 },
        {60,61,62,69,70,71,78,79,80},
    };
    [HideInInspector]
    public int[] columnIndexes = new int[9]
    {
        0,1,2,3,4,5,6,7,8,
    };

    private (int, int) GetCellPosition(int index)
    {
        int pos_row = -1;
        int pos_col = -1;

        for (int row = 0; row<9;row++)
        {
             for (int col = 0; col<9;col++)
            {
                if (line_data[row,col] == index)
                {
                    pos_row = row;
                    pos_col = col;
                }
            }
        }
        return (pos_row,pos_col);
    }
    public int[] GetVerticalLine(int column)
    {
        int[] line = new int[9];
        int cellPosCol = GetCellPosition(column).Item2;
        for (column = 0; column<9; column++)
        {
            line[column] = line_data[column,cellPosCol];
        }
        return line;
    }
    public int[] GetHorizontalLine(int row)
    {
        int[] line = new int[9];
        for (var index = 0; index < 9; index++)
        {
            line[index] = line_data[row, index];
        }
        return line;
    }

    public int GetGridSquareIndex(int square)
    {
        for (int row = 0; row<9; row++)
        {
            for (int column = 0; column<9; column++)
            {
                if (square_data[row,column] == square)
                {
                    return row;
                }
            }
        }
        return -1;
    }
    void Start()
    {



    }

}   
