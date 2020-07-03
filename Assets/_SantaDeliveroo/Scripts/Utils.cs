using System.Collections.Generic;
using UnityEngine;
public class Utils
{
    static public bool NearlyZero(float value)
    {
        if (value < -Mathf.Epsilon || value > Mathf.Epsilon)
        {
            return false;
        }

        return true;
    }

    static public bool NearlyZero(Vector2 vector)
    {
        if (NearlyZero(vector.x) == false)
        {
            return false;
        }
        if (NearlyZero(vector.y) == false)
        {
            return false;
        }

        return true;
    }

    static public bool NearlyZero(Vector3 vector)
    {
        if (NearlyZero(vector.x) == false)
        {
            return false;
        }
        if (NearlyZero(vector.y) == false)
        {
            return false;
        }
        if (NearlyZero(vector.z) == false)
        {
            return false;
        }

        return true;
    }

    static public bool Equals<T>(List<T> first, List<T> second)
    {
        if (first.Count != second.Count)
        {
            return false;
        }

        foreach (T item in second)
        {
            if (first.Contains(item) == false)
            {
                return false;
            }
        }

        return true;
    }

    static public void AddPosition(LineRenderer lineRenderer, Vector3 position)
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount + 1];
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            positions[i] = lineRenderer.GetPosition(i);
        }

        positions[positions.Length - 1] = position;

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}
