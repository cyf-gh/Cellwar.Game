# CellWar.Game
Game for iGEM.
This game is used in HumanPractices part in our iGEM project. After iGEM, we can polish this game and make it for other uses. Meanwhile, this game is about the war of cells.

See [TODO List](<https://github.com/bennycui99/Cellwar.Game/blob/master/TODO.md>) and fuck your tasks off! LOL

## Where the hell we are?
### 9.8 MapEditor
When holding a bacteria/ chemical you can not active/de-active a block.
Left Click to active/ right click to de-active.
Left Click multiple times can change the capacity of a block.

Left Click to add to block(can add multiple tiems)/Right Click to discard the holding.
Z to discard last bacteria/ X to discard last chemical.

Bacteria reads from Resources/Save/strain.json; Chemicals reads from Resources/GameData/chemicals.json.
Export to Resources/Save/map_generation.json

GameScene reads map from Resources/GameData/map.json

### 9.6 Implement Video Manager - Fade between scenes

The in-game fade in animation remain unimplemented, but the API is easy to use.
![avatar](Progress/9.6.gif)


### 9.4 Smooth Game Animations

![avatar](Progress/9.4.gif)



### 7.30 Finished Lab

![avatar](Progress/7.30.gif)

## About CellWar Json Files In GameData

We use excel to edit raw data and for 2json, you can use [THIS SITE](<http://www.bejson.com/json/col2json/>)

to convert the excel to json file.

see **CellWar.Document/game_data.xlsx** file to make sense about the core game data of CW.

## C# Coding/Scripting Agreement

### Namespace

All game c# logic should be wrote in namespace like

```c#
namespace CellWar {
    namspace Foo {
        ...
    }
}
```

or

```c#
namespace CellWar.Foo {
    
}
```



### Class Members

```
m[MemberName]

eg.
Block mHexBlock;
```

### Property

```
[Capital]xxxx

eg.
int Length { get; set; }
```

### Class Name & Namespace Name

```
both [Capital]xxxx

eg.
namespace CellWar.Model.Foo {
    public class FooClass {
        ...
    }
}
```



### MVC

The project obeys the **MVC** pattern, whose **Model** refers to the CellWar.Model namespace. **View** refers to all the unity scripts attached to the unity game object.

#### Model

which refers to the [model.cs](<https://github.com/bennycui99/Cellwar.Game/blob/master/CellWar.Game/Assets/Scripts/Model.cs>) file for the time being.

#### View

which refers to all the unity scripts attached to the unity game object.

All unity script, which is attached to the game object, should be named as

```
U3D_[ScriptName].cs

eg
U3D_CameraLogic.cs
```

All unity class should be put in namespace like

```c#
namespace CellWar.View {
    class U3D_FooLogic : MonoBehavior {
        ...
    }
}
```



#### Controller

be left vacant for the time being.

The controller behavior is integreted into the model so far.



## Directories

The repository contains following folders in root directory.

* **CellWar.Game** - Unity3d project.
* **CellWar.Document** - Literal documents about the designing of the game.



## FAQs

### Haste cloning & pulling

For the fucking previous commits were not using gitignore file, it might takes you a fucking shit long time to clone the whole repository. So you can clone like this.

#### Clone

~~~shell
$ git clone --depth=1 https://github.com/bennycui99/iGEM-game.git
~~~

Or you want pull the latest change.

#### Pull

```shell
$ git pull --depth=1
```

Or a lazier way.

```shell
$ . pull.sh
```



### Haste Unity Starting Up

You should install [ppbash](<http://github.com/cyf-gh/ppbash>) first enable to use the [go] command.

```shell
$ . open_scene.sh
```

For powershell,

```powershell
PS >. open_scene.ps1
```

