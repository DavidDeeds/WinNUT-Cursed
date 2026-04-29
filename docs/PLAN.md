# WinNUT-Cursed Plan

This document tracks the current migration state of `WinNUT-Cursed` and the next major steps needed to finish the fork to a senior-maintained standard.

## Current Status

- The legacy VB.NET solution tree has been replaced by the renamed `WinNUT-Cursed` C# solution structure.
- The active solution is `WinNUT-Cursed/WinNUT-Cursed.sln`.
- The repository root, CI workflows, and supporting docs now use the `WinNUT-Cursed` path consistently.
- `CHANGELOG.md` has been reset for this fork and now serves as the release-history entry point for this repository state.
- Old translation automation and VB-era localized files have been removed.
- The remaining `.resx` files are still required and are part of the active WinForms C# application.

## Verified Baseline

Last checked in this repository state:

- `dotnet build "C:\Data\Programming\Cursor\WinNUT-Cursed\WinNUT-Cursed\WinNUT-Cursed.sln" -c Debug`
- Result: success, but the solution still emits existing WinForms/resource warnings that should not be treated as release-quality proof on their own.

## Completed Milestones

1. Port the WinNUT client and shared library from VB.NET to C#.
2. Rename the inner solution tree from `WinNUT_V2` to `WinNUT-Cursed`.
3. Update GitHub workflow paths to the renamed tree.
4. Reset fork documentation so `README.md` and `CHANGELOG.md` reflect this repo and its current migration scope.
5. Preserve only the active resource system needed by the current C# WinForms app.

## Next Priorities

1. Fix the connection lifecycle in `WinNUT-Cursed/WinNUT.Client.Common/UpsDevice.cs` so the app only reports a successful connection after optional login/authentication has actually completed.
2. Repair blackout-facing runtime behavior in `WinNUT-Cursed/WinNUT.Client/WinNUT.cs` and `WinNUT-Cursed/WinNUT.Client/ShutdownGui.cs`, especially battery transition notifications, shutdown timer handling, and behavior when battery metrics are missing.
3. Make `WinNUT-Cursed/WinNUT.Client.Common/NutSocket.cs` and `WinNUT-Cursed/WinNUT.Client.Common/UpsDevice.cs` more defensive against malformed or partial protocol lines, especially in `Query_List_Datas` and status parsing.
4. Test live connectivity to a real UPS and confirm the current C# build can connect, authenticate, notify correctly, and read expected values under normal and blackout conditions.
5. Verify the gauges and related live status visuals behave correctly, since earlier builds did not render or update as expected.
6. Review the GUI layout, sizing, and scaling across key forms and adjust anything that regressed during the port.
7. Align updater logic in `WinNUT-Cursed/WinNUT.Client.Common/Updater/UpdateUtil.cs` with the actual release artifacts published by `.github/workflows/build-release.yaml`, especially MSI vs ClickOnce vs zip expectations.
8. Harden updater selection and download behavior in `WinNUT-Cursed/WinNUT.Client.Common/Updater/UpdateUtil.cs`, especially release ordering, draft filtering, prerelease policy, and download validation.
9. Validate release packaging end-to-end on GitHub tags, especially ClickOnce output and release assets in `.github/workflows/build-release.yaml`.
10. Verify MSI packaging from `WinNUT-Cursed/Setup/Setup.vdproj` against the renamed solution tree and current build configurations.
11. Revisit the `WinNUT-Setup` solution entry in Visual Studio 2022 because Solution Explorer still shows it as incompatible with the note `The application is not installed.`, even after the installer-project extension update.
12. Audit the remaining application behavior end-to-end to confirm there are no porting regressions in forms, updater flow, notification behavior, and NUT protocol handling.
13. Decide whether the fork should remain English-only or reintroduce localized resource files in a deliberate way.
14. Add a lightweight release checklist for future tagged builds so the repo stays consistent after each milestone.

## Review Findings

### Runtime / Code

- `WinNUT-Cursed/WinNUT.Client.Common/UpsDevice.cs`: `Connect_UPS` appears to raise connected-ready behavior before optional login fully succeeds, which can leave the UI showing a connected state even if authentication fails immediately afterward.
- `WinNUT-Cursed/WinNUT.Client/WinNUT.cs`: battery transition popup logic currently appears unreachable, so the intended on-battery / back-on-line notifications are not being triggered as designed.
- `WinNUT-Cursed/WinNUT.Client/ShutdownGui.cs`: shutdown timer handling still needs a robustness pass because the current implementation does UI-thread work during a critical countdown path.
- `WinNUT-Cursed/WinNUT.Client/WinNUT.cs` and `WinNUT-Cursed/WinNUT.Client.Common/UpsDevice.cs`: blackout shutdown policy still needs explicit review for the case where the UPS reports `OB` but battery charge/runtime values are unavailable.
- `WinNUT-Cursed/WinNUT.Client.Common/Updater/UpdateUtil.cs`: update selection currently needs a stricter release policy so draft releases or incorrectly ordered results cannot become the chosen update target.
- `WinNUT-Cursed/WinNUT.Client.Common/NutSocket.cs`: list-query parsing should be hardened so malformed or unexpected server lines do not cause avoidable parsing failures.
- `WinNUT-Cursed/WinNUT.Client/Program.cs`: second-instance handling is still silent, which is acceptable for now but is not ideal user experience.
- The solution still has no automated test project, so protocol logic, updater behavior, and UI event sequencing are still primarily protected by manual verification.

### Build / Release / Packaging

- `WinNUT-Cursed/WinNUT.Client.Common/Updater/UpdateUtil.cs` and `.github/workflows/build-release.yaml`: the in-app updater and the GitHub release workflow still need to agree on what the canonical downloadable asset is, especially whether releases are expected to publish MSI, ClickOnce, zip, or some combination.
- `.github/workflows/build-release.yaml`: release generation can still succeed with incomplete assets because ClickOnce output is conditional and unmatched release files do not currently fail the workflow.
- `WinNUT-Cursed/Setup/Setup.vdproj`: installer metadata and output behavior still need a dedicated validation pass, including current versioning, output naming, changelog inclusion, and renamed-path assumptions.
- `WinNUT-Cursed/WinNUT.Client/WinNUT.Client.csproj` and `WinNUT-Cursed/Setup/Setup.vdproj`: support URLs, installer metadata, and release-facing branding still need a final consistency pass so users always land on the intended project resources.
- GitHub Actions currently build the app successfully, but release readiness is still weaker than normal build readiness.

## Recommended Engineering Sequence

1. Fix the connection/authentication lifecycle first so the UI and logs can be trusted during further debugging.
2. Repair blackout-facing runtime behavior next, especially battery transition notifications, shutdown countdown handling, and missing-metric shutdown edge cases.
3. Harden protocol parsing and polling resilience so runtime decisions are based on reliable NUT data.
4. Validate the app manually against a real UPS, including blackout behavior, gauges, notifications, and main GUI behavior.
5. Align updater logic with the assets actually produced by GitHub releases, then tighten release workflow behavior.
6. Finish installer/packaging cleanup and Visual Studio setup-project validation.
7. Add a minimal automated test project for parser, updater, and connection lifecycle coverage.

## Guardrails

- Keep `README.md`, `CHANGELOG.md`, and this file aligned whenever the project scope changes.
- Keep `docs/HOW-WINNUT-CURSED-WORKS.md` aligned with this file whenever blackout behavior, notification flow, or release/update behavior changes.
- Add new recurring bug-fix notes to `docs/BUG_SOLUTIONS.md`.
- Do not remove the current `.resx` files unless the WinForms resource model is deliberately replaced.
- Treat release workflow, installer changes, and blackout/shutdown behavior as requiring manual verification on Windows, not just source review.
