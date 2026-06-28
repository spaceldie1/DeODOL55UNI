# DeODOL55UNI

## Summary

This release packages a patched `DeODOL54`-derived build intended to recover several ODOL v54 parsing failures observed on real DayZ models.

## Main Changes

- added recovery for shifted embedded `RVMat` blocks
- added support for cases where polygon data starts immediately after materials
- fixed empty-LOD handling
- fixed `nUVSets == 0` handling
- reduced default log noise for CLI use

## Verified Result

Confirmed successful conversion of a previously failing model:

- `minidress_f.p3d`

Produced outputs:

- `minidress_f_mlod.p3d`
- `model.cfg`

## Notes

This is a public source release of a patched local build. See `README.md` for provenance and licensing notes.

