# How WinNUT-Cursed Works

This document explains the current runtime behavior of `WinNUT-Cursed`, especially how it connects to a NUT server, polls UPS state, reacts to blackouts, updates the UI, and decides when to shut the Windows machine down.

It reflects the current C# WinForms code in this repository, not an idealized design. Where the present behavior has notable caveats, those are called out explicitly.

## High-Level Architecture

`WinNUT-Cursed` is a Windows Forms desktop application with three main layers:

- `WinNUT.Client`: the WinForms UI, user settings screens, notifications, and shutdown orchestration.
- `WinNUT.Client.Common`: the NUT protocol client, UPS polling, updater logic, and shared helpers.
- `Setup`: the MSI packaging project.

At runtime, the main form creates an `UpsDevice` using the current saved settings, then subscribes to connection, data-refresh, and status-change events. The UI is therefore event-driven even though the UPS itself is polled on a timer.

## Startup and Connection

When the app starts, it loads settings, initializes logging, and opens the main `WinNUT` form. The form then creates an `UpsDevice` using the configured NUT host, port, username, password, poll interval, and nominal input frequency.

The active poll interval comes from `NUT_PollIntervalMsec`. The current default is `1000`, so the app polls once per second unless the user changes it.

## Polling Model

The UPS polling loop is driven by a WinForms timer in `UpsDevice`.

- A `System.Windows.Forms.Timer` named `_updateData` is created in `UpsDevice`.
- Its interval is set from the configured poll interval.
- Each tick runs `Retrieve_UPS_Datas`.

On each successful poll, the app reads live NUT variables such as:

- `battery.charge`
- `battery.voltage`
- `battery.runtime`
- `input.frequency`
- `input.voltage`
- `output.voltage`
- `ups.load`
- `ups.status`

If some values are unavailable, the code sometimes falls back to calculations. For example, battery charge can be estimated from battery voltage, and runtime can be estimated when the UPS does not report it directly.

## How the App Knows a Blackout Happened

The app relies on the NUT status field `ups.status`.

That status is normalized and parsed into the `UPS_States` flags enum. The important flags for blackout behavior are:

- `OL`: on line, utility power present
- `OB`: on battery, utility power lost / UPS carrying the load
- `LB`: low battery
- `FSD`: full shutdown requested by the NUT server

The app therefore does not detect a blackout by observing input voltage alone. It detects it because NUT reports that the UPS has moved to `OB`.

## Status Transitions vs Every Poll

The code distinguishes between:

- `StatusesChanged`: raised only when newly active status bits appear
- `DataUpdated`: raised on every successful poll

That means:

- Transition handling such as "we just went on battery" is based on status changes.
- Gauge refresh, tray text refresh, battery icon updates, and threshold checks happen on every poll.

This is an important design detail: the app only needs one `OB` transition to know that mains power failed, but it keeps reevaluating battery/runtime conditions every second while the UPS remains on battery.

## What Happens During a Blackout

When the UPS changes to `OB`, the app logs that the UPS has switched to battery power.

That transition alone does not immediately shut the computer down.

Instead, the main form keeps polling and reevaluates shutdown conditions on every update while `OB` remains active. The shutdown path starts when one of the following becomes true:

1. `PW_RespectFSD` is enabled and the UPS status transition contains `FSD`
2. battery charge is less than or equal to `PW_BattChrgFloor`
3. battery runtime is less than or equal to `PW_RuntimeFloor`

The current default values are:

- `PW_BattChrgFloor = 30`
- `PW_RuntimeFloor = 120`
- `PW_Immediate = false`
- `PW_RespectFSD = false`
- `PW_StopType = 0`
- `PW_StopDelaySec = 15`

So by default, the app does not immediately stop the machine when blackout begins. It waits until the UPS is on battery and one of the configured stop thresholds is reached, then starts a 15 second shutdown countdown.

## Shutdown Behavior

When the stop conditions are met, `Shutdown_Event()` is called.

There are two modes:

### Immediate shutdown

If `PW_Immediate` is enabled:

- the UPS connection is disconnected
- the app immediately calls `Shutdown_Action()`

### Countdown shutdown

If `PW_Immediate` is disabled, which is the default:

- the app shows `ShutdownGui`
- a countdown timer starts using `PW_StopDelaySec`
- the user can optionally extend the timer if `PW_UserExtendStopTimer` is enabled
- when the timer expires, the main form calls `Shutdown_Action()`

If mains power returns before the countdown completes, the app cancels the pending shutdown by stopping the shutdown timers and closing the shutdown form.

## The Actual Windows Power Action

The final power action depends on `PW_StopType`:

- `0`: full Windows shutdown
- `1`: suspend / sleep
- `2`: hibernate

For a full shutdown, the command used is:

`C:\WINDOWS\system32\Shutdown.exe -f -s -t 0`

Meaning:

- `-s`: shut down
- `-t 0`: no additional delay
- `-f`: force applications to close

This is compatible with Windows 10 and Windows 11. It uses the standard Windows shutdown executable and is appropriate for unattended emergency shutdown from a UPS workflow.

However, there is an important trade-off: `-f` is aggressive. It is good for making sure the machine actually goes down before battery is exhausted, but it can force close other applications that may still have unsaved user work.

For `PW_StopType = 1` and `2`, the app uses:

- `Application.SetSuspendState(PowerState.Suspend, false, true)`
- `Application.SetSuspendState(PowerState.Hibernate, false, true)`

Those calls are also valid on modern Windows, subject to the machine supporting the requested power state.

## Notifications and UI Updates

### What updates every poll

On every successful poll, the app refreshes:

- main form labels
- status colors
- gauge values
- battery runtime display
- tray icon text
- battery image state

So the visible UI is very much "live" while connected.

### What does not happen every poll

Popup notifications do not fire on every poll while the UPS remains on battery.

The intended on-battery / back-on-line popup code currently compares the same `UPS_Status` string to both `OL` and `OB` at the same time, which is impossible. As a result, those specific battery-transition popups do not currently fire through that path.

In practice, popups are currently associated with:

- connected
- lost connection
- disconnected

The app supports:

- Windows 10+ toast notifications via `Microsoft.Toolkit.Uwp.Notifications`
- `NotifyIcon.ShowBalloonTip(...)` as the fallback path

## Logging and Diagnostics

The application logs heavily, especially at debug level. During live polling this includes recurring debug entries for:

- refresh cycles
- battery icon state
- tray text updates
- threshold checks

This is useful for debugging live UPS behavior, but it also means logs can become noisy quickly at a one-second poll interval.

## Settings and Secret Handling

The app stores its user-configurable runtime settings outside the repository in per-user settings files.

Important implications:

- the live UPS IP address, UPS name, username, and password are not meant to live in tracked source files
- defaults in `App.config` and `Settings.settings` are placeholder values, not live device credentials
- the username and password are stored as `SerializedProtectedString`
- those credentials are intended to be protected using Windows DPAPI

This means a normal Git commit from the repository should not include the live UPS credentials unless a local runtime settings file is manually copied into the workspace. The `.gitignore` file has therefore been tightened to ignore likely local settings snapshots such as `user.config`, `.env`, and `*.local.config`.

## Connection and Reconnection Notes

`UpsDevice` owns both:

- the main update timer
- a reconnect timer

This lets the app continue monitoring after transient failures and keeps the UI in sync with connection state. The main form also updates menus, tray text, and connection labels as the connection lifecycle changes.

## Important Current Caveats

This repository is actively being reviewed and migrated, so the current logic should be understood with a few caveats in mind:

1. Battery transition popup logic currently appears broken, so on-battery / on-line toasts are not being triggered as intended.
2. The app depends on either valid battery charge/runtime data or an `FSD` instruction to decide when to stop the machine. If both battery charge and runtime are unavailable, threshold shutdown does not occur from that path.
3. The shutdown command uses `-f`, which is operationally safe for emergency unattended shutdown, but not the gentlest behavior for interactive applications with unsaved documents.
4. Debug builds do not exercise the exact same shutdown path as release builds because the actual power-action switch is inside `#if !DEBUG`.
5. `ShutdownGui` currently does some timer and UI-thread work that deserves further review if blackout-handling robustness becomes a higher priority.

## Practical Summary

From the user's point of view, the current logic is:

1. Connect to the NUT server
2. Poll once per second by default
3. When the UPS reports `OB`, show on-battery state
4. Keep polling battery/runtime values while on battery
5. When configured limits are reached, start the shutdown process
6. If power returns in time, cancel the shutdown
7. Otherwise issue a Windows shutdown, sleep, or hibernate action

That core behavior is working from a design perspective and matches what a UPS monitoring client is supposed to do, even though a few parts of the notification and shutdown implementation still deserve tightening as the fork matures.
