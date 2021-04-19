# Installation Guide

Development Unity Version: **2019.4.1f1**

Please download the **2019.4.1** from below Unity website if concerning the Unity version:

https://unity3d.com/get-unity/download/archive

## Demo website

https://iphauman.github.io/Digital-Learning-Assistant/


## Step 1

Create an empty 2D Unity project

![img](https://iphauman.github.io/game-based-digital-learning-assistant/images/clip_image002.png)

## Step 2

Copy the `TagManager.asset` into `/created_project_folder/ProjectSettings`



## Step 3

Import the `Digital_Learning_Assistant.unitypackage` into the project 

![image-20210419164742054](https://iphauman.github.io/game-based-digital-learning-assistant/images/image-20210419164742054.png)

## Step 4

Add Scenes into the Project

![img](https://iphauman.github.io/game-based-digital-learning-assistant/images/clip_image004.png)

![img](https://iphauman.github.io/game-based-digital-learning-assistant/images/clip_image005.png)

![img](https://iphauman.github.io/game-based-digital-learning-assistant/images/clip_image007.png)

![img](https://iphauman.github.io/game-based-digital-learning-assistant/images/clip_image009.png)

![img](https://iphauman.github.io/game-based-digital-learning-assistant/images/clip_image011.png)

## Step 5

Open the `Login` scene from the Project folder by double click it and start the game.



## Step 6

In case of publicity of the GitHub repository, I removed the endpoint of the firebase online database. It requires to edit the variable `endpoint` script `/Scripts/Firebase/FirebaseRest.cs` inside the project.



```c#
endpoint = ""
```



## Additional Information

* If you want to see the changes of the database, you need to create a firebase account from https://firebase.google.com/ and create a Realtime Database to replace the REST API endpoint in the C#script `/Scripts/Firebase/FirebaseRest.cs

* In case of failure importing package, you may access the GitHub repository in https://github.com/iphauman/Digital-Learning-Assistant to download the source code. 



