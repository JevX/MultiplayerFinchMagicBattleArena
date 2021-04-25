using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeDetector : MonoBehaviour
{
    [Tooltip("The shape we want to detect.")]
    public LeanShape Shape;
	public LineRenderer line;

	[Tooltip("If the finger moves this many scaled pixels, the detector will update.")]
	public float StepThreshold = 1.0f;

	[Tooltip("This allows you to specify the minimum length of each edge in the detected shape in scaled pixels.")]
	public float MinimumEdgeLength = 5.0f;

	//[Tooltip("If the finger traces an edge that exceeds this length scale, then the finger will be discarded.")]
	//[Range(1.01f, 10.0f)]
	//public float LengthThreshold = 1.5f;

	[Tooltip("If the finger strays in a direction this far from the expected heading, then the finger will be discarded.")]
	[Range(0.5f, 0.99f)]
	public float DirectionPrecision = 0.85f;

	[Tooltip("If you want to allow partial shape matches, then specify the minimum amount of edges that must be matched in the shape.")]
	public int MinimumPoints = -1;

	[Tooltip("Which direction should the shape be checked using?")]
	public DirectionType Direction = DirectionType.ForwardAndBackward;

	public enum DirectionType
	{
		Forward,
		Backward,
		ForwardAndBackward
	}

	DrawShapeData drawShapeData = new DrawShapeData();
	public class DrawShapeData
	{
		public int Index; // This is the Shape.Points index that this finger is starting from.
		public bool Reverse; // Is this finger drawing the shape in reverse?
		public List<Vector2> Points = new List<Vector2>(); // This stores the current shape this finger has drawn.
		public List<Vector2> Buffer = new List<Vector2>(); // This stores the currently buffered points that could be used to define the next edge if you continue drawing enough.

		public Vector2 EndPoint
		{
			get
			{
				return Points[Points.Count - 1];
			}
		}

		public Vector2 EndBuffer
		{
			get
			{
				return Buffer[Buffer.Count - 1];
			}
		}

		public Vector2 EndVector
		{
			get
			{
				return Points[Points.Count - 1] - Points[Points.Count - 2];
			}
		}
	}


    public void CheckShape()
    {
		drawShapeData.Index = 0;
		drawShapeData.Reverse = false;

		drawShapeData.Buffer.Clear();
		drawShapeData.Points.Clear();

		drawShapeData.Buffer.Add(line.GetPosition(0));
		drawShapeData.Points.Add(line.GetPosition(0));
		drawShapeData.Points.Add(line.GetPosition(0));

		if (Shape != null)
		{
			for (int i = 0; i < line.positionCount; i++)
            {
				TryExtend(drawShapeData, line.GetPosition(i));
			}

			HandleFingerUp();
		}
	}

	private void HandleFingerUp()
	{
		if (Shape != null && Shape.Points != null)
		{
			//Debug.Log("count " + fingerData.Points.Count);
			var minimum = Shape.ConnectEnds == true ? Shape.Points.Count + 1 : Shape.Points.Count;

			if (MinimumPoints > 0 && MinimumPoints < minimum)
			{
				minimum = MinimumPoints;
			}

			if (drawShapeData.Points.Count >= minimum)
			{
				Debug.Log("Fuck eah");
			}
		}
	}

	private bool TryExtend(DrawShapeData fingerData, Vector2 newPoint)
	{
		fingerData.Points[fingerData.Points.Count - 1] = newPoint;

		if (Vector2.Distance(fingerData.EndBuffer, newPoint) >= StepThreshold)
		{
			fingerData.Buffer.Add(newPoint);

			var lineA = fingerData.Points[fingerData.Points.Count - 2];
			var lineB = fingerData.Points[fingerData.Points.Count - 1];

			if (BufferOutOfBounds(lineA, lineB, fingerData, 0, fingerData.Buffer.Count) == true)
			{
				Debug.Log("Le false");
				return false;
			}
			else
			{
				TryPivot(fingerData);
			}
		}

		return true;
	}

	private bool BufferOutOfBounds(Vector2 lineA, Vector2 lineB, DrawShapeData fingerData, int indexA, int indexB)
	{
		for (var i = indexA; i < indexB; i++)
		{
			if (Distance(lineA, lineB, fingerData.Buffer[i]) > 20.0f)
			{
				return true;
			}
		}

		return false;
	}

	private bool TryPivot(DrawShapeData fingerData)
	{
		if (fingerData.Buffer.Count > 2)
		{
			var first = fingerData.Buffer[0];
			var last = fingerData.Buffer[fingerData.Buffer.Count - 1];
			var bestScore = -1.0f;
			//var bestScoreA      = -1.0f;
			//var bestScoreB      = -1.0f;
			var bestIndex = -1;
			var bestMiddle = default(Vector2);
			var shapePointA = Shape.GetPoint(fingerData.Index + fingerData.Points.Count - 2, fingerData.Reverse);
			var shapePointB = Shape.GetPoint(fingerData.Index + fingerData.Points.Count - 1, fingerData.Reverse);
			var shapePointC = Shape.GetPoint(fingerData.Index + fingerData.Points.Count - 0, fingerData.Reverse);
			var shapeDirectionA = (shapePointB - shapePointA).normalized;
			var shapeDirectionB = (shapePointC - shapePointB).normalized;

			for (var i = fingerData.Buffer.Count - 2; i >= 1; i--)
			{
				var middle = fingerData.Buffer[i];

				if (Vector2.Distance(first, middle) >= MinimumEdgeLength && Vector2.Distance(middle, last) >= MinimumEdgeLength)
				{
					var directionA = (middle - first).normalized;
					var directionB = (last - middle).normalized;
					var scoreA = Mathf.Max(0.0f, Vector2.Dot(directionA, shapeDirectionA));
					var scoreB = Mathf.Max(0.0f, Vector2.Dot(directionB, shapeDirectionB));
					var score = scoreA * scoreB;

					if (scoreA > DirectionPrecision && scoreB > DirectionPrecision && score > bestScore)
					{
						//bestScoreA = scoreA;
						//bestScoreB = scoreB;
						bestScore = score;
						bestIndex = i;
						bestMiddle = middle;
					}
				}
			}

			if (bestIndex > 0)
			{
				if (BufferOutOfBounds(first, bestMiddle, fingerData, 0, bestIndex) == false)
				{
					if (BufferOutOfBounds(bestMiddle, last, fingerData, bestIndex, fingerData.Buffer.Count) == false)
					{
						fingerData.Points.Insert(fingerData.Points.Count - 1, bestMiddle);

						fingerData.Buffer.Clear();

						fingerData.Buffer.Add(last);

						return true;
					}
				}
			}
		}

		return false;
	}

	private float Distance(Vector2 lineA, Vector2 lineB, Vector2 point)
	{
		var v = lineB - lineA;
		var w = point - lineA;
		var c1 = Vector2.Dot(w, v); if (c1 <= 0.0f) return Vector2.Distance(point, lineA);
		var c2 = Vector2.Dot(v, v); if (c2 <= c1) return Vector2.Distance(point, lineB);
		var b = c1 / c2;
		var Pb = lineA + b * v;

		return Vector2.Distance(point, Pb);
	}

}
