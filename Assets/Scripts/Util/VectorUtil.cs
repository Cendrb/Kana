using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class VectorUtil
    {
        public static float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
        {
            float angle = Mathf.DeltaAngle(Mathf.Atan2(vec1.y, vec1.x) * Mathf.Rad2Deg,
                                Mathf.Atan2(vec2.y, vec2.x) * Mathf.Rad2Deg);
            return NormalizeAngle(angle);
        }

        public static float NormalizeAngle(float angle)
        {
            if (angle < 0)
                return 180 + Mathf.Abs(angle);
            else
                return angle;
        }
    }
}
