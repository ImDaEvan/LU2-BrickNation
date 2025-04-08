using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
   public List<ShapeData> shapeData;
   public List<Shape> shapeList;
    void Start()
    {
        foreach ( var shape in shapeList)
        {
            var shapeIndex = UnityEngine.Random.Range(0,shapeData.Count);
            shape.CreateShape(shapeData[shapeIndex]);
        }
    }
    public Shape GetCurrentSelectedShape()
    {
        foreach( Shape shape in shapeList)
        {
            if (shape.IsOnStartPos() == false && shape.IsAnyShapeCellActive())
            {
                return shape;
            }
        }
        Debug.LogError("There is no shape currently selected.");
        return null;
    }
}
