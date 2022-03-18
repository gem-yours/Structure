using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static void Generate(Vector2 center, int column, int row, GameObject glassObject)
    {
        var size = glassObject.GetComponent<SpriteRenderer>().size;

        for(int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                var position = Vector2.Scale(new Vector2(x, y) - new Vector2(column, row) / 2, size);
                Instantiate(glassObject, position + center, Quaternion.identity);
            }
        }
    }
}
