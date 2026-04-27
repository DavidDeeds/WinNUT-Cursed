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

1. Fix the connection lifecycle in `WinNUT-Cursed/WinNUT.Client.Common/UpsDevice.cs` so the app only reports a successful connection after optional login/authentication has actually completed.
2. Harden updater selection and download behavior in `WinNUT-Cursed/WinNUT.Client.Common/Updater/UpdateUtil.cs`, especially release ordering, draft filtering, and download validation.
3. Make `WinNUT-Cursed/WinNUT.Client.Common/NutSocket.cs` more defensive against malformed or partial protocol lines, especially in `Query_List_Datas`.
4. Test live connectivity to a real UPS and confirm the current C# build can connect, authenticate, and read expected values.
5. Verify the gauges and related live status visuals behave correctly, since earlier builds did not render or update as expected.
6. Review the GUI layout, sizing, and scaling across key forms and adjust anything that regressed during the port.
7. Validate release packaging end-to-end on GitHub tags, especially ClickOnce output and release assets in `.github/workflows/build-release.yaml`.
8. Verify MSI packaging from `WinNUT-Cursed/Setup/Setup.vdproj` against the renamed solution tree and current build configurations.
9. Revisit the `WinNUT-Setup` solution entry in Visual Studio 2022 because Solution Explorer still shows it as incompatible with the note `The application is not installed.`, even after the installer-project extension update.
10. Audit the remaining application behavior against upstream WinNUT to confirm there are no porting regressions in forms, updater flow, and NUT protocol handling.
11. Decide whether the fork should remain English-only or reintroduce localized resource files in a deliberate way.
12. Add a lightweight release checklist for future tagged builds so the repo stays consistent after each milestone.

## Review Findings

### Runtime / Code

- `WinNUT-Cursed/WinNUT.Client.Common/UpsDevice.cs`: `Connect_UPS` appears to raise connected-ready behavior before optional login fully succeeds, which can leave the UI showing a connected state even if authentication fails immediately afterward.
- `WinNUT-Cursed/WinNUT.Client.Common/Updater/UpdateUtil.cs`: update selection currently needs a stricter release policy so draft releases or incorrectly ordered results cannot become the chosen update target.
- `WinNUT-Cursed/WinNUT.Client.Common/NutSocket.cs`: list-query parsing should be hardened so malformed or unexpected server lines do not cause avoidable parsing failures.
- `WinNUT-Cursed/WinNUT.Client/Program.cs`: second-instance handling is still silent, which is acceptable for now but is not ideal user experience.
- The solution still has no automated test project, so protocol logic, updater behavior, and UI event sequencing are still primarily protected by manual verification.

### Build / Release / Packaging

- `.github/workflows/build-release.yaml`: release generation can still succeed with incomplete assets because ClickOnce output is conditional and unmatched release files do not currently fail the workflow.
- `WinNUT-Cursed/Setup/Setup.vdproj`: installer metadata and output behavior still need a dedicated validation pass, including current versioning, output naming, changelog inclusion, and renamed-path assumptions.
- `WinNUT-Cursed/WinNUT.Client/WinNUT.Client.csproj` and `WinNUT-Cursed/Setup/Setup.vdproj`: support URLs, installer metadata, and release-facing branding still need a final fork-specific pass so users are not sent back to upstream by mistake.
- GitHub Actions currently build the app successfully, but release readiness is still weaker than normal build readiness.

## Recommended Engineering Sequence

1. Fix the connection/authentication lifecycle and protocol parsing risks first.
2. Validate the app manually against a real UPS, including gauges and main GUI behavior.
3. Tighten updater logic and release workflow behavior.
4. Finish installer/packaging cleanup and Visual Studio setup-project validation.
5. Add a minimal automated test project for parser, updater, and connection lifecycle coverage.

## Guardrails

- Keep `README.md`, `CHANGELOG.md`, and this file aligned whenever the project scope changes.
- Add new recurring bug-fix notes to `docs/BUG_SOLUTIONS.md`.
- Do not remove the current `.resx` files unless the WinForms resource model is deliberately replaced.
- Treat release workflow and installer changes as requiring manual verification, not just source review.
