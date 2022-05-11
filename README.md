# VRVisualization

![image-20220511144101993](image/image-20220511144101993.png)

VRVisualization is a VR visualization tool for evolutionary algorithm optimization results. The development is based on Unity2019.4.0f1.



## Usage

After opening the project with Unity,  there is sample scene 'VRVisualization' in **'/Scenes'** folder. Go to that scene, the most important object is 'Scatterplot':

![image-20220511143026768](image/image-20220511143026768.png)

It is the visualization prefabs for any 3D data. Open the inspector of 'Plotter' in the sub hierarchy of 'Scatterplot':

![image-20220511143316062](image/image-20220511143316062.png)

Notice the properties exposed by the 'PointRenderer' script. User can the file path to the position file, color file and scale file. Some other properties can also be changed as the function denoted by their names.

As the 'Scatterplot' is a prefab, it can be reused very easily.



## Control

When connect VR to the PC and run the project. Users can grab, rotate, move the plotter anyway they want by the controllers.

Some basic move and teleport functions are also provided. 