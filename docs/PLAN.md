# WinNUT-Cursed Plan

This document tracks the current migration state of `WinNUT-Cursed` and the next major steps needed to finish the fork to a senior-maintained standard.

## Current Status

- The legacy VB.NET solution tree has been replaced by the renamed `WinNUT-Cursed` C# solution structure.
- The active solution is `WinNUT-Cursed/WinNUT-Cursed.sln`.
- The repository root, CI workflows, and supporting docs now use the `WinNUT-Cursed` path consistently.
- `CHANGELOG.md` has been reset for this fork and now points to upstream history for pre-fork release notes.
- Old translation automation and VB-era localized files have been removed.
- The remaining `.resx` files are still required and are part of the active WinForms C# application.

## Verified Baseline

Last checked in this repository state:

- `dotnet build "C:\Data\Programming\Cursor\WinNUT-Cursed\WinNUT-Cursed\WinNUT-Cursed.sln" -c Debug`
- Result: success, `0 warnings`, `0 errors`

## Completed Milestones

1. Port the WinNUT client and shared library from VB.NET to C#.
2. Rename the inner solution tree from `WinNUT_V2` to `WinNUT-Cursed`.
3. Update GitHub workflow paths to the renamed tree.
4. Reset fork documentation so `README.md` and `CHANGELOG.md` reflect this repo rather than upstream release history.
5. Preserve only the active resource system needed by the current C# WinForms app.

## Next Priorities

1. Validate release packaging end-to-end on GitHub tags, especially ClickOnce output and release assets in `.github/workflows/build-release.yaml`.
2. Verify MSI packaging from `WinNUT-Cursed/Setup/Setup.vdproj` against the renamed solution tree and current build configurations.
3. Revisit the `WinNUT-Setup` solution entry in Visual Studio 2022 because Solution Explorer still shows it as incompatible with the note `The application is not installed.`, even after the installer-project extension update.
4. Test live connectivity to a real UPS and confirm the current C# build can connect and read expected values.
5. Verify the gauges and related live status visuals behave correctly, since earlier builds did not render or update as expected.
6. Review the GUI layout, sizing, and scaling across key forms and adjust anything that regressed during the port.
7. Audit the remaining application behavior against upstream WinNUT to confirm there are no porting regressions in forms, updater flow, and NUT protocol handling.
8. Decide whether the fork should remain English-only or reintroduce localized resource files in a deliberate way.
9. Add a lightweight release checklist for future tagged builds so the repo stays consistent after each milestone.

## Guardrails

- Keep `README.md`, `CHANGELOG.md`, and this file aligned whenever the project scope changes.
- Add new recurring bug-fix notes to `docs/BUG_SOLUTIONS.md`.
- Do not remove the current `.resx` files unless the WinForms resource model is deliberately replaced.
- Treat release workflow and installer changes as requiring manual verification, not just source review.
