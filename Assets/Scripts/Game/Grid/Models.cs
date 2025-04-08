using UnityEngine;
namespace Models{
    public class GridSize
    {
        public int columns {get;set;}
        public int rows {get;set;}
        public GridSize(int ccolumns, int crows)
        {
            columns = ccolumns;
            rows = crows;
        }
    }
}