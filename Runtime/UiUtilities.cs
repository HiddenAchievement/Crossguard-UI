using UnityEngine;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// Utility functions to help UI navigation.
    /// </summary>
    public static class UiUtilities
    {
	    enum Corner { SW, NW, NE, SE };
	    
	    
	    private static Vector3[] _corners = new Vector3[4];

	    public static void FitRectTransform(RectTransform element, RectTransform target)
	    {
		    target.GetWorldCorners(_corners);
		    element.position = new Vector3(
			    (_corners[(int)Corner.NE].x + _corners[(int)Corner.NW].x) * 0.5f,
			    (_corners[(int)Corner.SW].y + _corners[(int)Corner.NW].y) * 0.5f,
			    0);
		    Vector3 scale = element.lossyScale;
		    element.sizeDelta = new Vector2(
			    (_corners[(int)Corner.NE].x - _corners[(int)Corner.NW].x) / scale.x, 
			    (_corners[(int)Corner.NW].y - _corners[(int)Corner.SW].y) / scale.y);
	    }

	    public static void EnsureItemVisible(ScrollRect scroller, RectTransform item)
		{
			Rect viewportWorldRect = GetWorldRect(scroller.viewport);
			Rect itemWorldRect = GetWorldRect(item);
			
			Vector3 adjustment = Vector3.zero;

			if (viewportWorldRect.xMin > itemWorldRect.xMin)
			{
				adjustment.x = viewportWorldRect.xMin - itemWorldRect.xMin;
			}
			else if (viewportWorldRect.xMax < itemWorldRect.xMax)
			{
				adjustment.x = viewportWorldRect.xMax - itemWorldRect.xMax;
			}

			if (viewportWorldRect.yMin > itemWorldRect.yMin)
			{
				adjustment.y = viewportWorldRect.yMin - itemWorldRect.yMin;
			}
			else if (viewportWorldRect.yMax < itemWorldRect.yMax)
			{
				adjustment.y = viewportWorldRect.yMax - itemWorldRect.yMax;
			}
			
			if (adjustment == Vector3.zero) return; // Return if there's nothing to do.

			scroller.content.position += adjustment;
		}
		
		static public Rect GetWorldRect(RectTransform rt)
		{
			// Convert the rectangle to world corners and grab the top left
			rt.GetWorldCorners(_corners);
			Vector3 topLeft = _corners[0];
			Vector3 bottomRight = _corners[2];
 
			// Rescale the size appropriately based on the current Canvas scale
			// Vector2 scaledSize = new Vector2(scale.x * rt.rect.size.x, scale.y * rt.rect.size.y);
 
			return new Rect(topLeft, new Vector2(bottomRight.x - topLeft.x, bottomRight.y - topLeft.y));
		}
    }
}
