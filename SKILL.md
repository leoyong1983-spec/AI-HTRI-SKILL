---
name: ai-htri-skill
description: Control HTRI Xchanger Suite on Windows through the official Automation Server for environment verification, existing-case takeover, Xist input auditing, Rating reruns, result export, independent reopen verification, DWSIM property-table bridging, and auditable engineering reports. Use when Codex must inspect, run, modify, validate, or hand off HTRI `.htri` cases, compare vendor HTRI calculations, or build a bounded equivalent Rating for equipment without a native HTRI geometry model.
---

# AI HTRI Skill

Control HTRI with an automation-first, evidence-driven workflow. Prefer an existing runnable case or an official sample as the template; use the GUI only for visual inspection and final human review.

Use [public-source-index.md](references/public-source-index.md) for public-source provenance and version-scope boundaries. It does not replace licensed HTRI documentation or exact-version runtime verification.

## Operating Rules

1. Identify the exact HTRI version, executable, license availability, module, case path, and requested calculation mode before editing. Treat a version change as a calculation-method change until a frozen reference case has been rerun and its duty, U, pressure-drop, warning, and result deltas have been reviewed; opening or converging in the new version is not acceptance. When the target is Xchanger Suite 9.4, include applicable published changes—tube coatings, supercritical tubeside options for carbon dioxide/water, laminar twisted-tape methods, deep inline high-finned bundles, common inlet piping, and Final Results temperature-effectiveness reporting—in the regression scope; do not assume those methods or reports exist in older versions.
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
- design/Rating mode, selected service type, and every service-type-derived default that remains active;
- hot- and cold-side flow, composition, phase, pressure basis, temperatures, fouling, allowable pressure drop, and required duty;
- geometry source and revision;
- property source, thermodynamic method, and—where phase change or two liquid phases are present—the heat-release/phase-equilibrium basis and unresolved property warnings;
- required outputs and acceptance criteria.

Stop and label the case incomplete when gauge/absolute pressure, mass/molar flow, phase boundary, circuit count, effective area, or geometry revision is ambiguous.

### 2. Verify the environment

Run:

```powershell
& ".\scripts\Test-HtriEnvironment.ps1" -ProbeCom
```

Require the HTRI executable and `HtriCalc.HeatExchangerNetwork` COM ProgID to be available. Read [automation-server-xist-9.0.md](references/automation-server-xist-9.0.md) before changing fields.

Keep HTRIconnect separate from the local Automation Server route. If HTRIconnect is in scope, record the active membership entitlement, registered-file and data-binding scope, time-series calculation behavior, and whether parallel processing is enabled. Its public product description does not prove local COM field keys, writeback safety, or compatibility with the installed Xchanger Suite version; verify those items with a controlled exact-version run.

### 3. Take over an existing case

Use the existing case or official module sample as a template. Audit the model before changing it:

- module and unit count;
- stream names and side assignment;
- selected service type and the explicit value/source of each retained automatic default;
- process conditions and pressure basis;
- property source/package, heat-release or flash basis, phase fractions, and latent-heat/property warnings;
- geometry, tube dimensions, passes/circuits, effective area, fouling, and coefficient methods;
- imposed film coefficients or multipliers;
- run/error messages and invalid sentinel values; explicitly disposition low B-stream, fluid-property extrapolation, latent-heat, and local intube temperature-pinch warnings when present, and verify every message number and meaning against the installed version.

Do not accept a vendor model merely because it opens or converges. Compare the actual case conditions and geometry against the drawing/data sheet.

When a tube layout is created or edited, reconcile the saved layout against the controlled drawing and record every unresolved discrepancy. An automatically generated or visually similar layout is not drawing-confirmed geometry.

Export a structured input audit before making changes:

```powershell
& ".\scripts\Audit-HtriCase.ps1" `
  -InputFile "D:\case\source.htri" `
  -OutputDirectory "D:\case\artifacts\htri-audit"
```

### 4. Run a controlled Rating

Use the bundled runner for a source-preserving check, run, save-copy, and raw/SI result export:

```powershell
& ".\scripts\Invoke-HtriCase.ps1" `
  -InputFile "D:\case\source.htri" `
  -OutputDirectory "D:\case\artifacts\htri-rating"
```

Accept only when all gates pass:

- `OpenFile` succeeds;
- `CheckNetworkForRun=0`;
- `Run=3`;
- run messages contain `Run Completed` and `Solution Reached`;
- required outputs are finite and not HTRI sentinels;
- non-blocking warnings and post-processing monitors are dispositioned separately from thermal convergence; neither is cleared merely because the standard heat-transfer/pressure-drop solution converged;
- for condensing, boiling, or other phase-change cases, latent-heat and property warnings are resolved or their impact is quantified and explicitly accepted;
- the saved copy reopens and reruns independently;
- duty agrees with an independent enthalpy balance within the project tolerance.

When the verified installed version provides Engineering Checklists, treat them as a project-defined QA layer: preserve the rule-set identifier/version and actionable report, rerun the checklist after case changes, and disposition every failure. A checklist pass does not replace the independent geometry, property, warning, duty, pressure-drop, or operability review; do not assume that the feature or an automation interface exists in another HTRI version.

### 5. Build or modify a case

Change the smallest proven field set. After every change:

1. save to a new filename;
2. export input and output reports;
3. rerun the model;
4. compare the changed fields and results against the previous accepted case;
5. record assumptions and unresolved geometry/property issues.

For a parametric or sensitivity series, freeze and rerun the baseline template, vary only the declared field set, and retain a result-to-case mapping for every point. A bulk sweep must not be used to carry unverified field keys or unit assumptions into another HTRI version.

For external property generation, use a validated simulator such as DWSIM and preserve the property grid, simulator version, thermodynamic package, composition, pressure curves, and point count. HTRI 9.0 testing in this skill used 30-point curves; verify other versions before changing that count.

For two-phase, two-liquid-phase, or other special-property cases, document why the selected property package/model and heat-release basis are suitable for the service. Do not treat a default property selection as self-validating.

For boiling cases, record whether a true critical pressure is available and which reduced-pressure basis or density-ratio option is active on the installed version. Include that method selection in every version-regression comparison because it can change the boiling heat-transfer calculation.

For partial or complete condensers, audit the outlet-nozzle phase routing and confirm on the installed version whether the intended self-venting calculation was actually activated; preserve the Froude, flooding, and nozzle pressure-drop messages. Do not assume a single two-phase outlet is gravity self-venting or that its reported outlet-nozzle pressure drop is physically valid. For tubeside vacuum condensers with a decelerating velocity profile, also document how inlet/outlet-header momentum recovery is treated before accepting the pressure-drop margin.

For fouling-sensitive or temperature-limited service, evaluate both the fouled design state and the credible clean start-up/early-operation control state. Review local skin/wall temperatures, wetting or flow-regime indicators, duty control, and pressure drop; a satisfactory fouled design point alone does not establish acceptable clean-operation temperatures.

For U-tube cases, audit whether the modeled U-bend area is treated as effective for heat transfer. The setting is shell-type dependent and is controlled through the Supports tab in the Baffles panel; reconcile the selected setting and reported effective area against the actual support/layout geometry. Do not infer full U-bend effectiveness from nominal tube length, and do not transfer the setting or any shell-specific table between HTRI versions without an exact-version check.

For kettle reboilers or vaporizers, audit the heat-release-curve source and any version-specific modification, circulation, liquid level or bundle dryout, entrainment, vibration, and fouling. When a simplified or modified heat-release curve materially affects local bundle conditions, compare an alternative supported by the installed version and seek vendor or operating evidence; do not carry retired guidance into a current calculation without confirmation.

For thermosiphon reboilers, audit the full circulation loop rather than the exchanger alone: piping/static head, fluid properties and heat-release basis, pressure losses, circulation, wet-wall/dry-wall behavior, warnings, and turndown. A duty match without a closed loop pressure/circulation check is not acceptance.

For electric process-heater services, verify that the installed Xist version supports the intended service type; HTRI publicly places the electric-heater option in Xist 9.1 and later. Record the heater type, heating-element and bundle arrangement, power supply, instrumentation/controls, and the applicable design checklist. Review maximum watt density, radiation and convection, design margin, hot-spot avoidance, and other safety considerations. Treat the calculated thermal result as conditional: a converged case or public guidebook checklist does not establish hot-spot prediction, electrical protection, mechanical integrity, or operability. Require exact-version runtime evidence plus vendor, electrical, controls, and mechanical review before acceptance.

Treat Xist vibration results as screening evidence. For a U-tube design in which the longest unsupported span is in the U-bend, or when impingement-plate jetting, complex supports, or local flow orientation materially affect excitation, do not accept the Xist screen alone; use an exact-version Xvib analysis when available and require mechanical review. Preserve every Xist/Xvib warning and the modeled tube/support/velocity state.

For unbaffled X shells or exchangers with strongly asymmetric inlet/outlet, distributor, or bypass geometry, separate the global Xist duty and pressure-drop result from local flow-distribution acceptance. When maldistribution could control heat transfer, erosion, vibration, or structural loading, require project-specific distribution evidence such as validated CFD, vendor testing, or operating data; agreement on bundle pressure drop or duty alone does not prove acceptable local velocities.

When user-specified f/j factors or other custom heat-transfer/pressure-drop methods are used, label them as imposed methods. Preserve their source, applicability range, units, and sensitivity against the standard-method baseline; successful convergence does not validate a custom correlation.

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
- Check the current/retired status and supported-version scope before applying an HTRI TechTip; a retired TechTip is discovery evidence, not current calculation authority.
- Do not equate convergence with design adequacy, operability, mechanical integrity, or guaranteed performance.
