# Quest Checkout

Kiosk to borrow one of the six Meta Quest 3 kits. Made in Unity for the HCI second partial (Project 02), based on our Project 01 Figma kiosk.

The student picks an available kit, checks that all the items are in the kit, picks the loan duration, reviews the order (and can edit it) and confirms. The kiosk shows the pickup locker and the return deadline. If there are no kits available, the kiosk explains why, shows when the next kit will be free and lets the student ask to be notified.

Loans and the kit inventory are saved, so they are still there after closing and opening the app.

## Team
| Member | GitHub | Contributions |
|---|---|---|
| Mateo Gonzalez | glzhmto | |
| Alexis Vargas | AlexisVarBB | |

## Links
- Figma (Project 01): https://www.figma.com/design/DuTTjp4lgwnO90iWBKXV1Y/Project01
- Design justification: [docs/design.md](docs/design.md)
- Testing report: [docs/testing.md](docs/testing.md)
- AI use log: [docs/ai-use.md](docs/ai-use.md)
- Release / build: https://github.com/glzhmto/QuestCheckout/releases
- Demo video:

## Unity version
6000.3.8f1

## Setup
1. Clone the repo.
2. Open the folder with Unity Hub using Unity 6000.3.8f1.
3. Open `Assets/Scenes/Kiosk.unity` and press Play.

To run the build, download it, unzip it and open the app.

## Controls
- Mouse click on the buttons.
- **RESET A** and **RESET B** (top right, on the start screen) are testing controls to load each scenario.

## How to test
### Scenario A - borrow a kit
1. Press **RESET A**. Kits 02, 03 and 06 are available, 01 is charging and 04 and 05 are in use.
2. Tap to start, pick an available kit.
3. Tap each item of the kit to check it, then pick **90 MIN**.
4. On the review screen check the kit, duration and deadline, then confirm and lock the loan.
5. The last screen shows the pickup locker and the return deadline. The kit now shows as **IN USE**.

### Scenario B - no kits available
1. Press **RESET B**. All 6 kits are charging or in use.
2. Tap to start. The kiosk says there are no kits available.
3. Open **See wait times & alternatives** to see when the next kit is free.
4. Press **Notify me when a kit is ready**.

### Reset data
- **RESET A** and **RESET B** overwrite the saved data with that scenario.
- The data is saved with PlayerPrefs. To check it is saved, borrow a kit, stop the app (or close the build) and open it again.

## Known limitations
- "Notify me" only saves that the student wants a notice, it does not send a real email.
- 120 and 180 minutes are always blocked (fixed rule, like in the Figma).
- The times use the clock of the computer running the app.
- Uses the default Unity font, not the monospace font of the Figma.
