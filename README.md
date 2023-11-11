# SQLite4Unity

A simple SQLite ORM that is compatible with Unity (tested on Unity 2019.4.40f1).

## How to Use

### Defining Table Schema

To define a table schema, create a new public class, with public properties corresponding to each column and an empty constructor.

```csharp
[SQLiteTable("npc_spawn")]
public class NpcSpawn
{
    [Column("SpawnID", PrimaryKey = true)]
    public int SpawnID { get; set; }
    public int TemplateID { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }
    public float RotX { get; set; }
    public float RotY { get; set; }
    public float RotZ { get; set; }

    public NpcSpawn() {}
}
```

The `[SQLiteTable("tablename")]` attribute defines the table that this class refers to and is required.
The `[Column("columnname")]` attribute is optional, and specifies which column a given variable represents, with the `PrimaryKey = true` parameter specifying primary keys. If no `[Column(...)]` attributes are used, the column name will be identical to the property name.

If using the Column attribute for every property, the `NpcSpawn` class could look like this:

```csharp
[SQLiteTable("npc_spawn")]
public class NpcSpawn
{
    [Column("SpawnID", PrimaryKey = true)]
    public int SpawnID { get; set; }
    [Column("TemplateID")]
    public int TemplateID { get; set; }
    [Column("PosX")]
    public float PosX { get; set; }
    [Column("PosY")]
    public float PosY { get; set; }
    [Column("PosZ")]
    public float PosZ { get; set; }
    [Column("RotX")]
    public float RotX { get; set; }
    [Column("RotY")]
    public float RotY { get; set; }
    [Column("RotZ")]
    public float RotZ { get; set; }

    public NpcSpawn() {}
}
```

### Retrieving Data

Once you have your schema defined, accessing the data should be done from within a `MonoBehaviour` using a `DbSet` for each table:

```csharp
public class DatabaseTest : MonoBehaviour
{
    DbSet<NpcTest> npcTestHolder;

    void Awake()
    {
        npcTestHolder = new DbSet<NpcTest>("world.db");

        List<NpcTest> testList = npcTestHolder.GetAll();

        Debug.LogWarning($"First spawn info: SpawnID: {testList[0].SpawnID}, TemplateID: {testList[0].TemplateID}");
    }
}
```

The `GetAll()` method retrieves every row from the table as your previously defined C# object.