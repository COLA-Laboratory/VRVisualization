using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;


// This script gets values from CSVReader script
// It instantiates points and particles according to values read

public class PointRenderer : MonoBehaviour {

    //********Public Variables********

    // Name of the input file, no extension
    public string positionFile;
    public string colorFile;
    public string scaleFile;
    public string plotTitle;

    // Scale of the prefab particlePoints
    [Range(0.0f, 1.0f)]
    public float pointScale = 0.25f;

    // The prefab for the data particlePoints that will be instantiated
    public GameObject PointPrefab;

    // Object which will contain instantiated prefabs in hiearchy
    public GameObject PointHolder;


    // Scale of particlePoints within graph, WARNING: Does not scale with graph frame
    private float plotScale = 10;

    //********Private Variables********
    // Minimum and maximum values of columns
    private float xMin;
    private float yMin;
    private float zMin;

    private float xMax;
    private float yMax;
    private float zMax;

    // Number of rows
    private int rowCount;

    // List for holding data from CSV reader
    private List<Dictionary<string, object>> pointList;
    private float[,] pointPosition;
    private float[,] pointColor;
    private float[] pointSize;
    private float maxPointSize = 0.0f;

    void Awake()
    {
        // rend normal data
        ReadPointData(positionFile);
        ReadColorData(colorFile);
        ReadScaleData(scaleFile);

        if(pointSize!=null)
        {
            // scale normalization
            for (int i = 0;i < pointSize.Length;i++)
            {
                if (maxPointSize < pointSize[i])
                    maxPointSize = pointSize[i];
            }

            for (int i = 0; i < pointSize.Length; i++)
                pointSize[i] /= maxPointSize;
        }
    }


    // Use this for initialization
    void Start () 
	{
        // Get maxes of each axis, using FindMaxValue method defined below
        xMax = FindMaxValue(0);
        yMax = FindMaxValue(1);
        zMax = FindMaxValue(2);

        // Get minimums of each axis, using FindMinValue method defined below
        xMin = FindMinValue(0);
        yMin = FindMinValue(1);
        zMin = FindMinValue(2);

        // Assign values to the labels
        AssignLabels();
        
        // Call PlacePoint methods defined below
        PlacePrefabPoints();   
    }
    
		
	// Update is called once per frame
	void Update ()
    {
    }

    // Places the prefabs according to values read in
	private void PlacePrefabPoints()
	{
        for (var i = 0; i < pointPosition.GetLength(0); i++)
        {
            float x = (pointPosition[i,0] - xMin) / (xMax - xMin);
            float y = (pointPosition[i,1] - yMin) / (yMax - yMin);
            float z = (pointPosition[i,2] - zMin) / (zMax - zMin);

            // Create vector 3 for positioning particlePoints
            Vector3 position = new Vector3 (x, y, z) * plotScale;

			//instantiate as gameobject variable so that it can be manipulated within loop
			GameObject dataPoint = Instantiate (PointPrefab, Vector3.zero, Quaternion.identity);

            
            // Make child of PointHolder object, to keep particlePoints within container in hiearchy
            dataPoint.transform.parent = PointHolder.transform;

            // Position point at relative to parent
            dataPoint.transform.localPosition = position;

            if(pointSize!= null)
            {
                dataPoint.transform.localScale = new Vector3(pointScale * pointSize[i], pointScale * pointSize[i], pointScale * pointSize[i]);
            }
            else
            {
                dataPoint.transform.localScale = new Vector3(pointScale * 0.5f, pointScale * 0.5f, pointScale * 0.5f);
            }

            // Converts index to string to name the point the index number
            string dataPointName = i.ToString();

            // Assigns name to the prefab
            dataPoint.transform.name = dataPointName;


            if(pointColor != null)
            {
                // Sets color according to x/y/z value
                dataPoint.GetComponent<Renderer>().material.color = new Color(pointColor[i, 0], pointColor[i, 1], pointColor[i, 2], pointColor[i, 3]);

                // Activate emission color keyword so we can modify emission color
                dataPoint.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                dataPoint.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(pointColor[i, 0], pointColor[i, 1], pointColor[i, 2], pointColor[i, 3]));
            }
            else
            {
                dataPoint.GetComponent<Renderer>().material.color = new Color(0.7f,0.7f,0.7f,1.0f);
            }
        }

	}


    // Finds labels named in scene, assigns values to their text meshes
    // WARNING: game objects need to be named within scene
    private void AssignLabels()
    {
        // Update point counter
        GameObject.Find("Point_Count").GetComponent<TextMesh>().text = pointPosition.GetLength(0).ToString("0");

        // Update title according to inputfile name
        GameObject.Find("Dataset_Label").GetComponent<TextMesh>().text = plotTitle;

        // Update axis titles to ColumnNames
        GameObject.Find("X_Title").GetComponent<TextMesh>().text = "f1";
        GameObject.Find("Y_Title").GetComponent<TextMesh>().text = "f2";
        GameObject.Find("Z_Title").GetComponent<TextMesh>().text = "f3";

        // Set x Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find("X_Min_Lab").GetComponent<TextMesh>().text = xMin.ToString("0.0");
        GameObject.Find("X_Mid_Lab").GetComponent<TextMesh>().text = (xMin + (xMax - xMin) / 2f).ToString("0.0");
        GameObject.Find("X_Max_Lab").GetComponent<TextMesh>().text = xMax.ToString("0.0");

        // Set y Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find("Y_Min_Lab").GetComponent<TextMesh>().text = yMin.ToString("0.0");
        GameObject.Find("Y_Mid_Lab").GetComponent<TextMesh>().text = (yMin + (yMax - yMin) / 2f).ToString("0.0");
        GameObject.Find("Y_Max_Lab").GetComponent<TextMesh>().text = yMax.ToString("0.0");

        // Set z Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find("Z_Min_Lab").GetComponent<TextMesh>().text = zMin.ToString("0.0");
        GameObject.Find("Z_Mid_Lab").GetComponent<TextMesh>().text = (zMin + (zMax - zMin) / 2f).ToString("0.0");
        GameObject.Find("Z_Max_Lab").GetComponent<TextMesh>().text = zMax.ToString("0.0");
                
    }

    //Method for finding max value, assumes PointList is generated
    private float FindMaxValue(string columnName)
    {
        //set initial value to first value
        float maxValue = Convert.ToSingle(pointList[0][columnName]);

        //Loop through Dictionary, overwrite existing maxValue if new value is larger
        for (var i = 0; i < pointList.Count; i++)
        {
            if (maxValue < Convert.ToSingle(pointList[i][columnName]))
                maxValue = Convert.ToSingle(pointList[i][columnName]);
        }

        //Spit out the max value
        return maxValue;
    }

    private int FindMaxValue(int column)
    {
        //set initial value to first value
        float maxValue = pointPosition[0,column];

        //Loop through Dictionary, overwrite existing maxValue if new value is larger
        for (var i = 0; i < pointPosition.GetLength(0); i++)
        {
            if (maxValue < pointPosition[i, column])
                maxValue = pointPosition[i, column];

        }

        //Spit out the max value
        return (int)(maxValue + 1.0f);
    }

    //Method for finding minimum value, assumes PointList is generated
    private float FindMinValue(string columnName)
    {
        //set initial value to first value
        float minValue = Convert.ToSingle(pointList[0][columnName]);

        //Loop through Dictionary, overwrite existing minValue if new value is smaller
        for (var i = 0; i < pointList.Count; i++)
        {
            if (Convert.ToSingle(pointList[i][columnName]) < minValue)
                minValue = Convert.ToSingle(pointList[i][columnName]);
        }

        return minValue;
    }

    private float FindMinValue(int column)
    {
        //set initial value to first value
        float minValue = pointPosition[0, column];

        //Loop through Dictionary, overwrite existing maxValue if new value is larger
        for (var i = 0; i < pointPosition.GetLength(0); i++)
        {
            if (minValue > pointPosition[i, column])
                minValue = pointPosition[i, column];

        }

        //Spit out the min value
        return minValue;
    }


    void ReadPointData(string path)
    {
        using (StreamReader sr = new StreamReader(path))
        {
            string line;
            int rows = 0;
            int cols = 0;

            line = sr.ReadLine();
            string[] temp = Regex.Split(line, "[\\s,;]+", RegexOptions.IgnoreCase);
            rows++;
            cols = temp.Length;
            // 从文件读取并显示行，直到文件的末尾 
            while ((line = sr.ReadLine()) != null)
            {
                rows++;
            }

            pointPosition = new float[rows, cols];


            sr.BaseStream.Seek(0, SeekOrigin.Begin);

            int count = 0;
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitLine = Regex.Split(line, "[\\s,;]+", RegexOptions.IgnoreCase);
                for (int i = 0; i < splitLine.Length; ++i)
                {
                    pointPosition[count, i] = Convert.ToSingle(splitLine[i]);
                }
                float temp_data = pointPosition[count, 1];
                pointPosition[count, 1] = pointPosition[count, 2];
                pointPosition[count, 2] = temp_data;
                count++;
            }

        }
    }

    void ReadColorData(string path)
    {
        if (path.Length != 0)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                int rows = 0;
                int cols = 0;

                line = sr.ReadLine();
                string[] temp = Regex.Split(line, "[\\s,;]+", RegexOptions.IgnoreCase);
                rows++;
                cols = temp.Length;
                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    rows++;
                }

                pointColor = new float[rows, cols];

                sr.BaseStream.Seek(0, SeekOrigin.Begin);

                int count = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] splitLine = Regex.Split(line, "[\\s,;]+", RegexOptions.IgnoreCase);
                    for (int i = 0; i < splitLine.Length; ++i)
                    {
                        pointColor[count, i] = Convert.ToSingle(splitLine[i]);
                    }
                    count++;
                }

            }
        }
    }

    void ReadScaleData(string path)
    {
        if (path.Length != 0)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                int rows = 0;

                line = sr.ReadLine(); rows++;
                while ((line = sr.ReadLine()) != null)
                {
                    rows++;
                }
                pointSize = new float[rows];


                sr.BaseStream.Seek(0, SeekOrigin.Begin);

                int count = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    pointSize[count] = Convert.ToSingle(line);
                    pointSize[count] = Mathf.Pow(pointSize[count], 0.33333f);
                    count++;
                }

            }
        }
    }
}


