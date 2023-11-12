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

The `[SQLiteTable("npc_spawn")]` attribute is required and defines the table that this class refers to ('`npc_spawn`').
The `[Column("SpawnID")]` attribute is optional, and specifies which column a given variable represents, with the `PrimaryKey = true` parameter specifying primary keys. If no `[Column(...)]` attributes are used, the column name will be identical to the property name.

If using the `[Column(...)]` attribute for every property, the `NpcSpawn` class could look like this:

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

The `[Column(...)]` attribute is most useful in the case of wanting to have a property corresponding to a column with a different name, for example:

```csharp
[SQLiteTable("npc_spawn")]
public class NpcSpawn
{
    ...
    [Column("position_x")]
    public float PosX { get; set; }
    ...
```

In this case, data from the column '`position_x`' will be stored in the `PosX` property.

### Retrieving Data

Once you have your schema defined, accessing the data can be done from within a `MonoBehaviour` class using a `DbSet<T>` (where T is your DB schema class) object for each table:

```csharp
public class DatabaseTest : MonoBehaviour
{
    DbSet<NpcSpawn> npcTestHolder;

    void Awake()
    {
        npcTestHolder = new DbSet<NpcSpawn>("world.db");

        List<NpcSpawn> testList = npcTestHolder.GetAll();

        Debug.LogWarning($"First spawn info: SpawnID: {testList[0].SpawnID}, TemplateID: {testList[0].TemplateID}");
    }
}
```

When initializing the `DbSet<TableClass>` object, pass in your SQLite database file name.
From there, calling `GetAll()` retrieves every row from the table as a list of the previously defined C# objects.

### Queries and Commands

You can also query or execute commands on the table, using `DbSet<T>.Query()` and `DbSet<T>.Execute()`:

```csharp
public class DatabaseTest : MonoBehaviour
{
    DbSet<NpcSpawn> npcTestHolder;

    void Awake()
    {
        npcTestHolder = new DbSet<NpcSpawn>("world.db");

        int spawnId = 4;

        List<NpcSpawn> foundNpcSpawns = npcTestHolder.Query($"SELECT * FROM @TableName WHERE SpawnID = '{spawnId}'");

        if (foundNpcSpawns.Count > 0)
        {
            NpcSpawn npcSpawn = foundNpcSpawns[0];

            Debug.Log($"Spawn with ID template is {npcSpawn.TemplateID}");

            npcTestHolder.Execute($"DELETE FROM @TableName WHERE SpawnID = '{npcSpawn.SpawnID}'");

            Debug.Log($"Deleted the npc spawn that was found.");
        }
    }
}
```

In the above example, we query the table for an `NpcSpawn` with a `SpawnID` of 4 and then delete the entry. `@TableName` can be used in place of the actual table name in queries and commands.