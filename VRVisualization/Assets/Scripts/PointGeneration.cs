using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;

public class PointGeneration : MonoBehaviour
{
    // Start is called before the first frame update
    public float test;
    public float[,] pointPosition;
    public GameObject pointPrefab;
    public GameObject coordinatePrefab;

    Vector3 TransformFromExpectToUnity(Vector3 expectPosition)
    {
        return new Vector3(expectPosition.y, expectPosition.z, -expectPosition.x);
    }

    Vector3 TransformFromUnityToExpect(Vector3 unityPosition)
    {
        return new Vector3(-unityPosition.z, unityPosition.x, unityPosition.y);
    }

    void ReadPointData(string path)
    {
        using (StreamReader sr = new StreamReader(path))
        {
            string line;
            int rows = 0;
            int cols = 0;

            line = sr.ReadLine();
            string[] temp = Regex.Split(line, "\\s+", RegexOptions.IgnoreCase);
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
                string[] splitLine = Regex.Split(line, "\\s+", RegexOptions.IgnoreCase);
                for (int i = 0; i < splitLine.Length; ++i)
                {
                    pointPosition[count, i] = Convert.ToSingle(splitLine[i]) * 3.0f;
                }
                count++;
            }

        }
    }

    void Start()
    {
        ReadPointData("Assets\\ModelData\\DTLZ1.3D.pf");

        //for(int i = 0;i < 20;++i)
        //{
        //    for(int j = 0;j < 3;++j)
        //    {
        //        Debug.Log(pointPosition[i,j]);
        //    }
        //}

        //Instantiate(coordinatePrefab, transform.position, Quaternion.identity, transform);

        //Vector3 expectOrigin = TransformFromUnityToExpect(transform.position);
        //pointPosition = new float[4, 3]{ { 0.1f,0.1f,0},{ 0.1f, 0.1f, 0.1f },{ 0.1f, 0.2f, 0.1f },{ 0.2f, 0.2f, 0.2f } };
        //pointPosition[0, 0] = 1;
        //pointPosition[0, 1] = 1;
        //pointPosition[0, 2] = 0;
        int row = pointPosition.GetLength(0);
        int col = pointPosition.GetLength(1);

        Vector3 unityPosition = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 expectPosition = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 tempPosition = new Vector3(0.0f, 0.0f, 0.0f);
        for(int i = 0;i < row;++i)
        {
            for (int j = 0; j < col; ++j)
                tempPosition[j] = pointPosition[i,j];
            //expectPosition = expectOrigin + tempPosition;

            unityPosition = TransformFromExpectToUnity(tempPosition);

            Instantiate(pointPrefab, transform.TransformPoint(unityPosition), Quaternion.identity, transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
