# Evidence and Reporting

## Evidence hierarchy

Grade evidence before adopting it:

- **A**: official HTRI documentation or locally verified runtime evidence on the exact installed version;
- **B+**: direct HTRI case/report for the target workflow, but not independently rerun;
- **B**: adjacent HTRI version/module or a credible engineering precedent;
- **C**: generic forum, screenshot, code fragment, or unverified wrapper;
- **D**: unclear, unsafe, polluted, or licensing-incompatible source.

GitHub code, UI automation, or an AI wrapper does not prove exact-version field safety. Keep unverified sources in quarantine until they pass a local sample and target-case test.

## Acceptance evidence package

Create a stable artifact directory containing:

```text
source/       immutable input references
model/        accepted HTRI models only
reports/      HTRI input/output reports and formal calculation note
results/      normalized SI tables and independent property/duty checks
qa/           logs, hashes, screenshots, rejected/development models
work/         scripts and temporary build files
```

The formal report must identify:

- HTRI product, version, module, mode, and model filename;
- source model hash and accepted model hash;
- process and utility boundary conditions;
- geometry source/revision;
- property method and phase treatment;
- coefficient method for each side;
- fouling and design margins;
- check/run return codes and messages;
- duty, area, EMTD, U, film coefficients, pressure drop, and overdesign;
- independent duty/property verification;
- sensitivities, limitations, and vendor-confirmation items.

## Wording rules

Use these labels precisely:

- **reported**: directly visible in a controlled source;
- **calculated**: reproduced from stated equations or software output;
- **assumed**: selected for analysis but not source-confirmed;
- **inferred**: derived from drawing interpretation;
- **to be confirmed**: required for acceptance but not yet proved;
- **conditional result**: valid only under imposed coefficient/geometry assumptions.

Never write `HTRI proves the equipment meets duty` when the geometry is a surrogate or a film coefficient is imposed. Write `the equivalent Rating meets the stated thermal duty under the listed assumptions`.

## Independent verification

Verify process duty with a separate enthalpy balance or simulator. Compare:

```text
Q_process = mass flow * (hout - hin)
Q_utility = utility mass flow * (hin - hout)
Q_HTRI    = reported HTRI duty
```

Resolve differences before acceptance. Check pressure basis, composition, phase fraction, property package, flow units, and temperature specifications first.

Reopen the accepted saved model in a fresh Automation Server process and rerun it. Record the second run log and result hash. A model that only runs in the creator process is not accepted.

## Minimum comparison table for vendor review

Compare the vendor case, current drawing/data sheet, and independent model for:

- mass flow;
- inlet/outlet temperature;
- pressure and pressure basis;
- phase fraction;
- duty;
- utility conditions;
- geometry and effective area;
- tube size, count/circuits, length, and passes;
- EMTD and correction method;
- clean/dirty/actual U;
- shell and tube film coefficients;
- fouling;
- pressure drop;
- coefficient method and any multiplier.

Treat a case with mismatched flow, outlet temperature, pressure basis, or geometry as a different case, not as proof of the current design.
