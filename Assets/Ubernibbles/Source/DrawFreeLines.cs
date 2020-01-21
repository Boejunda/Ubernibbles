// The DrawLinesTouch script adapted to work with mouse input, with the option for 3D or 2D lines
using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

namespace Ubernibbles
{
	public class DrawFreeLines : MonoBehaviour {

	public Texture2D lineTex;
	public int maxPoints = 5000;
	public float lineWidth = 2.0f;
	public int minPixelMove = 5;	// Must move at least this many pixels per sample for a new segment to be recorded
	public bool useEndCap = false;
	public Texture2D capLineTex;
	public Texture2D capTex;
	public float capLineWidth = 20.0f;
	// If line3D is true, the line is drawn in the scene rather than as an overlay. Note that in this demo, the line will look the same
	// in the game view either way, but you can see the difference in the scene view.
	public bool line3D = false;
	public float distanceFromCamera = 1.0f;
	
	private VectorLine line;
	private Vector3 previousPosition;
	private int sqrMinPixelMove;
	private bool canDraw = false;
	
	void Start () {
		float useLineWidth;
		Texture2D tex;
		if (useEndCap) {
			VectorLine.SetEndCap ("RoundCap", EndCap.Mirror, capLineTex, capTex);
			tex = capLineTex;
			useLineWidth = capLineWidth;
		}
		else {
			tex = lineTex;
			useLineWidth = lineWidth;
		}
		
		if (line3D) {
			line = new VectorLine("DrawnLine3D", new List<Vector3>(), tex, useLineWidth, LineType.Continuous, Joins.Weld);
		}
		else {
			line = new VectorLine("DrawnLine", new List<Vector2>(), tex, useLineWidth, LineType.Continuous, Joins.Weld);
		}
		line.endPointsUpdate = 2;	// Optimization for updating only the last couple points of the line, and the rest is not re-computed
		if (useEndCap) {
			line.endCap = "RoundCap";
		}
		// Used for .sqrMagnitude, which is faster than .magnitude
		sqrMinPixelMove = minPixelMove*minPixelMove;
		
		Debug.Log("Screen: " + Screen.width + ", " + Screen.height);
	}
	
	void Update () {
		var newPoint = GetMousePos();
		// Mouse button clicked, so start a new line
		if (Input.GetMouseButtonDown (0)) {
			line.points2.Clear();
			line.Draw();
			previousPosition = Input.mousePosition;
			AddPoint (newPoint);
			canDraw = true;
		}
		// Mouse button held down and mouse has moved far enough to make a new point
		else if (Input.GetMouseButton (0) && (Input.mousePosition - previousPosition).sqrMagnitude > sqrMinPixelMove && canDraw) {
			previousPosition = Input.mousePosition;
			AddPoint (newPoint);
			var pointCount = line.points2.Count;
			line.Draw();
			if (pointCount >= maxPoints) {
				canDraw = false;
			}
		}
	}


		private void AddPoint(Vector2 point)
		{
			Debug.Log("Adding point:" + point);
			line.points2.Add(point);
		}
	
	Vector3 GetMousePos () {
		var p = Input.mousePosition;
		if (line3D) {
			p.z = distanceFromCamera;
			return Camera.main.ScreenToWorldPoint (p);
		}
		return p;
	}
}

}
