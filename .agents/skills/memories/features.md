---
id: features
title: Feature Instructions
content: Feature Instructions about this Mod
---

# Agent Instructions for Capture Them (Continued)

## Mod Overview and Purpose
Capture Them (Continued) is an updated version of busted_bunny's RimWorld mod designed to simplify the process of capturing downed enemy pawns. This mod integrates a new work type, "Capture", which alleviates the manual task of selecting pawns for capture and automates the process using a work queue system. It aims to enhance gameplay by streamlining the capture mechanic, allowing players to focus on other aspects of colony management.

## Key Features and Systems
- **New Work Type:** Introduces a "Capture" work type that falls between "Bed Rest" and "Basic" in the work priority menu.
- **Automation Tool:** Allows players to mark downed enemy pawns for capture using a new designation tool in the order menu.
- **Tool Accessibility:** The designation tool is easily accessible when selecting any downed pawns not belonging to the player's faction.
- **Compatibilities and Patches:**
  - Special patch compatibility with Misc. Robots++ integration.
  - Translations available in English and Simplified Chinese.

## Coding Patterns and Conventions
- Adopts standard C# coding practices, using clear class and method naming conventions for easier readability and maintenance.
- All method names follow PascalCase, while variable names follow camelCase conventions.
- Proper use of access modifiers to encapsulate implementation details and expose necessary functionalities.

## XML Integration
- **DesignationCategories.xml**: Defines the categories under which new designations appear.
- **Misc. Robots++ Integration (RPP_Races.xml)**: Provides work type definitions to integrate the capture function with Misc. Robots++.
- **ReturnToBedPatch.xml**: Adjustments to ensure pawns return captured individuals to beds effectively.

## Harmony Patching
- Harmony is used to apply non-invasive method patches for integrating new behaviors into RimWorld without altering its original code base.
- Patches include:
  - **Pawn_GuestTracker_SetGuestStatus.cs**: Postfix patch ensures captured pawns are properly assigned guest status.
  - **Pawn_HealthTracker_MakeUndowned.cs**: Prefix patch handles events when pawns regain consciousness.
  - **ReverseDesignatorDatabase_InitDesignators.cs**: Postfix patch adds the capture designator tool to the game's UI.

## Suggestions for Copilot
- **Pattern Recognition**: Automatically extend method stubs for existing class patterns, particularly in Designator and HarmonyPatch classes.
- **Consistent Code Formatting**: Suggest consistent formatting styles for XML files to maintain readability.
- **Efficient Patch Writing**: Assist in writing efficient Harmony patches by suggesting common patterns for prefix and postfix usage.
- **Mod Compatibility Handling**: Recommend practices for better handling of mod compatibility and conflict avoidance.
- **Localization Support**: Provide boilerplate for additional language support by referencing existing translation methods.
- **Error Logging Enhancements**: Propose enhanced error-logging features for better debugging and user support.

By adhering to these guidelines and leveraging the capabilities of GitHub Copilot, contributors can effectively maintain and enhance the Capture Them (Continued) mod to provide a seamless experience for RimWorld players.

This instruction file provides detailed guidance to developers and contributors working on the Capture Them (Continued) mod, ensuring consistent development and effective use of tooling.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.
