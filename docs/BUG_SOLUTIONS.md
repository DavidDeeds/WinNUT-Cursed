# Bug solutions and patterns

Short notes for recurring fixes in this repo. Add an entry when you fix a user-facing or CI bug.

## Logger: `MaxEvents` had no effect (2025)

**Symptom:** Setting `MaxEvents` did not change the in-memory event buffer size.

**Cause:** The property setter validated input but never assigned `_MaxEvents`.

**Fix:** Assign `_maxEvents = value` after validation in [Logger.cs](../WinNUT_V2/WinNUT.Client.Common/Logger.cs).

## Logger: NullReference after failed log file init (2025)

**Symptom:** If `FileLogTraceListener` construction failed, later code still called `LogFile.WriteLine`.

**Cause:** `LogFile` was cleared in the catch path but the method continued unconditionally.

**Fix:** After the try/catch, `Return` early when `Not IsWritingToFile` before writing history to the file.

## UpdateUtil: Download progress spam and hangs (2025)

**Symptom:** Progress events fired every read; downloads could hang forever.

**Cause:** `DateTime.AddTicks` was called without assigning the result; `HttpClient` had no timeout.

**Fix:** Use `nextProgressUpdate = Date.Now.AddMilliseconds(PROGRESS_CHANGED_DELAY)` after each event; set `HttpClient.Timeout`. Use `FirstOrDefault` for `.msi` assets so missing assets do not throw in the property setter.

## NutSocket: Stream races and stuck `streamInUse` (2025)

**Symptom:** Concurrent queries could interleave; `streamInUse` could stay true after errors in `Query_List_Datas`.

**Cause:** The flag was cleared before the read phase; list reads had no `Try`/`Finally`.

**Fix:** Hold `streamInUse` for the full `Query_Data` operation inside one outer `Try`/`Finally`. Wrap the `Query_List_Datas` read loop in `Try`/`Finally`. Harden `ERR` lines with `Enum.TryParse`. Replace generic `Exception("error")` with `NutException`. Use `ExceptionDispatchInfo.Capture(ex).Throw()` in `OnSocketBroken` to preserve stack traces.

## Crash UI: reversed log buffer and non-Exception failures (2025)

**Symptom:** After generating a crash report, in-memory log order was reversed; rare non-`Exception` faults broke the handler.

**Cause:** `LastEvents.Reverse()` mutated the live list; `AppDomain` unhandled events can pass non-CLR objects.

**Fix:** Copy `LastEvents` to a new list, reverse the copy for the report only. Wrap unknown `ExceptionObject` in `Exception` before showing the dialog. Use `Using` for the crash report `StreamWriter`.

## CI: Wrong prerelease / semver from tags (2025)

**Symptom:** `get-ver.ps1` failed or mis-detected prerelease; gh-pages commits showed empty version.

**Cause:** Semver regex was matched against full `refs/tags/v…` without stripping the prefix. `GroupCollection` has no `ContainsKey` for optional groups. PowerShell `if ("false")` is true. `env.SEMVER` was never set while outputs held `SEMVER`.

**Fix:** Strip `refs/tags/` and optional `v` before matching. Use `Groups["prerelease"].Success` for prerelease. Compare `ISPRERELEASE` to the string `'true'`. Commit message uses `${{ steps.get-ver.outputs.SEMVER }}`.

## UpsDevice `forceful` disconnect (2025)

**Symptom / confusion:** `forceful` sounded like “hard kill” but mapped to `NutSocket.Disconnect(skipLogout)`.

**Fix:** Document on `UpsDevice.Disconnect` that `forceful` means skip LOGOUT (same as `skipLogout` on the socket).
