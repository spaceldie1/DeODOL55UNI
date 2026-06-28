# DeODOL55UNI

## Summary

This release packages a patched `DeODOL54`-derived build intended to recover several ODOL v54 parsing failures observed on real DayZ models.

## Main Changes

- added recovery for shifted embedded `RVMat` blocks
- added polygon-header realignment before `Polygons`
- added support for cases where polygon data starts immediately after materials
- added explicit handling for empty compressed index-array blocks
- fixed empty-LOD handling
- fixed `nUVSets == 0` handling
- added plausibility guards for generic array counters and UV-set counts
- made `model.cfg` extraction non-fatal when partial config data is present
- reduced default log noise for CLI use

## Verified Result

Confirmed successful conversion of previously failing models:

- `minidress_f.p3d`
- `tenement_medium.p3d`
- `tisy_garages2_sakhal_grass_l.p3d`
- `transitbus.p3d`

Smoke-test status for this build:

- `40 / 40` models passed
- selection rule: top 40 `.p3d` files under `15 MB` from a large local `P:\\dz` dataset

Produced outputs:

- `minidress_f_mlod.p3d`
- `model.cfg`

## Notes

This is a public source release of a patched local build. See `README.md` for provenance and licensing notes.
