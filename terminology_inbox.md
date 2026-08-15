# Candidate Terminology and Rules

This is a candidate-only inbox for AI-HTRI-SKILL. It is not an approved calculation basis; `SKILL.md`, controlled project documents, HTRI licensed documentation, and verified runtime evidence remain controlling.

### 2026-07-27 - HTRI cross-version calculation regression boundary

- Status: to be confirmed.
- Type: engineering software / HTRI / calculation basis / version compatibility / result verification.
- Candidate rule: Public HTRI release information shows that calculation methods and property options can change between releases. When the HTRI version changes, rerun a frozen reference case and review duty, U, pressure-drop, warning/message, and key-result deltas before accepting a transferred model. Opening or converging alone is not acceptance.
- Constraint: Do not carry field keys, units, correlation behavior, or acceptance tolerances from one version into another without exact-version evidence. Any acceptable delta threshold requires project, vendor, or responsible-engineer confirmation.
- Scope: HTRI Xchanger Suite case takeover, version upgrades, Design/Rating comparison, and calculation-report review.
- Source and trigger: 2026-07-27 heartbeat review of the public HTRI Xchanger Suite 9.4 overview; local Automation Server evidence in this skill is limited to version 9.0.
- Suggested disposition: retain as a pending candidate; do not promote to a formal terminology rule without controlled benchmark evidence and user confirmation.

### 2026-07-27 - HTRI phase-change property warnings and clean/fouled operating states

- Status: to be confirmed.
- Type: engineering software / HTRI / property basis / fouling / calculation acceptance.
- Candidate rule: For condensing, boiling, two-liquid-phase, fouling-sensitive, or temperature-limited service, the HTRI review should justify the property package/model and heat-release basis, resolve or quantify latent-heat/property warnings, and check both the fouled design state and a credible clean start-up/early-operation control state.
- Constraint: Do not transfer warning codes, package-selection rules, fouling thresholds, or acceptable skin-temperature limits from a public example into a project. The project value must come from controlled HTRI documentation, process chemistry/material limits, vendor data, operating evidence, or responsible-engineer confirmation.
- Scope: HTRI Xist Design/Rating, property-table bridging, phase-change cases, fouling review, and thermal acceptance reports.
- Source and trigger: 2026-07-27 heartbeat review of HTRI public TechTips on physical properties and latent-heat warnings, plus the HTRI public debutanizer-reboiler fouling case study.
- Suggested disposition: retain as a pending candidate; formal adoption requires project-specific property and operating-condition confirmation.

### 2026-07-27 - HTRI geometry, kettle, and custom-correlation acceptance boundary

- Status: to be confirmed.
- Type: engineering software / HTRI / geometry / kettle modeling / custom methods.
- Candidate rule: Edited tube layouts must be reconciled to a controlled drawing; kettle calculations must explicitly audit heat-release-curve treatment, circulation, liquid level/dryout, entrainment, vibration, and fouling; user f/j factors or other custom methods remain imposed assumptions until independently validated.
- Constraint: Do not transfer Xchanger Suite 9.3 editor behavior to 9.0, reuse retired kettle guidance as current, or treat convergence as validation of a custom correlation. Project acceptance requires installed-version evidence, controlled geometry, method provenance, applicability range, and vendor/operating confirmation where material.
- Scope: HTRI Xist geometry takeover, kettle reboilers/vaporizers, custom heat-transfer or pressure-drop methods, and calculation reports.
- Source and trigger: 2026-07-27 heartbeat review of official HTRI public descriptions for tube-layout customization, kettle-vaporizer modeling/current training scope, and custom f/j-factor calculations.
- Suggested disposition: retain as a pending candidate; formal adoption requires exact-version and project evidence.

### 2026-07-27 - HTRI thermosiphon and vibration escalation boundary

- Status: to be confirmed.
- Type: engineering software / HTRI / thermosiphon / vibration / mechanical review.
- Candidate rule: A thermosiphon Rating requires a closed-loop piping/static-head, pressure-loss, property/heat-release, circulation and turndown review. Xist vibration output remains screening evidence; U-tube cases with the longest unsupported span in the U-bend or materially complex local excitation require exact-version Xvib analysis where available and mechanical review.
- Constraint: Do not use a duty match as proof of stable thermosiphon circulation, or a clean Xist screen as mechanical acceptance. Preserve warnings and modeled support/velocity states. Current/retired TechTip status must be checked before applying guidance.
- Scope: HTRI Xist/Xvib thermosiphon reboilers, U-tube exchangers, vibration review, and engineering handoff.
- Source and trigger: 2026-07-27 heartbeat review of HTRI's public Xist/Xvib comparison article, current Thermosiphon Reboilers course outline, and TechTips status index.
- Suggested disposition: retain as pending; formal adoption requires licensed guidance, exact-version runs, project geometry, and responsible mechanical/process review.

### 2026-07-27 - HTRI global Rating versus local flow-distribution boundary

- Status: to be confirmed.
- Type: engineering software / HTRI / Xist / shellside maldistribution / pressure drop / CFD.
- Candidate rule: For an unbaffled X shell or exchanger with strongly asymmetric nozzle, distributor, or bypass geometry, a global Xist duty or bundle pressure-drop result must be kept separate from acceptance of local flow distribution. Where local velocity affects heat transfer, erosion, vibration, or structural loading, obtain project-specific distribution evidence such as validated CFD, vendor testing, or operating data.
- Constraint: Do not transfer the numerical results or geometry changes from a public case study to another exchanger, and do not treat a CFD model calibrated to Xist bundle pressure drop as independent validation of the Xist thermal model. Exact software version, boundary conditions, turbulence/porous-media treatment, mesh independence, and validation basis remain project review items.
- Scope: Xist Rating and design review for X-shell condensers and other shell-and-tube exchangers with material inlet/outlet or bypass maldistribution risk.
- Source and trigger: 2026-07-27 heartbeat review of HTRI's public Stream Analysis Method overview and shellside-maldistribution condenser case study.
- Suggested disposition: retain as pending; formal adoption requires exact-version HTRI evidence, controlled geometry, and responsible process/mechanical review.

### 2026-07-27 - HTRI condenser outlet and service-default audit boundary

- Status: to be confirmed.
- Type: engineering software / HTRI / Xist / condenser / nozzle pressure drop / automatic defaults.
- Candidate rule: A condenser Rating must record its service type and expand every retained automatic default into the input audit. Outlet-nozzle phase routing, activation of the intended self-venting calculation, Froude/flooding messages, and—where relevant—vacuum-condenser momentum recovery must be checked before accepting pressure-drop margin.
- Constraint: Do not transfer the 2013/2014/2016 webinar's default values, numerical warning thresholds, or method behavior to the installed version without exact-version confirmation. Do not assume a single two-phase outlet is gravity self-venting or treat convergence as proof of a physically valid nozzle pressure drop.
- Scope: Xist partial/complete condensers, reflux condensers, tubeside vacuum condensers, existing-case takeover, and Design/Rating review.
- Source and trigger: 2026-07-27 heartbeat review of HTRI's public webinar descriptions for service-type defaults, self-venting condenser nozzles, and tubeside vacuum-condenser momentum recovery.
- Suggested disposition: retain as pending; formal adoption requires installed-version evidence, controlled process/nozzle geometry, and responsible process/mechanical review.

### 2026-07-27 - HTRI convergence, monitor, and boiling-method separation

- Status: to be confirmed.
- Type: engineering software / HTRI / convergence / post-processing monitor / boiling method / version regression.
- Candidate rule: Thermal convergence, non-blocking warnings, and post-processing monitor results are separate acceptance layers. For boiling cases, the audit must record whether true critical pressure is available and which reduced-pressure or density-ratio basis is active, then preserve that selection in version comparisons.
- Constraint: Do not infer that a monitor or warning affects the converged thermal calculation unless exact-version documentation says so; equally, do not dismiss an operability warning because it does not change standard duty or pressure-drop results. Xchanger Suite 8.2/9.0 method descriptions are version evidence, not universal equations or project acceptance limits.
- Scope: Xist boiling, thermosiphon, condenser, pressure-drop, warning/monitor review, and cross-version Rating acceptance.
- Source and trigger: 2026-07-27 heartbeat review of HTRI's public method-impact pages for Xchanger Suite 8.2 and 9.0.
- Suggested disposition: retain as pending; formal adoption requires installed-version output, licensed guidance where needed, and responsible process review.
