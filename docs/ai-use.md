# AI use log

| Tool | Date | Teammate | Purpose | Prompt | Response used | Changes | Verification | Related work |
|---|---|---|---|---|---|---|---|---|
| Claude Code | 2026-10-12 | Mateo | Understand the project, set up the repo | "Read the project PDF and our Figma" | Requirements summary, project structure, docs templates | Adjusted the work split | Project opened in Unity | 41f7d0f |
| Claude Code | 2026-10-12 | Mateo | Kiosk screens | "Help me with these details of the screens from our Figma" | `KioskUI.cs`, `KioskSceneBuilder.cs` | Chose "Notify me" for scenario B | Full flow tested, cases 1 to 4 | PR #2 |
| Claude Code | 2026-10-13 | Mateo | Free kits after their time | "How do we fix that a kit stays in use forever" | `FreeFinishedKits()` | None | Tested in Unity | cfa7be2 |
| Claude Code | 2026-10-13 | Mateo | Documentation | "Help me with the README, design decisions and test cases" | First drafts | Edited and shortened, results are from our tests | Checked against the app | PR #4 |
| Claude Code | 2026-10-13 | Mateo | Usability fixes | "How shall we fix P1 and P3" | Next button text, blocked durations | None | Screenshots and retest | PR #5 |
| Claude Code | 2026-10-12 to 13 | Mateo | Git and GitHub | "How do we organize branches and PRs" | Commands, PR texts | Edited every PR text | PRs in the repo | PRs #1 to #7 |
| ChatGPT | 2026-10-13 | Alexis | Understand Mateo's code and git commands | Asked how the scripts and commands work | Explanations | None | Approved PR #2 after testing | PR #2 |
| ChatGPT | 2026-10-13 | Alexis | Layout and TestBar changes | Asked how to align the buttons and hide the reset buttons | Show TestBar only on start screen | Positions adjusted by hand | Compared with the Figma | PR #3 |
| ChatGPT | 2026-10-13 | Alexis | Review the project and testing report | Asked to review them against the rubric | Missing items in the report | Results are from the real tests | Tests with P1 to P4 | 85a6d8f, 65ef1df |
