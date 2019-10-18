using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Utils {
    public class UIHelper {
        public static bool IsOverUI(Vector2 point) {
            EventSystem current = EventSystem.current;
            if (current == null) return false;
            PointerEventData eventDataCurrentPosition = new PointerEventData(current);
            eventDataCurrentPosition.position = new Vector2(point.x, point.y);
            List<RaycastResult> results = new List<RaycastResult>();
            current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public static GameObject GetElementUnder(Vector2 point) {
            EventSystem current = EventSystem.current;
            if (current == null) return null;
            PointerEventData eventDataCurrentPosition = new PointerEventData(current);
            eventDataCurrentPosition.position = new Vector2(point.x, point.y);
            List<RaycastResult> results = new List<RaycastResult>();
            current.RaycastAll(eventDataCurrentPosition, results);
            // DEBUG DEPTH (results are ordered by it, which means pretty much which one is in the front).
            /*foreach(RaycastResult rs in results) {
                Debug.Log("NAME: " + rs.gameObject.name + "DEPTH: " + rs.depth + "DISTANCE: " + rs.distance);
            }*/
            if (results.Count > 0) return results[0].gameObject;
            return null;
        }
    }
}
