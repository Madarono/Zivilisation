using UnityEngine;
using System.Collections.Generic;

public class GridTaken : MonoBehaviour
{
    public int width = 2;
    public int height = 2;

    public HashSet<Vector2Int> placesTaken = new HashSet<Vector2Int>();

    // void Start() // - Controlled by GridManager.cs
    // {
    //     MeasureGridTaken();
    // }

    public void MeasureGridTaken()
    {
        placesTaken.Clear();

        int startX = Mathf.RoundToInt(transform.position.x);
        int startY = Mathf.RoundToInt(transform.position.y);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int tilePos = new Vector2Int(startX + x, startY + y);
                
                placesTaken.Add(tilePos);
            }
        }

        foreach (var place in placesTaken)
        {
            // Debug.Log($"{gameObject.name} Takes: {place}", gameObject);
            GridManager.instance.placesTaken.Add(place);
        }
    }

    public void PutUnwalkable()
    {
        foreach(var place in placesTaken)
        {
            GridManager.instance.PlaceBuilding(place.x, place.y);
        }
    }

    public void RevertUnwalkable()
    {
        foreach(var place in placesTaken)
        {

            GridManager.instance.placesTaken.Remove(place);
        }
        // GridManager.instance.places(place.x, place.y - 1);
    }
}