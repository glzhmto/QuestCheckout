# Testing report

Build tested: v1.0.0 (Mac build from commit 02158fd)

## Functional cases
| # | Case | Setup | Steps | Expected | Actual | Pass/Fail | Evidence |
|---|---|---|---|---|---|---|---|
| 1 | Checkout | Reset to Scenario A | 1. Click Start<br>2. Select available kit (#02)<br>3. Check all 3 items<br>4. Select 90 MIN<br>5. Review & Confirm loan | Locker assigned (Locker #02) and return deadline displayed on Done screen. | Locker assigned and return time correctly calculated and displayed. | Pass | docs/images/evidence1.png |
| 2 | No availability | Reset to Scenario B | 1. Click Start<br>2. Observe 0 available kits<br>3. Click "See Alternatives"<br>4. Click "Notify me" | System displays estimated wait time for next kit and confirms email notification status. | Next available time correctly calculated; "Notify me" button disables and shows confirmation message. | Pass | docs/images/evidence2.png |
| 3 | Invalid kit | Reset to Scenario A | 1. Click Start<br>2. Click kit marked "CHARGING" or "IN USE" | Warning message displays showing when the kit will be free; selection blocked. | System prevented selection and displayed exact time of availability. | Pass | docs/images/evidence3.png |
| 4 | Editing choices | Reset to Scenario A | 1. Proceed to Review screen<br>2. Click "Edit kit" and pick new kit<br>3. Click "Edit duration" | App navigates back seamlessly; retains checked items and updates summary. | Screen back-navigation worked as expected and updated kit details in review. | Pass | docs/images/evidence4.png |
| 5 | Saved data after restart | Complete loan in Case 1 | 1. Close app / Stop Play mode<br>2. Relaunch app<br>3. Check kit status | Previously borrowed kit remains IN USE with saved deadline via PlayerPrefs. | Borrowed kit status and freeAt timestamp persisted correctly across restart. | Pass | docs/images/evidence5.png |
| 6 | Scenario reset | Active loan session | 1. Click RESET A or RESET B from TestBar on Start screen | Clears/overwrites PlayerPrefs and resets kit states to default Scenario A or B. | Scenario reset applied immediately and refreshed UI state without errors. | Pass | docs/images/evidence6.png |

## User tests
| Participant | Scenario | Completed | Time | Errors | Help | Observations |
|---|---|---|---|---|---|---|
| P1 | A | Yes | 35.34s | 1 | None | Attempted to proceed without checking all kit items. Realized "Next" button was disabled, completed checklist independently. |
| P1 | B | Yes | 17.55s | 0 | None | Understood zero availability state quickly; navigated to Alternatives and activated notification. |
| P2 | A | Yes | 23.09s | 0 | None | Smooth and intuitive checkout flow; selected kit, verified items, and confirmed loan without hesitation. |
| P2 | B | Yes | 14.56s | 0 | None | Direct interaction with no availability screen; immediately clicked "See Alternatives" and registered. |
| P3 | A | Yes | 28.12s | 1 | None | Tried selecting 120 min duration option. Read warning text about class reservation at 12:00 PM, adjusted to 90 min and finished. |
| P3 | B | Yes | 16.40s | 0 | None | Quickly identified wait time details on Alternatives screen and requested notification. |

## Fixes
### Fix 1
- **Problem:** TestBar buttons remained permanently visible across all kiosk screens, causing visual clutter and potential misclicks during regular checkout flow.
- **Change:** Updated `KioskUI.cs` to auto-detect `TestBar` dynamically and toggle its active status so it only displays during `startScreen`.
- **Before:** TestBar stayed visible on top of every sub-screen.
- **After / retest:** TestBar automatically hides when entering checkout/alternatives screens and reappears on restart. Retested and verified in Build `v1.0.0`.

### Fix 2
- **Problem:** Navigation and action button bars across multiple screens were improperly aligned relative to the Canvas bounds, negatively impacting aesthetic appeal and touch targets.
- **Change:** Re-aligned anchors, positioning, and layout properties for bottom navigation containers directly in `Kiosk.unity` Inspector.
- **Before:** Crooked/misaligned UI buttons relative to screen borders.
- **After / retest:** Clean, visually symmetrical, and responsive bottom bar layouts on all screens. Retested and verified in Build `v1.0.0`.

## Unresolved issues
- "Notify me" only saves that the student asked for a notice. It does not send a real email.
- 120 and 180 minutes are always blocked (fixed rule for the 12:00 PM class), they don't depend on a real schedule.
- There is no return flow at the kiosk. A kit becomes available again only when its time is over, so a kit returned early still shows IN USE.
- The times use the clock of the computer running the app.
- The Figma uses a monospace font for small text. The app uses the default Unity font.
- Only tested on the Mac build.
