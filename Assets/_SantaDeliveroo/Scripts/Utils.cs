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
}
