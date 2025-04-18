
# TriSerializer

TriSerializer is an evolving Unity serialization framework for serializing and deserializing Unity objects into binary files. The project is not yet complete and currently lacks support for:

- Animation Clips serialization
- Cameras serialization
- Lights serialization
- Physics components (colliders, rigidbodies, etc)

Contributions and feedback are welcome.

## Overview

TriSerializer enables efficient serialization of Unity objects like GameObjects, components, meshes, materials, and textures into binary files. It is designed for saving game states, creating asset bundles, or other use cases requiring robust object persistence. The framework is extensible, allowing developers to add support for custom Unity types.

## Features

- **Core Serialization**: Supports `GameObject`, `Transform`, `Mesh`, `Material`, `Texture2D`, `MeshFilter`, `MeshRenderer`, and `SkinnedMeshRenderer`.
- **Extensible Design**: Built around the `IObjectSerializer` interface and `ObjectSerializer<T>` class for easy customization.
- **Resource Handling**: Serializes dependent resources (e.g., textures in materials, meshes in renderers) for complete object reconstruction.
- **Binary Efficiency**: Uses `BinaryReader` and `BinaryWriter` with extensions for optimized binary data handling.
- **Callback Support**: Includes serialization and deserialization callbacks for custom logic.

## Installation

1. Clone or download the repository to your Unity project's `Assets` folder.
2. Optionally, define the `USE_SHARED` preprocessor directive to enable serialization of shared resources (e.g., `sharedMesh`, `sharedMaterials`).

## Usage

### Basic Serialization and Deserialization

Serialize a `GameObject` to a file and deserialize it back:

```csharp
using TriSerializer;
using UnityEngine;

public class Example : MonoBehaviour
{
    public GameObject targetGameObject;
    private string filePath = "Assets/serialized.data";

    void Start()
    {
        // Serialize the GameObject
        Serializer.SerializeToFile(
            filename: filePath,
            gameObject: targetGameObject,
            nonLock: false,
            onFinish: (context) => Debug.Log("Serialization complete"),
            onHash: null,
            userData: null,
            compress: false
        );

        // Deserialize into a new GameObject
        Serializer.DeserializeToGameObject(
            filename: filePath,
            nonLock: false,
            userData: null,
            onFinish: (context) => Debug.Log($"Deserialized GameObject: {context.GameObject.name}")
        );
    }
}
```

## Key Components

### Core Classes

- `IObjectSerializer`: Interface for serialization, deserialization, and resource collection.
- `ObjectSerializer<T>`: Abstract base for serializing Unity objects, handling instance ID and hide flags.
- `ComponentSerializer<T>`: Extends `ObjectSerializer<T>` for components, managing GameObject associations.
- `RendererSerializer<T>`: Handles renderer-specific properties and materials.

### Specific Serializers

- `GameObjectSerializer`: Serializes `GameObject` properties (e.g., name, layer, tag, active state).
- `TransformSerializer`: Manages `Transform` properties (e.g., position, rotation, scale, parent).
- `MeshSerializer`: Serializes `Mesh` data (e.g., vertices, normals, UVs, blend shapes).
- `MaterialSerializer`: Handles `Material` properties, shaders, and textures.
- `TextureSerializer2D`: Serializes `Texture2D` data, including raw pixels and settings.
- `MeshFilterSerializer`: Manages `MeshFilter` components and mesh references.
- `MeshRendererSerializer`: Serializes `MeshRenderer` components, including vertex streams.
- `SkinnedMeshRendererSerializer`: Handles `SkinnedMeshRenderer` components, bones, and meshes.

### Utilities

- `BinaryReaderExtensions` **and** `BinaryWriterExtensions`: Extension methods for Unity types (e.g., `Vector3`, `Quaternion`).
- `FileSystemHelper`: Utilities for path validation and SHA-256 hashing.
- `CoroutineHelper`: Singleton for managing coroutines.

## Extensibility

To create a custom serializer:

1. Inherit from `ObjectSerializer<T>` or `ComponentSerializer<T>` for components.
2. Override the `Identifier` property with a unique ID.
3. Implement `Serialize`, `Deserialize`, and optionally `CollectResources`.
4. Register with `Serializer.ObjectSerializers`.

Example:

```csharp
public class CustomComponentSerializer : ComponentSerializer<CustomComponent>
{
    public override ObjectIdentifier Identifier => "CUS";

    public override IEnumerable Serialize(SerializationContext context, CustomComponent source)
    {
        foreach (var item in base.Serialize(context, source))
        {
            yield return item;
        }
        context.BinaryWriter.Write(source.SomeProperty);
    }

    public override IEnumerable Deserialize(SerializationContext context)
    {
        foreach (var item in base.Deserialize(context))
        {
            yield return item;
        }
        var component = context.GameObject.AddComponent<CustomComponent>();
        component.SomeProperty = context.BinaryReader.ReadSingle();
        context.SetupDestination(component);
        DeserializationCallback(context, component);
    }
}
```

## This project uses
SharpZipLib (MIT)

https://github.com/icsharpcode/SharpZipLib

## Notes

- **Readability**: Types like `Mesh` and `Texture2D` require `isReadable` to be true, or serialization will fail with an error.

## License

Licensed under the MIT License. See the LICENSE file for details.
