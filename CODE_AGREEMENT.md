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