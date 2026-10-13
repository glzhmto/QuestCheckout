# Quest Checkout

Unity kiosk to borrow one of six Meta Quest 3 kits, based on our Project 01 Figma (HCI, Project 02).

The student picks a kit, checks its items, picks the duration, reviews (and can edit) the order and confirms. The kiosk shows the pickup locker and return deadline. If no kits are available, it explains why, shows when the next one is free and lets the student ask to be notified. Loans are saved after closing the app.

## Team
| Member | GitHub | Contributions |
|---|---|---|
| Mateo Gonzalez | glzhmto | Project setup, kit inventory and saved data (PR #1), screens and checkout flow (PR #2), README and design (PR #4), usability fixes (PR #5). Reviewed PR #3. |
| Alexis Vargas | AlexisVarBB | Kiosk scene and layout (PR #3), functional and user tests (docs/testing.md), Releases v1.0.0 and v1.1.0. Reviewed PR #2. |

## Links
- Figma: https://www.figma.com/design/DuTTjp4lgwnO90iWBKXV1Y/Project01
- [Design](docs/design.md) · [Testing](docs/testing.md) · [AI use](docs/ai-use.md)
- Release: https://github.com/glzhmto/QuestCheckout/releases
- Demo video:

## Setup
Unity **6000.3.8f1**. Clone the repo, open it with Unity Hub, open `Assets/Scenes/Kiosk.unity` and press Play.
For the build, download the zip from the Release, unzip it and open the app.

## Controls
Mouse click on the buttons. **RESET A** and **RESET B** (top right of the start screen) load each test scenario.

## How to test
**Scenario A - borrow a kit**
1. Press RESET A (kits 02, 03 and 06 available).
2. Start, pick an available kit, check its 3 items and pick 90 MIN.
3. Review, confirm and lock the loan. The last screen shows the locker and deadline, and the kit is now IN USE.

**Scenario B - no kits**
1. Press RESET B (all kits charging or in use).
2. Start. The kiosk says no kits are available.
3. Open See wait times & alternatives and press Notify me.

**Reset and saved data:** RESET A / RESET B overwrite the saved data. To check saving, borrow a kit, close the app and open it again.

## Known limitations
- "Notify me" doesn't send a real email.
- 120 and 180 min are always blocked (fixed rule, like in the Figma).
- Times use the computer's clock.
- Default Unity font instead of the Figma monospace font.
- The UNAVAILABLE label is cut on the 120 and 180 buttons.
