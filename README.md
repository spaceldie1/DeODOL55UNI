# DeODOL55UNI

Patched public source release of a local `DeODOL54`-based P3D debinarizer for DayZ/Bohemia ODOL v54 models.

## What This Fixes

This build was created to address a real failure case where `DeODOL54` crashed on some `.p3d` models during debinarization.

The patched build fixes several parser issues found in ODOL v54 models:

- embedded `RVMat` alignment recovery after malformed or shifted material tails
- polygon header realignment before `Polygons`
- recovery when `pointToVertex` / `vertexToPoint` arrays are absent and polygon data starts immediately
- explicit handling for empty compressed index-array blocks
- safe handling of empty LODs
- safe handling of `nUVSets == 0`
- plausibility guards for array counters and UV-set counts to avoid runaway reads on malformed layouts
- non-fatal `model.cfg` extraction when source animation/config data is incomplete
- quieter default logging for practical CLI usage

Validated on previously failing models:

- `minidress_f.p3d`
- `tenement_medium.p3d`
- `tisy_garages2_sakhal_grass_l.p3d`
- `transitbus.p3d`

Smoke test result on a DayZ sample set:

- `40 / 40` ODOL `.p3d` files converted successfully
- sample selection: largest files under `15 MB` from a large local `P:\\dz` model tree
- outputs verified as generated in isolated temp workspaces

## Build

Requirements:

- .NET 8 SDK or newer

Build:

```powershell
dotnet build .\Deodol54.csproj -c Release
```

Publish standalone Windows build:

```powershell
dotnet publish .\Deodol54.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o .\publish
```

## Usage

Single model:

```powershell
.\DeODOL55UNI.exe P:\path\to\model.p3d
```

Batch mode:

```powershell
.\DeODOL55UNI.exe allp3ds P:\path\to\models
```

Output:

- `*_mlod.p3d`
- `model.cfg`

## Repository Scope

This repository contains the patched source tree used to build `DeODOL55UNI`, plus release binaries distributed through GitHub Releases.

## Provenance And Licensing

This repository is based on a locally recovered/decompiled and patched `DeODOL54` binary workflow.

Because the original upstream source licensing for that exact binary has not been independently verified here, this repository is published publicly for transparency and reproducibility, but it is **not currently presented as an OSI-licensed clean-room reimplementation**.

If a verifiable upstream source repository and license are identified later, the licensing position for this repository should be updated to match what is legally supportable.

## Status

Current packaged build:

- `DeODOL55UNI`
- version label: `55.0.2-uni`
