// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using System.Runtime.InteropServices;

namespace Cratis.VerticalSlices;

/// <summary>
/// Represents an enumerator for types in an assembly.
/// </summary>
public class AssemblyTypeEnumerator : IDisposable
{
    private readonly MetadataLoadContext _mlc;
    private readonly Assembly _assembly;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyTypeEnumerator"/> class.
    /// </summary>
    /// <param name="assemblyFile">The path to the assembly file.</param>
    public AssemblyTypeEnumerator(string assemblyFile)
    {
        var assemblyFolder = Path.GetDirectoryName(assemblyFile)!;

        // Build the search set
        var root = Path.GetDirectoryName(RuntimeEnvironment.GetRuntimeDirectory())!;
        var version = Path.GetFileName(root)!;
        var shared = Directory.GetParent(Directory.GetParent(root)!.FullName)!.FullName;
        var aspnet = Path.Combine(shared, "Microsoft.AspNetCore.App", version);

        var runtimeAssemblies = Directory.GetFiles(root, "*.dll");
        var aspnetAssemblies = Directory.GetFiles(aspnet, "*.dll");
        var appAssemblies = Directory.GetFiles(assemblyFolder, "*.dll");

        var paths = runtimeAssemblies.Concat(aspnetAssemblies).Concat(appAssemblies)
                                     .Distinct(StringComparer.OrdinalIgnoreCase);

        _mlc = new MetadataLoadContext(new PathAssemblyResolver(paths));
        _assembly = _mlc.LoadFromAssemblyPath(Path.GetFullPath(assemblyFile));
    }

    /// <summary>
    /// Gets the exported types from the assembly.
    /// </summary>
    /// <returns>The exported types.</returns>
    public IEnumerable<Type> ExportedTypes => _assembly.ExportedTypes;

    /// <inheritdoc/>
    public void Dispose()
    {
        _mlc.Dispose();
    }
}
