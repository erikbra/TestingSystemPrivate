# TestingSystemPrivate
Repo to reproduce issue with System.Private.ServiceModel, reported here:

https://github.com/dotnet/wcf/issues/4500

(details copied from the issue, in case someone moves the repos in the future)

**Describe the bug**
I am unable to use the types in System.IdentityModel, System.ServiceModel, etc, that are defined in System.Private.ServiceModel in a .net core app (neither netcoreapp3.1 or net5.0 works). I don't know if I'm supposed to reference this nuget package directly, or if referencing System.ServiceModel.Primitives should make it "just work".

**To Reproduce**
Steps to reproduce the behavior:
1. Create a new .net core project - minimal example created here: https://github.com/erikbra/TestingSystemPrivate
2. Add reference to System.Private.ServiceModel nuget package
3. Use a type, e.g. `System.IdentityModel.Tokens.X509SecurityToken` in your code
2. `dotnet build`
3. Observe one of two errors, either with or without a 'hack' to copy `System.Private.ServiceModel.dll` to the output folder.

The 'hack' described, is to explicitly add a reference to system.servicemodel.dll in the `.csproj` file:
```csproj
<Reference Include="$(NuGetPackageRoot)/system.private.servicemodel/**/system.private.servicemodel.dll" />
```

The actual files needed are:

## Program.cs
```csharp
using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace TestingSystemPrivate
{
    public class Program
    {
        // I can reference the X509SecurityToken type if I do the hack in the .csproj to copy
        // System.Private.ServiceModel.dll to the output folder.
        // If I omit the hack, this type is not recognized.
        static X509SecurityToken _token;

        public static void Main()
        {
            _token = new X509SecurityToken(new X509Certificate2());
            Console.WriteLine("Token: " + _token);

            // If I include the hack in the csproj, I get an error that this type exists in both
            // System.Private.ServiceModel, Version=4.8.0.0 and
            // System.ServiceModel.Primitives, Version=4.8.0.0
            var c = new SecurityKeyIdentifierClause();
        }
    }
}
```

## TestingSystemPrivate.csproj

```csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <OutputType>Exe</OutputType>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="System.Private.ServiceModel" Version="4.8.0" />
    <!-- Without this "hack", It will not compile -->
    <!-- <Reference Include="$(NuGetPackageRoot)/system.private.servicemodel/**/system.private.servicemodel.dll" /> -->

    <!-- But, if I add more nuget packages in the same areas, there are problems with types found in more than one dll -->
    <PackageReference Include="System.ServiceModel.Primitives" Version="4.8.0" />
  </ItemGroup>

</Project>
```


**Expected behavior**
It should be possible to use types in System.IdentityModel (e.g. `System.IdentityModel.Tokens.X509SecurityToken`) in a .net core 3.1 app if the correct Nuget Package is referenced. I'm uncertain of whether this should be
* System.Private.ServiceModel
* System.ServiceModel.Primitives
* (or both)

**Output from `dotnet build`**

**Without** the 'hack' in the .csproj file to copy:
```console
❯ dotnet build
Microsoft (R) Build Engine version 16.8.0+126527ff1 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  You are using a preview version of .NET. See: https://aka.ms/dotnet-core-preview
C:\Users\erikb\Source\Repos\TestingSystemPrivate\Program.cs(12,16): error CS0246: The type or namespace name 'X509SecurityToken' could not be found (are you missing a using directive or an assembly reference?) [C:\Users\erikb\Source\Repos\TestingSystemPrivate\TestingSystemPrivate.csproj]

Build FAILED.

C:\Users\erikb\Source\Repos\TestingSystemPrivate\Program.cs(12,16): error CS0246: The type or namespace name 'X509SecurityToken' could not be found (are you missing a using directive or an assembly reference?) [C:\Users\erikb\Source\Repos\TestingSystemPrivate\TestingSystemPrivate.csproj]
    0 Warning(s)
    1 Error(s)

Time Elapsed 00:00:01.01
```

**With** the 'hack' in the .csproj file to copy:
```console
❯ dotnet build
Microsoft (R) Build Engine version 16.8.0+126527ff1 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  Restored C:\Users\erikb\Source\Repos\TestingSystemPrivate\TestingSystemPrivate.csproj (in 246 ms).
  You are using a preview version of .NET. See: https://aka.ms/dotnet-core-preview
C:\Users\erikb\Source\Repos\TestingSystemPrivate\Program.cs(22,25): error CS0433: The type 'SecurityKeyIdentifierClause' exists in both 'System.Private.ServiceModel, Version=4.8.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' and 'System.ServiceModel.Primitives, Version=4.8.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' [C:\Users\erikb\Source\Repos\TestingSystemPrivate\TestingSystemPrivate.csproj]

Build FAILED.

C:\Users\erikb\Source\Repos\TestingSystemPrivate\Program.cs(22,25): error CS0433: The type 'SecurityKeyIdentifierClause' exists in both 'System.Private.ServiceModel, Version=4.8.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' and 'System.ServiceModel.Primitives, Version=4.8.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' [C:\Users\erikb\Source\Repos\TestingSystemPrivate\TestingSystemPrivate.csproj]
    0 Warning(s)
    1 Error(s)

Time Elapsed 00:00:01.46
```

**Additional context**
Add any other context about the problem here.

