---
name: ai-htri-skill
description: Control HTRI Xchanger Suite on Windows through the official Automation Server for environment verification, existing-case takeover, Xist input auditing, Rating reruns, result export, independent reopen verification, DWSIM property-table bridging, and auditable engineering reports. Use when Codex must inspect, run, modify, validate, or hand off HTRI `.htri` cases, compare vendor HTRI calculations, or build a bounded equivalent Rating for equipment without a native HTRI geometry model.
---

# AI HTRI Skill

Control HTRI with an automation-first, evidence-driven workflow. Prefer an existing runnable case or an official sample as the template; use the GUI only for visual inspection and final human review.

Use [public-source-index.md](references/public-source-index.md) for public-source provenance and version-scope boundaries. It does not replace licensed HTRI documentation or exact-version runtime verification.

## Operating Rules

1. Identify the exact HTRI version, executable, license availability, module, case path, and requested calculation mode before editing. Treat a version change as a calculation-method change until a frozen reference case has been rerun and its duty, U, pressure-drop, warning, and result deltas have been reviewed; opening or converging in the new version is not acceptance.
2. Preserve every source file. Copy the case to a timestamped work/output directory and never overwrite the vendor or design source.
3. Prefer existing-case takeover. Do not build a blank case until the target module, object hierarchy, field keys, units, and success codes have been proved on the installed version.
4. Separate reported inputs, model assumptions, calculated outputs, and items to be confirmed.
5. Treat HTRI output as conditional on its geometry and coefficient methods. A converged run is not proof that the physical equipment is represented correctly.
6. Require an independent reopen and rerun before accepting a model.
7. Keep raw HTRI output beside every normalized SI result. Never discard warning lists, run codes, input copies, or hashes.

## Workflow

### 1. Establish the calculation contract

Record:

- installed HTRI version and module;
- source model and immutable source hash;
- design/Rating mode;
- hot- and cold-side flow, composition, phase, pressure basis, temperatures, fouling, allowable pressure drop, and required duty;
- geometry source and revision;
- property source and thermodynamic method;
- required outputs and acceptance criteria.

Stop and label the case incomplete when gauge/absolute pressure, mass/molar flow, phase boundary, circuit count, effective area, or geometry revision is ambiguous.

### 2. Verify the environment

Run:

```powershell
& "D:\SKILL\AI-HTRI-SKILL\scripts\Test-HtriEnvironment.ps1" -ProbeCom
```

Require the HTRI executable and `HtriCalc.HeatExchangerNetwork` COM ProgID to be available. Read [automation-server-xist-9.0.md](references/automation-server-xist-9.0.md) before changing fields.

### 3. Take over an existing case

Use the existing case or official module sample as a template. Audit the model before changing it:

- module and unit count;
- stream names and side assignment;
- process conditions and pressure basis;
- property source and phase fractions;
- geometry, tube dimensions, passes/circuits, effective area, fouling, and coefficient methods;
- imposed film coefficients or multipliers;
- run messages and invalid sentinel values.

Do not accept a vendor model merely because it opens or converges. Compare the actual case conditions and geometry against the drawing/data sheet.

Export a structured input audit before making changes:

```powershell
& "D:\SKILL\AI-HTRI-SKILL\scripts\Audit-HtriCase.ps1" `
  -InputFile "D:\case\source.htri" `
  -OutputDirectory "D:\case\artifacts\htri-audit"
```

### 4. Run a controlled Rating

Use the bundled runner for a source-preserving check, run, save-copy, and raw/SI result export:

```powershell
& "D:\SKILL\AI-HTRI-SKILL\scripts\Invoke-HtriCase.ps1" `
  -InputFile "D:\case\source.htri" `
  -OutputDirectory "D:\case\artifacts\htri-rating"
```

Accept only when all gates pass:

- `OpenFile` succeeds;
- `CheckNetworkForRun=0`;
- `Run=3`;
- run messages contain `Run Completed` and `Solution Reached`;
- required outputs are finite and not HTRI sentinels;
- the saved copy reopens and reruns independently;
- duty agrees with an independent enthalpy balance within the project tolerance.

### 5. Build or modify a case

Change the smallest proven field set. After every change:

1. save to a new filename;
2. export input and output reports;
3. rerun the model;
4. compare the changed fields and results against the previous accepted case;
5. record assumptions and unresolved geometry/property issues.

For a parametric or sensitivity series, freeze and rerun the baseline template, vary only the declared field set, and retain a result-to-case mapping for every point. A bulk sweep must not be used to carry unverified field keys or unit assumptions into another HTRI version.

For external property generation, use a validated simulator such as DWSIM and preserve the property grid, simulator version, thermodynamic package, composition, pressure curves, and point count. HTRI 9.0 testing in this skill used 30-point curves; verify other versions before changing that count.

### 6. Handle non-native equipment

Read [equivalent-rating-boundaries.md](references/equivalent-rating-boundaries.md) before modeling water-bath coils, wound coils, or other non-native geometry.

Use an equivalent Rating only to answer a stated thermal question. Do not use surrogate shell/tube pressure drop, vibration, nozzle loss, or flow distribution as manufacturing guarantees.

Whenever a film coefficient or multiplier is imposed:

- label the resulting overall `U` as conditional, not predicted;
- show the shell film, tube film, wall resistance, fouling, and area basis;
- run sensitivities on both sides and on effective area/flow distribution;
- compare against vendor data or operating tests;
- do not present an ideal-distribution `U` as an equipment guarantee.

### 7. Report and hand off

Follow [evidence-and-reporting.md](references/evidence-and-reporting.md). Deliver at minimum:

- source hash and accepted rerun model;
- calculation contract and input summary;
- raw HTRI report/output;
- normalized SI result table;
- check/run message log;
- independent duty/property verification;
- assumptions, sensitivities, limitations, and items requiring vendor confirmation.

Keep temporary probes and failed models outside the formal report. Store them in a separate QA/development directory.

## Boundaries

- Do not bypass HTRI licensing, registration, or access controls.
- Do not redistribute HTRI binaries, proprietary samples, type libraries, or vendor calculation files in this skill.
- Do not claim native support for a geometry that the installed HTRI module does not provide.
- Do not use raw internal field keys across HTRI versions without a local verification run.
- Do not equate convergence with design adequacy, operability, mechanical integrity, or guaranteed performance.
